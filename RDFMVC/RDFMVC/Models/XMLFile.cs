using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class XMLFile
    {
        [Display(Name = "File Name")]
        public string filename { set; get; }
        [Display(Name = "Upload Date")]
        public string Date { get; set; }
    }
}