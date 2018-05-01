using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class ResultSet
    {
        public IEnumerable<string> Columns { get; set; }
        public Queue<string> Rows = new Queue<string>();

    }
}