using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class AffiliateController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/UserAffiliate
        [Route("api/UserAffiliate/{UserName}")]
        public List<Models.Affiliate> GetUserAffiliate(string UserName)
        {
            var retryCounter = 0;
            List<Models.Affiliate> values;

            while (true)
            {
                try
                {
                    var Affiliate = from d in db.TrnAffiliates
                                    where d.MstUser.UserName == UserName
                                    select new Models.Affiliate
                                    {
                                        Id = d.Id,
                                        ProductPackageId = d.ProductPackageId,
                                        ProductPackage = d.MstProductPackage.ProductPackage,
                                        AffiliateURL = d.AffiliateURL,
                                        Price = d.MstProductPackage.Price,
                                        UserId = d.UserId,
                                        User = d.MstUser.UserName
                                    };
                    if (Affiliate.Count() > 0)
                    {
                        values = Affiliate.ToList();
                    }
                    else
                    {
                        values = new List<Models.Affiliate>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.Affiliate>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }
    }
}
