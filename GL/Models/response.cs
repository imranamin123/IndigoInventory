using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GL.Models
{
    public class response
    {
        public bool status { get; set; }
        public Int64 id { get; set; }
        public int HttpStatusCode { get; set; }
        public string resMessage { get; set; }
        public object resObj { get; set; }
        public List<object> resObjList { get; set; }
    }
}