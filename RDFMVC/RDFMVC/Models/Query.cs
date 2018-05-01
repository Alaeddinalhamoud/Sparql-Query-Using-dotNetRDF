using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RDFMVC.Models
{
    public class Query
    {
        [Required(ErrorMessage = "Query is required")]
        [DataType(DataType.MultilineText)]
        [Display(Name ="Query")]
        [AllowHtml]
        [ConfigurationPropertyAttribute("requestValidationMode", DefaultValue = "4.7")]
        public string query { get; set; }
    }
}