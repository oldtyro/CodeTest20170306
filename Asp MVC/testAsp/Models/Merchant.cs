using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testAsp.Models
{
    public class MerchantResponse
    {
        public string response_code { get; set; }
        public Merchant[] data { get; set; }
        public Pagination pagination { get; set; }
    }

    public class Pagination
    {
        public int page_size { get; set; }
        public int page_number { get; set; }
        public int total_records { get; set; }
        public int total_pages { get; set; }
    }

    public class Merchant
    {
        public string _id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int __v { get; set; }
        public string display_name { get; set; }
        public string registered_name { get; set; }
        public string web_url { get; set; }
        public int merchant_id { get; set; }
        public Address address { get; set; }
        public string date_modified { get; set; }
        public string date_created { get; set; }
        public string status { get; set; }
        public Logo logo { get; set; }
    }

    public class Address
    {
        public string country { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string suburb { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
    }

    public class Logo
    {
        public string _id { get; set; }
        public string status { get; set; }
        public string path_to_file { get; set; }
        public string date_created { get; set; }
    }
}