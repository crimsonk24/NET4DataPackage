using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Configuration;

namespace Data {
    public class Test : DB {
        public int id { get; set; }
        public string name { get; set; }

        // init -- called before constructor is processed
        private void init() {
            // init the base vars
            this.table = "tblContent";
            this.primaryKeyField = "id";
        }

        // main fill func -- populates obj values
        private void fill(DataRow r) {
            // fill the local vars
            this.id = (int)r["id"];
            this.name = (string)r["PageName"];            
        }

        // example constructor handlers
        public Test()           : base(ConfigurationManager.ConnectionStrings["connection"].ConnectionString) { init();             }
        public Test(DataRow r)  : base(ConfigurationManager.ConnectionStrings["connection"].ConnectionString) { init(); fill(r);    }
        public Test(int id)     : base(ConfigurationManager.ConnectionStrings["connection"].ConnectionString) { 
            init();
            DataRow dr = this.GetRecordByKey(this.table, this.primaryKeyField, id.ToString());
            fill(dr);            
        }
    }
}