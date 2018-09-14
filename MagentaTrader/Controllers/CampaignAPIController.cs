﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MagentaTrader.Controllers
{
    public class CampaignAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        [Authorize]
        [Route("api/GetInformationForMarriottTampa2018/{name}/{email}/{phone}")]
        public long GetInformationForMarriottTampa2018(string name, string email, string phone)
        {
            try
            {
                Data.TmpWorkshop workshopInfo = new Data.TmpWorkshop();

                workshopInfo.Name = name;
                workshopInfo.Email = email;
                workshopInfo.Phone = phone;
                workshopInfo.DateEncoded = DateTime.Today;
                workshopInfo.Workshop = "Marriott Tampa 2018";

                db.TmpWorkshops.InsertOnSubmit(workshopInfo);
                db.SubmitChanges();

                return workshopInfo.Id;
            }
            catch(Exception e)
            {

                return 0;
            }
        }

        [Authorize]
        [Route("api/GetCampaignEmails/{workshop}")]
        public List<CampaignEmails> GetCampaignEmails(string workshop)
        {
            var emails = from d in db.TmpWorkshops
                         where d.Workshop == workshop
                         select new CampaignEmails
                         {
                             Name = d.Name,
                             Email = d.Email
                         };

            if (emails.Any())
            {
                return emails.ToList();
            }
            else
            {
                return new List<CampaignEmails>();
            }
        }
    }

    public class CampaignEmails 
    {
        public String Name {get; set;}
        public String Email {get; set;}
    }
}
