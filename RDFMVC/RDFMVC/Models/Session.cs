using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }
        public string UserId { get; set; }
        public int FileId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}