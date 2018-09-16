using System;
using System.Collections.Generic;
using System.Text;

namespace ZibaobaoLib.Data
{
    public class BaobaoUser
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }

    }
}
