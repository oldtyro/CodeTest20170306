using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Json;
using testAsp.Models;
using System.Data.SqlClient;
using System.Data;

namespace testAsp.Controllers
{
    public class MerchantController : Controller
    {
        // GET: Merchant
        public ActionResult getMerchantList(int page_size = 10, int page_number = 1)
        {
            try
            {
                MerchantResponse merchantResponse = getMerchantResponse(page_size, page_number);

                ViewBag.response_code = merchantResponse.response_code;
                ViewBag.merchants = merchantResponse.data;
                ViewBag.pagination = merchantResponse.pagination;
            }
            catch (Exception e)
            {
                return null;
            }

            return View();
        }


        /// <summary>
        /// get object from webapi
        /// </summary>
        /// <param name="page_size"></param>
        /// <param name="page_number"></param>
        /// <returns></returns>
        private MerchantResponse getMerchantResponse(int page_size, int page_number)
        {
            MerchantResponse merchantResponse = null;

            //get json from webapi
            string URL = "http://api.demo.muulla.com/cms/merchant/all/active/" + page_size.ToString() + "/" + page_number.ToString();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Headers.Set("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiI1NGQxOTY4MGI1MWMxNTI2MGI5NDRmZDUiLCJpc3N1ZV9kYXRlIjoiMjAxNS0wOS0wOVQwNToxMzo1My40NThaIn0.Hk2XypA_KMUnIKdSVYnwq3Rn3QyMNSQ-e80-sZsA9bY");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MerchantResponse));
                object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                merchantResponse = (MerchantResponse)objResponse;
            }

            return merchantResponse;
        }
    }
}