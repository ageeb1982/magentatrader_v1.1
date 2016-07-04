using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class VariableScannerController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        private List<StockPriceData> getStockPrices(String symbol)
        {
            var symbols = from d in db.MstSymbols
                          where d.Symbol == symbol
                          select d;

            if (symbols.Any())
            {
                DateTime date1 = new DateTime();
                DateTime date2 = new DateTime();

                date2 = symbols.FirstOrDefault().LatestQuoteDate.Value;
                date1 = ((date2.AddDays(1)).AddYears(-10)).AddMonths(-5);

                var prices = (from d in db.TrnStockPrices
                             where (d.Symbol == symbols.FirstOrDefault().Symbol) &&
                                    (d.QuoteDate >= date1 && d.QuoteDate <= date2) &&
                                    (d.ClosePrice > 0) &&
                                    (d.Volume > 0)
                             orderby d.QuoteDate descending
                              select new StockPriceData
                             {
                                QuoteDate = d.QuoteDate,
                                OpenPrice = d.OpenPrice,
                                HighPrice = d.HighPrice,
                                LowPrice = d.LowPrice,
                                ClosePrice = d.ClosePrice,
                                Volume = d.Volume,
                                Season = 0,
                                SeasonCount = 0
                             }).ToList();
                return prices;
            }
            else
            {
                return new List<StockPriceData>(); 
            }
        }
        private Int32 sign(Decimal n)
        {
            return n < 0 ? -1 : 1;
        }
        private ScanResult getScanResult(String direction, 
                                         Decimal[,] closeYearData,
                                         Int32 index,
                                         Int32 range,
                                         Int32 noOfYears)
        {
            ScanResult result = new ScanResult();

            decimal up = 0;
            decimal down = 0;
            decimal averageUp = 0;
            decimal averageDown = 0;
            List<ScanYearlyResult> upYear = new List<ScanYearlyResult>();
            List<ScanYearlyResult> downYear = new List<ScanYearlyResult>();

            if (direction == "up") {
                for (var y = 0; y < noOfYears; y++) {
                    var startData = closeYearData[y,index];
                    var endData = closeYearData[y,index + range - 1];
                    if (endData > startData) {
                        up++;
                        if (startData != 0) {
                            ScanYearlyResult newUpYear = new ScanYearlyResult();
                            newUpYear.Year = Math.Round(((((endData - startData) / startData) * sign(startData)) * 100), 2);
                            upYear.Add(newUpYear);
                            averageUp = averageUp + Math.Round(((((endData - startData) / startData) * sign(startData)) * 100),2);
                        }
                    }
                }
                if (up > 0) {
                    averageUp = averageUp / up;
                }
                result.Up = up;
                result.UpAverage = averageUp;
                result.UpYear = upYear;
                result.Down = 0;
                result.DownAverage = 0;
                result.DownYear = downYear;
            } else {
                for (var y = 0; y < noOfYears; y++) {
                    var startData = closeYearData[y,index];
                    var endData = closeYearData[y,index + range - 1];
                    if (startData > endData) {
                        down++;
                        if (endData != 0) {
                            ScanYearlyResult newDownYear = new ScanYearlyResult();
                            newDownYear.Year = Math.Round(((((startData - endData) / endData) * sign(endData)) * 100), 2);
                            downYear.Add(newDownYear);
                            averageDown = averageDown + Math.Round(((((startData - endData) / endData) * sign(endData)) * 100),2);
                        }
                    }
                }
                if (down > 0) {
                    averageDown = averageDown / down;
                }
                result.Up = 0;
                result.UpAverage = 0;
                result.UpYear = upYear;
                result.Down = down;
                result.DownAverage = averageDown;
                result.DownYear = downYear;
            }
            return result;
        }

        // GET api/VariableScanner
        [Authorize]
        [Route("api/VariableScanner")]
        public List<Models.VariableScanner> GetVariableScanner([FromUri] List<String> Symbols)
        {
            List<Models.VariableScanner> data = new List<Models.VariableScanner>();
            try
            {
                for (int s = 0; s < Symbols.Count; s++)
                {
                    List<StockPriceData> prices = getStockPrices(Symbols[s]);

                    // ===========================
                    // Get the First Closing Price
                    // ===========================

                    int dayIndex = 126;
                    int countYear = 0;
                    int i = 0;
                    int d = 0;
                    int r = 0;
                    decimal[] firstClosingPrice = new decimal[10];

                    foreach (StockPriceData p in prices)
                    {
                        p.SeasonCount = dayIndex;
                        dayIndex--;

                        if (dayIndex == 0)
                        {
                            dayIndex = 252;

                            if (countYear > 0)
                            {
                                firstClosingPrice[countYear - 1] = p.ClosePrice;
                            }
                            countYear++;
                        }
                        else
                        {
                            if (i == prices.Count() - 1)
                            {
                                if (countYear > 0 && countYear < 11)
                                {
                                    firstClosingPrice[countYear - 1] = p.ClosePrice;
                                }
                            }
                        }
                        i++;
                    }

                    // ===========================
                    // Compute for the Yearly Data
                    // ===========================

                    decimal season = 0;
                    decimal[,] seasonYearData = new decimal[10, 252];
                    decimal[,] closeYearData = new decimal[10, 252];
                    countYear = 0;
                    dayIndex = 251;
                    i = 0;
                    foreach (StockPriceData p in prices)
                    {
                        if (i > 125)
                        {
                            if (firstClosingPrice[countYear] > 0)
                            {
                                season = ((p.ClosePrice - firstClosingPrice[countYear]) / firstClosingPrice[countYear]) * 100;
                            }
                            else
                            {
                                season = 0;
                            }

                            p.Season = season;

                            seasonYearData[countYear, dayIndex] = season;
                            closeYearData[countYear, dayIndex] = p.ClosePrice;

                            dayIndex--;
                            if (dayIndex == -1)
                            {
                                dayIndex = 251;
                                countYear++;
                                if (countYear > 9) break;
                            }
                        }
                        i++;
                    }

                    // ==================================
                    // Compute for Ten Year Seasonal Data
                    // ==================================

                    decimal[] SeasonTenYearData = new decimal[252];
                    for (i = 0; i < 252; i++)
                    {
                        season = 0;
                        for (int y = 0; y < 10; y++)
                        {
                            season = season + seasonYearData[y, i];
                        }
                        SeasonTenYearData[i] = season;
                    }

                    // =================
                    // Plot data (1-252)
                    // =================

                    DateTime futureDate = prices.First().QuoteDate.AddDays(1);
                    List<StockPriceData> plotData = new List<StockPriceData>();
                    for (i = 0; i < 252; i++)
                    {
                        if (i > 125)
                        {
                            if (futureDate.ToString("ddd") == "Sun" || futureDate.ToString("ddd") == "Sat")
                            {
                                i--;
                            }
                            else
                            {
                                plotData.Add(new StockPriceData
                                {
                                    QuoteDate = futureDate,
                                    Season = SeasonTenYearData[i] / 10,
                                    SeasonCount = i + 1
                                });
                            }
                            futureDate = futureDate.AddDays(1);
                        }
                        else
                        {
                            StockPriceData p = prices[125 - i];
                            plotData.Add(new StockPriceData
                            {
                                QuoteDate = p.QuoteDate,
                                OpenPrice = p.OpenPrice,
                                HighPrice = p.HighPrice,
                                LowPrice = p.LowPrice,
                                ClosePrice = p.ClosePrice,
                                Volume = p.Volume,
                                Season = SeasonTenYearData[i] / 10,
                                SeasonCount = i + 1
                            });
                        }
                    }

                    // =========
                    // Scan Data
                    // =========

                    int startNoOfDay = 10;
                    int endNoOfDay = 40;
                    int noOfYears = firstClosingPrice.Length;

                    decimal highestUp = 0;
                    decimal highestUpAverage = 0;
                    int highestUpIndex = 0;
                    int highestUpDaySpan = 0;
                    decimal highestDown = 0;
                    decimal highestDownAverage = 0;
                    int highestDownIndex = 0;
                    int highestDownDaySpan = 0;

                    List<Models.YearPercentage> upYearPercentage = new List<Models.YearPercentage>();
                    List<Models.YearPercentage> downYearPercentage = new List<Models.YearPercentage>();

                    for (d = startNoOfDay; d <= endNoOfDay; d++) {
                        for (r = 125 ; r < 252 - d ; r++) {
                            decimal seasonTenYear = plotData[r].Season;
                            decimal seasonTenYearPlusVarDays = plotData[r + d - 1].Season;
                            ScanResult result = new ScanResult();

                            if (seasonTenYearPlusVarDays > seasonTenYear) {
                                result = getScanResult("up",closeYearData, r, d, noOfYears);
                            } else {
                                result = getScanResult("down", closeYearData, r, d, noOfYears);
                            }

                            if (result.Up > 0) {
                                if (result.Up > highestUp || (result.Up == highestUp && result.UpAverage > highestUpAverage))
                                {
                                    highestUp = result.Up;
                                    highestUpAverage = result.UpAverage;
                                    highestUpIndex = r;
                                    highestUpDaySpan = d;
                                    upYearPercentage = new List<Models.YearPercentage>();
                                    foreach (ScanYearlyResult uy in result.UpYear)
                                    {
                                        Models.YearPercentage newUpYearPercentage = new Models.YearPercentage();
                                        newUpYearPercentage.Percentage = uy.Year;
                                        upYearPercentage.Add(newUpYearPercentage);
                                    }
                                }
                            }
                            else if (result.Down> 0)
                            {
                                if (result.Down > highestDown || (result.Down == highestDown && result.DownAverage > highestDownAverage))
                                {
                                    highestDown = result.Down;
                                    highestDownAverage = result.DownAverage;
                                    highestDownIndex = r;
                                    highestDownDaySpan = d;
                                    downYearPercentage = new List<Models.YearPercentage>();
                                    foreach (ScanYearlyResult dy in result.DownYear)
                                    {
                                        Models.YearPercentage newDownYearPercentage = new Models.YearPercentage();
                                        newDownYearPercentage.Percentage = dy.Year;
                                        downYearPercentage.Add(newDownYearPercentage);
                                    }
                                }
                            }

                        }
                    }

                    // ===========
                    // Save Result
                    // ===========

                    Models.VariableScanner newData = new Models.VariableScanner();

                    newData.Symbol = Symbols[s];
                    if (highestUp > 0)
                    {
                        newData.UpDate = Convert.ToString(plotData[highestUpIndex].QuoteDate.Year) + "-" + Convert.ToString(plotData[highestUpIndex].QuoteDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(plotData[highestUpIndex].QuoteDate.Day + 100).Substring(1, 2);
                        newData.UpNoOfDays = highestDownDaySpan;
                        newData.UpRate = "Up: " + highestUp + "/" + (noOfYears - highestUp);
                        newData.UpPercentage = Math.Round(highestUpAverage, 2);
                        newData.UpYearPercentage = upYearPercentage;
                    }
                    if (highestDown > 0)
                    {
                        newData.DownDate = Convert.ToString(plotData[highestDownIndex].QuoteDate.Year) + "-" + Convert.ToString(plotData[highestDownIndex].QuoteDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(plotData[highestDownIndex].QuoteDate.Day + 100).Substring(1, 2);
                        newData.DownNoOfDays = highestDownDaySpan;
                        newData.DownRate = "Down: " + highestDown + "/" + (noOfYears - highestDown);
                        newData.DownPercentage = Math.Round(highestDownAverage, 2);
                        newData.DownYearPercentage = downYearPercentage;
                    }

                    data.Add(newData);
                }
            } catch {
                data = new List<Models.VariableScanner>();
            }
            return data.ToList();
        }
    }

    public class StockPriceData
    {
        public DateTime QuoteDate { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Volume { get; set; }
        public decimal Season { get; set; }
        public int SeasonCount { get; set; }
    }
    public class ScanResult {
        public decimal Up {get; set;}
        public decimal UpAverage {get; set;}
        public List<ScanYearlyResult> UpYear { get; set; }
        public decimal Down {get; set;}
        public decimal DownAverage {get; set;}
        public List<ScanYearlyResult> DownYear { get; set; }
    }
    public class ScanYearlyResult
    {
        public decimal Year { get; set; }
    }
}
