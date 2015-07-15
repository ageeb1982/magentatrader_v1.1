﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class SalesController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/Sales
        [Authorize]
        public List<Models.Sales> Get()
        {
            var retryCounter = 0;
            List<Models.Sales> values;

            while (true)
            {
                try
                {
                    var Sales = from d in db.TrnSales
                                join p in db.MstProductPackages on d.ProductPackageId equals p.Id
                                select new Models.Sales
                                {
                                    Id = d.Id,
                                    ProductPackageId = d.ProductPackageId,
                                    ProductPackage = d.MstProductPackage.ProductPackage,
                                    UserId = d.UserId,
                                    User = d.MstUser.UserName,
                                    FirstName = d.MstUser.FirstName,
                                    LastName = d.MstUser.LastName,
                                    SalesNumber = d.SalesNumber,
                                    SalesDate = Convert.ToString(d.SalesDate.Year) + "-" + Convert.ToString(d.SalesDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.SalesDate.Day + 100).Substring(1, 2),
                                    RenewalDate = d.RenewalDate.ToShortDateString(),
                                    ExpiryDate = d.ExpiryDate.ToShortDateString(),
                                    Particulars = d.Particulars,
                                    Quantity = d.Quantity,
                                    Price = d.Price,
                                    Amount = d.Amount,
                                    IsActive = d.IsActive,
                                    IsRefunded = d.IsRefunded,
                                    SalesStatus = d.SalesStatus,
                                    Group = p.ProductPackageGroup,
                                    SalesAmount = d.SalesStatus.ToString() == "OK" ? d.Amount : 0
                                };
                    if (Sales.Count() > 0)
                    {
                        values = Sales.ToList();
                    }
                    else
                    {
                        values = new List<Models.Sales>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.Sales>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        // GET api/UserSales
        [Authorize]
        [Route("api/UserSales/{UserName}")]
        public List<Models.Sales> GetUserSales(string UserName)
        {
            var retryCounter = 0;
            List<Models.Sales> values;

            while (true)
            {
                try
                {
                    var Sales = from d in db.TrnSales
                                where d.MstUser.UserName == UserName
                                orderby d.SalesDate descending
                                select new Models.Sales
                                {
                                    Id = d.Id,
                                    ProductPackageId = d.ProductPackageId,
                                    ProductPackage = d.MstProductPackage.ProductPackage,
                                    ProductPackageURL = d.MstProductPackage.PackageURL,
                                    UserId = d.UserId,
                                    User = d.MstUser.UserName,
                                    FirstName = d.MstUser.FirstName,
                                    LastName = d.MstUser.LastName,
                                    SalesNumber = d.SalesNumber,
                                    SalesDate = Convert.ToString(d.SalesDate.Year) + "-" + Convert.ToString(d.SalesDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(d.SalesDate.Day + 100).Substring(1, 2),
                                    RenewalDate = d.RenewalDate.ToShortDateString(),
                                    ExpiryDate = d.ExpiryDate.ToShortDateString(),
                                    Particulars = d.Particulars,
                                    Quantity = d.Quantity,
                                    Price = d.Price,
                                    Amount = d.Amount,
                                    IsActive = d.IsActive,
                                    IsRefunded = d.IsRefunded
                                };
                    if (Sales.Count() > 0)
                    {
                        values = Sales.ToList();
                    }
                    else
                    {
                        values = new List<Models.Sales>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.Sales>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        // POST api/AddSales
        [Authorize]
        [Route("api/AddSales")]
        public int Post(Models.Sales value)
        {
            try
            {

                Data.TrnSale NewSale = new Data.TrnSale();

                SqlDateTime SalesDate = new SqlDateTime(new DateTime(Convert.ToDateTime(value.SalesDate).Year, +
                                                                     Convert.ToDateTime(value.SalesDate).Month, +
                                                                     Convert.ToDateTime(value.SalesDate).Day));
                SqlDateTime RenewalDate = new SqlDateTime(new DateTime(Convert.ToDateTime(value.RenewalDate).Year, +
                                                                       Convert.ToDateTime(value.RenewalDate).Month, +
                                                                       Convert.ToDateTime(value.RenewalDate).Day));
                SqlDateTime ExpiryDate = new SqlDateTime(new DateTime(Convert.ToDateTime(value.ExpiryDate).Year, +
                                                                      Convert.ToDateTime(value.ExpiryDate).Month, +
                                                                      Convert.ToDateTime(value.ExpiryDate).Day));

                NewSale.ProductPackageId = value.ProductPackageId;
                NewSale.UserId = value.UserId;
                NewSale.SalesNumber = value.SalesNumber;
                NewSale.SalesDate = SalesDate.Value;
                NewSale.RenewalDate = RenewalDate.Value;
                NewSale.ExpiryDate = ExpiryDate.Value;
                NewSale.Particulars = value.Particulars;
                NewSale.Quantity = value.Quantity;
                NewSale.Price = value.Price;
                NewSale.Amount = value.Amount;
                NewSale.IsActive = value.IsActive;
                NewSale.IsRefunded = value.IsRefunded;

                db.TrnSales.InsertOnSubmit(NewSale);
                db.SubmitChanges();

                return NewSale.Id;
            }
            catch
            {
                return 0;
            }
        }

        // PUT /api/UpdateSales/5
        [Authorize]
        [Route("api/UpdateSales/{Id}")]
        public HttpResponseMessage Put(String Id, Models.Sales value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                var Sales = from d in db.TrnSales where d.Id == id select d;

                if (Sales.Any())
                {
                    var UpdatedSale = Sales.FirstOrDefault();

                    DateTime salesDate = Convert.ToDateTime(value.SalesDate, new CultureInfo("en-US"));
                    DateTime renewalDate = Convert.ToDateTime(value.RenewalDate, new CultureInfo("en-US"));
                    DateTime expiryDate = Convert.ToDateTime(value.ExpiryDate, new CultureInfo("en-US"));

                    SqlDateTime SalesDate = new SqlDateTime(new DateTime(Convert.ToDateTime(salesDate).Year, +
                                                                         Convert.ToDateTime(salesDate).Month, +
                                                                         Convert.ToDateTime(salesDate).Day));
                    SqlDateTime RenewalDate = new SqlDateTime(new DateTime(Convert.ToDateTime(renewalDate).Year, +
                                                                           Convert.ToDateTime(renewalDate).Month, +
                                                                           Convert.ToDateTime(renewalDate).Day));
                    SqlDateTime ExpiryDate = new SqlDateTime(new DateTime(Convert.ToDateTime(expiryDate).Year, +
                                                                          Convert.ToDateTime(expiryDate).Month, +
                                                                          Convert.ToDateTime(expiryDate).Day));

                    UpdatedSale.ProductPackageId = value.ProductPackageId;
                    UpdatedSale.UserId = value.UserId;
                    UpdatedSale.SalesNumber = value.SalesNumber;
                    UpdatedSale.SalesDate = SalesDate.Value;
                    UpdatedSale.RenewalDate = RenewalDate.Value;
                    UpdatedSale.ExpiryDate = ExpiryDate.Value;
                    UpdatedSale.Particulars = value.Particulars;
                    UpdatedSale.Quantity = value.Quantity;
                    UpdatedSale.Price = value.Price;
                    UpdatedSale.Amount = value.Amount;
                    UpdatedSale.IsActive = value.IsActive;
                    UpdatedSale.IsRefunded = value.IsRefunded;

                    db.SubmitChanges();
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (NullReferenceException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/DeleteSales/5
        [Authorize]
        [Route("api/DeleteSales/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.TrnSale DeleteSale = db.TrnSales.Where(d => d.Id == Id).First();

            if (DeleteSale != null)
            {
                db.TrnSales.DeleteOnSubmit(DeleteSale);
                try
                {
                    db.SubmitChanges();
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}
