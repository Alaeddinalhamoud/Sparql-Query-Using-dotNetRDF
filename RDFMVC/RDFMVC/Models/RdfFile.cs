using System;
using System.ComponentModel.DataAnnotations;

namespace RDFMVC.Models
{
    public class RdfFile
    {
        [Key]
        public int FileId { get; set; }
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        [Display(Name = "Uploded By")]
        public string UserId { get; set; }
        [Display(Name = "Make it Public")]
        public bool IsPublic { get; set; }
        [Display(Name = "Uppload Date")]
        public DateTime UploadDate { get; set; }
        [Display(Name = "Size")]
        public int Size { get; set; }
    }
}