using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class EFDbContext: DbContext
    {
        public DbSet<RdfFile> RdfFiles { get; set; }
    }
}