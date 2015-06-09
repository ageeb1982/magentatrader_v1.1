using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class ProductPackageController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/ProductPackage
        [Authorize]
        public List<Models.ProductPackage> Get()
        {
            var retryCounter = 0;
            List<Models.ProductPackage> values;

            while (true)
            {
                try
                {
                    var Packages = from d in db.MstProductPackages
                                   select new Models.ProductPackage
                                   {
                                       Id = d.Id,
                                       ProductPackageDescription = d.ProductPackage,
                                       SKU = d.SKU,
                                       Price = d.Price,
                                       ProductId = d.ProductId,
                                       Product = d.MstProduct.Product,
                                       IsAvailable = d.IsAvailable,
                                       WithCoupon = d.WithCoupon,
                                       WithSoftware = d.WithSoftware,
                                       IsReoccuring = d.IsReoccuring,
                                       Particulars = d.Particulars,
                                       PackageURL = d.PackageURL
                                   };
                    if (Packages.Count() > 0)
                    {
                        values = Packages.ToList();
                    }
                    else
                    {
                        values = new List<Models.ProductPackage>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.ProductPackage>();
                        break;
                    }

                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }


        // POST api/AddProductPackage
        [Authorize]
        [Route("api/AddProductPackage")]
        public int Post(Models.ProductPackage value)
        {
            try
            {
                Data.MstProductPackage NewProductPackage = new Data.MstProductPackage();

                NewProductPackage.ProductPackage = value.ProductPackageDescription;
                NewProductPackage.SKU = value.SKU;
                NewProductPackage.Price = value.Price;
                NewProductPackage.ProductId = value.ProductId;
                NewProductPackage.IsAvailable = value.IsAvailable;
                NewProductPackage.WithCoupon = value.WithCoupon;
                NewProductPackage.WithSoftware = value.WithSoftware;
                NewProductPackage.IsReoccuring = value.IsReoccuring;
                NewProductPackage.Particulars = value.Particulars;
                NewProductPackage.PackageURL = value.PackageURL;

                db.MstProductPackages.InsertOnSubmit(NewProductPackage);
                db.SubmitChanges();

                return NewProductPackage.Id;
            }
            catch
            {
                return 0;
            }
        }

        // PUT /api/UpdateProductPackage/5
        [Authorize]
        [Route("api/UpdateProductPackage/{Id}")]
        public HttpResponseMessage Put(String Id, Models.ProductPackage value)
        {
            Id = Id.Replace(",", "");
            int id = Convert.ToInt32(Id);

            try
            {
                //var Sales = from d in db.TrnSales where d.Id == Id select d;
                var ProductPackage = from d in db.MstProductPackages where d.Id == id select d;

                if (ProductPackage.Any())
                {

                    var UpdatedProductPackage = ProductPackage.FirstOrDefault();

                    UpdatedProductPackage.ProductPackage = value.ProductPackageDescription;
                    UpdatedProductPackage.SKU = value.SKU;
                    UpdatedProductPackage.Price = value.Price;
                    UpdatedProductPackage.ProductId = value.ProductId;
                    UpdatedProductPackage.IsAvailable = value.IsAvailable;
                    UpdatedProductPackage.WithCoupon = value.WithCoupon;
                    UpdatedProductPackage.WithSoftware = value.WithSoftware;
                    UpdatedProductPackage.IsReoccuring = value.IsReoccuring;
                    UpdatedProductPackage.Particulars = value.Particulars;
                    UpdatedProductPackage.PackageURL = value.PackageURL;

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

        // DELETE api/DeleteProductPackage/5
        [Authorize]
        [Route("api/DeleteProductPackage/{Id}")]
        public HttpResponseMessage Delete(int Id)
        {
            Data.MstProductPackage DeleteProductPackage = db.MstProductPackages.Where(d => d.Id == Id).First();

            if (DeleteProductPackage != null)
            {
                db.MstProductPackages.DeleteOnSubmit(DeleteProductPackage);
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

        // GET api/GetProduct/1
        [Authorize]
        [Route("api/GetProduct/{Id}")]
        public List<Models.ProductPackage> GetProductInfo(int Id)
        {
            var retryCounter = 0;
            List<Models.ProductPackage> values;

            while (true)
            {
                try
                {
                    var Info = from p in db.MstProductPackages
                               where p.Id == Id
                               select new Models.ProductPackage
                               {
                                   Id = p.Id,
                                   Product = p.ProductPackage,
                                   SKU = p.SKU,
                                   Price = p.Price,
                                   ProductId = p.ProductId,
                                   IsAvailable = p.IsAvailable,
                                   WithCoupon = p.WithCoupon,
                                   WithSoftware = p.WithSoftware,
                                   IsReoccuring = p.IsReoccuring,
                                   Particulars = p.Particulars,
                                   PackageURL = p.PackageURL
                               };
                    if (Info.Count() > 0)
                    {
                        values = Info.ToList();
                    }
                    else
                    {
                        values = new List<Models.ProductPackage>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.ProductPackage>();
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                    retryCounter++;
                }
            }
            return values;
        }

        // GET api/GetPackageUsers/5
        [Authorize]
        [Route("api/GetPackageUsers/{Id}")]
        public List<Models.User> GetPackageUsers(int id)
        {
            var retryCounter = 0;
            List<Models.User> values;

            while (true)
            {
                try
                {
                    var Users = from m in db.MstUsers
                                join s in db.TrnSales
                                on m.Id equals s.UserId
                                where s.ProductPackageId == id
                                select new Models.User
                                {
                                    Id = m.Id,
                                    UserName = m.UserName,
                                    FirstName = m.FirstName,
                                    LastName = m.LastName,
                                    EmailAddress = m.EmailAddress,
                                    PhoneNumber = m.PhoneNumber,
                                    SaleId = s.Id,
                                    Price = s.Price,
                                    SalesNumber = s.SalesNumber,
                                    SalesDate = Convert.ToString(s.SalesDate.Year) + "-" + Convert.ToString(s.SalesDate.Month + 100).Substring(1, 2) + "-" + Convert.ToString(s.SalesDate.Day + 100).Substring(1, 2),
                                    RenewalDate = s.RenewalDate.ToShortDateString(),
                                    ExpiryDate = s.ExpiryDate.ToShortDateString(),
                                    Particulars = s.Particulars,
                                    Quantity = s.Quantity,
                                    Amount = s.Amount,
                                    IsActive = s.IsActive,
                                    IsRefunded = s.IsRefunded,
                                    ProductPackageId = s.ProductPackageId,
                                    ProductPackage = s.MstProductPackage.ProductPackage
                                };
                    if (Users.Count() > 0)
                    {
                        values = Users.ToList();
                    }
                    else
                    {
                        values = new List<Models.User>();
                    }
                    break;
                }
                catch
                {
                    if (retryCounter == 3)
                    {
                        values = new List<Models.User>();
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


