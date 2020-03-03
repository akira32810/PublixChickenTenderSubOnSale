using System;
using System.Collections.Generic;
using System.Text;

namespace PublixSubSale
{
    class JsonModel
    {
        public class User
        {
            public string userID { get; set; }
            public string hashedEmail { get; set; }
            public string list_id { get; set; }
            public int list_count { get; set; }
            public string user_state { get; set; }
            public string client_id { get; set; }
            public bool has_phone { get; set; }
        }

        public class Page
        {
            public string page_title { get; set; }
            public string level1 { get; set; }
            public string level2 { get; set; }
            public string level3 { get; set; }
            public string date { get; set; }
        }

        public class Products
        {
            public string id { get; set; }
            public string name { get; set; }
            public string brand { get; set; }
            public string category { get; set; }
            public string price { get; set; }
            public string sub_recipe { get; set; }
            public string variety { get; set; }
            public string allegerns { get; set; }
        }

        public class RootObject
        {
            public User user { get; set; }
            public Page page { get; set; }
            public Products products { get; set; }
        }
    }
}
