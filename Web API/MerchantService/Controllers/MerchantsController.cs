using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using MerchantDataAccess;
using Newtonsoft.Json.Linq;

namespace MerchantService.Controllers
{
    [RoutePrefix("api/Merchants")]
    public class MerchantsController : ApiController
    {
        #region webapi
        public HttpResponseMessage Get()
        {
            return Get("active", 10, 1);
        }

        [Route("{status}/{page_size}/{page_number}")]
        public HttpResponseMessage Get(string status, int page_size, int page_number)
        {
            try
            {
                using (MerchantEntities entities = new MerchantEntities())
                {
                    IEnumerable<Merchant> iMerchants;
                    List<Merchant> merchants;

                    //get merchant records by condition
                    if (status == "active")
                    {
                        iMerchants = (entities.Merchants.Where(m => m.status == 0));
                    }
                    else
                    {
                        iMerchants = entities.Merchants;
                    }

                    //pagination
                    int total_records = iMerchants.Count();
                    int total_pages = (int)(Math.Ceiling((double)total_records / (double)page_size));
                    if (page_number > total_pages) page_number = total_pages;
                    merchants = iMerchants.Skip((page_number - 1) * page_size).Take(page_size).ToList();

                    //convert to json
                    JArray jos = new JArray();
                    foreach (Merchant merchant in merchants)
                    {
                        JObject joMerchant = convertMerchantToJson(merchant);
                        jos.Add(joMerchant);
                    }
                    JObject joMerchants = new JObject(new JProperty("response_code", 0));
                    joMerchants.Add(new JProperty("data", jos));
                    joMerchants.Add(new JProperty("pagination", setPaginationJson(page_size, page_number, total_records, total_pages)));

                    return Request.CreateResponse(HttpStatusCode.OK, joMerchants);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id}")]
        public HttpResponseMessage Get(Guid id)
        {
            try
            {
                using (MerchantEntities entities = new MerchantEntities())
                {
                    Merchant merchant = entities.Merchants.FirstOrDefault(m => m.id == id);

                    if (merchant == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Merchant with id = " + id.ToString() + " not found");

                    //convert to json
                    JObject jo = convertMerchantToJson(merchant);

                    return Request.CreateResponse(HttpStatusCode.OK, jo);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public HttpResponseMessage Post([FromBody] JObject jMerchant)
        {
            try
            {
                //convert to object from json
                Merchant merchant = convertJsonToMerchant(jMerchant);
                Address address = convertJsonToAddress(jMerchant["address"], merchant.id);
                Logo logo = convertJsonToLogo(jMerchant["logo"], merchant.id);

                //operate db
                using (MerchantEntities entities = new MerchantEntities())
                {
                    entities.Merchants.Add(merchant);
                    entities.Addresses.Add(address);
                    entities.Logoes.Add(logo);

                    entities.SaveChanges();
                }

                //return
                var message = Request.CreateResponse(HttpStatusCode.Created, convertMerchantToJson(merchant));
                message.Headers.Location = new Uri(Request.RequestUri + merchant.id.ToString());

                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id}")]
        public HttpResponseMessage Put(Guid id, [FromBody] JObject jMerchant)
        {
            try
            {
                Merchant merchant;
                using (MerchantEntities entities = new MerchantEntities())
                {
                    //get object by id
                    merchant = entities.Merchants.FirstOrDefault(m => m.id == id);
                    if (merchant == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Merchant with id = " + id.ToString() + " not found to update!");
                    Address address = merchant.Addresses.FirstOrDefault();
                    Logo logo = merchant.Logoes.FirstOrDefault();

                    //update the object by json
                    merchant = convertJsonToMerchant(jMerchant, merchant);
                    address = convertJsonToAddress(jMerchant["address"], address);
                    logo = convertJsonToLogo(jMerchant["logo"], logo);

                    //operate db
                    entities.SaveChanges();
                }

                var message = Request.CreateResponse(HttpStatusCode.OK, convertMerchantToJson(merchant));

                return message;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [Route("{id}")]
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                using (MerchantEntities entities = new MerchantEntities())
                {
                    //get the object 
                    Merchant merchant = entities.Merchants.FirstOrDefault(m => m.id == id);
                    if (merchant == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Merchant with id = " + id.ToString() + " not found to delete!");

                    //operator db
                    entities.Addresses.Remove(entities.Addresses.FirstOrDefault(a => a.merchantid == id));
                    entities.Logoes.Remove(entities.Logoes.FirstOrDefault(l => l.merchantid == id));
                    entities.Merchants.Remove(merchant);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        #endregion

        #region operate json
        /// <summary>
        /// convert merchant to json when query
        /// </summary>
        /// <param name="merchant"></param>
        /// <returns></returns>
        private JObject convertMerchantToJson(Merchant merchant)
        {
            Address address = merchant.Addresses.FirstOrDefault();
            Logo logo = merchant.Logoes.FirstOrDefault();

            JObject joAddress = convertAddressToJson(address);
            JObject joLogo = convertLogoToJson(logo);

            JObject jo = new JObject(
                new JProperty("_id", merchant.id),
                new JProperty("email", merchant.email),
                new JProperty("phone", merchant.phone),
                new JProperty("display_name", merchant.display_name),
                new JProperty("registered_name", merchant.registered_name),
                new JProperty("__v", merchant.v),
                new JProperty("address", joAddress),
                new JProperty("date_modified", merchant.date_modified),
                new JProperty("date_created", merchant.date_created),
                new JProperty("status", merchant.status == 0 ? "active" : "inactive"),
                new JProperty("logo", joLogo)
                );
            return jo;
        }

        /// <summary>
        /// convert address to json when query
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private JObject convertAddressToJson(Address address)
        {
            JObject jo = new JObject(
                new JProperty("country", address.country),
                new JProperty("state", address.state),
                new JProperty("postcode", address.postcode),
                new JProperty("suburb", address.suburb),
                new JProperty("address1", address.address1)
                );

            if (address.address2 != null)
            {
                jo.Add("address2", address.address2);
            }

            return jo;
        }

        /// <summary>
        /// convert logo to json when query
        /// </summary>
        /// <param name="logo"></param>
        /// <returns></returns>
        private JObject convertLogoToJson(Logo logo)
        {
            JObject jo = new JObject(
                new JProperty("_id", logo.id),
                new JProperty("status", logo.status == 0? "active": "inactive"),
                new JProperty("path_to_file", logo.path_to_file)
                );

            if (logo.date_created != null)
            {
                jo.Add("date_created", logo.date_created);
            }
            
            return jo;
        }

        /// <summary>
        /// set json's pagination attribute
        /// </summary>
        /// <param name="page_size"></param>
        /// <param name="page_number"></param>
        /// <param name="total_records"></param>
        /// <param name="total_pages"></param>
        /// <returns></returns>
        private JObject setPaginationJson(int page_size, int page_number, int total_records, int total_pages)
        {
            JObject jo = new JObject(
                new JProperty("page_size", page_size),
                new JProperty("page_number", page_number),
                new JProperty("total_records", total_records),
                new JProperty("total_pages", total_pages)
                );
            return jo;
        }

        /// <summary>
        /// convert json to merchant when add
        /// </summary>
        /// <param name="jMerchant"></param>
        /// <returns></returns>
        private Merchant convertJsonToMerchant(JObject jMerchant)
        {
            Merchant merchant = new Merchant();
            merchant.id = Guid.NewGuid();

            if (jMerchant["email"] == null)
                throw new Exception("please input email!");
            else
                merchant.email = jMerchant["email"].ToString();

            if (jMerchant["phone"] == null)
                throw new Exception("please input phone!");
            else
                merchant.phone = jMerchant["phone"].ToString();

            if (jMerchant["display_name"] == null)
                throw new Exception("please input display_name!");
            else
                merchant.display_name = jMerchant["display_name"].ToString();

            if (jMerchant["registered_name"] == null)
                throw new Exception("please input registered_name!");
            else
                merchant.registered_name = jMerchant["registered_name"].ToString();

            if (jMerchant["__v"] == null)
                throw new Exception("please input __v!");
            else
                merchant.v = int.Parse(jMerchant["__v"].ToString());

            merchant.date_modified = null;
            merchant.date_created = DateTime.Now;

            if (jMerchant["status"] == null)
                throw new Exception("please input status!");
            else
                merchant.status = jMerchant["status"].ToString() == "active" ? 0 : 1;

            return merchant;
        }

        /// <summary>
        /// convert json to address when add
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        private Address convertJsonToAddress(JToken jToken, Guid merchantID)
        {
            Address address = new Address();
            address.id = Guid.NewGuid();
            address.merchantid = merchantID;

            if (jToken["country"] == null)
                throw new Exception("please input country!");
            else
                address.country = jToken["country"].ToString();

            if (jToken["state"] == null)
                throw new Exception("please input state!");
            else
                address.state = jToken["state"].ToString();

            if (jToken["postcode"] == null)
                throw new Exception("please input postcode!");
            else
                address.postcode = jToken["postcode"].ToString();

            if (jToken["suburb"] == null)
                throw new Exception("please input suburb!");
            else
                address.suburb = jToken["suburb"].ToString();

            if (jToken["address1"] == null)
                throw new Exception("please input address1!");
            else
                address.address1 = jToken["address1"].ToString();

            if (jToken["address2"] != null)
                address.address2 = jToken["address2"].ToString();
            else
                address.address2 = null;

            return address;
        }
        
        /// <summary>
        /// convert json to logo when add
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        private Logo convertJsonToLogo(JToken jToken, Guid merchantID)
        {
            Logo logo = new Logo();
            logo.id = Guid.NewGuid();
            logo.merchantid = merchantID;
            if (jToken["status"] == null)
                throw new Exception("please input status!");
            else
                logo.status = jToken["status"].ToString() == "active" ? 0 : 1;

            if (jToken["path_to_file"] == null)
                throw new Exception("please input path_to_file!");
            else
                logo.path_to_file = jToken["path_to_file"].ToString();

            logo.date_created = DateTime.Now;

            return logo;
        }

        /// <summary>
        /// convert json to merchant where update
        /// </summary>
        /// <param name="jMerchant"></param>
        /// <param name="merchant"></param>
        /// <returns></returns>
        private Merchant convertJsonToMerchant(JObject jMerchant, Merchant merchant)
        {
            if (jMerchant["email"] != null)
                merchant.email = jMerchant["email"].ToString();

            if (jMerchant["phone"] != null)
                merchant.phone = jMerchant["phone"].ToString();

            if (jMerchant["display_name"] != null)
                merchant.display_name = jMerchant["display_name"].ToString();

            if (jMerchant["registered_name"] != null)
                merchant.registered_name = jMerchant["registered_name"].ToString();

            if (jMerchant["__v"] != null)
                merchant.v = int.Parse(jMerchant["__v"].ToString());

            merchant.date_modified = DateTime.Now;

            if (jMerchant["status"] != null)
                merchant.status = jMerchant["status"].ToString() == "active" ? 0 : 1;

            return merchant;
        }

        /// <summary>
        /// convert json to address when update
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        private Address convertJsonToAddress(JToken jToken, Address address)
        {
            if (jToken == null) return address;

            if (jToken["country"] != null)
                address.country = jToken["country"].ToString();

            if (jToken["state"] != null)
                address.state = jToken["state"].ToString();

            if (jToken["postcode"] != null)
                address.postcode = jToken["postcode"].ToString();

            if (jToken["suburb"] != null)
                address.suburb = jToken["suburb"].ToString();

            if (jToken["address1"] != null)
                address.address1 = jToken["address1"].ToString();

            if (jToken["address2"] != null)
                address.address2 = jToken["address2"].ToString();

            return address;
        }

        /// <summary>
        /// convert json to logo when update
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="logo"></param>
        /// <returns></returns>
        private Logo convertJsonToLogo(JToken jToken, Logo logo)
        {
            if (jToken == null) return logo;

            if (jToken["status"] != null)
                logo.status = jToken["status"].ToString() == "active" ? 0 : 1;

            if (jToken["path_to_file"] != null)
                logo.path_to_file = jToken["path_to_file"].ToString();

            return logo;
        }
        #endregion
    }
}
