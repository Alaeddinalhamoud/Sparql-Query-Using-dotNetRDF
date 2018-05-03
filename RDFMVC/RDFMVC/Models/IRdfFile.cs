using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFMVC.Models
{
  public  interface IRdfFile
    {
      void  Save(RdfFile _RdfFile);
        IEnumerable<RdfFile> RdfFileIEnum { get; }
        IQueryable<RdfFile> RdfFileIQueryable { get; }
        RdfFile Delete(int? Id);
        RdfFile Details(int? Id);
    }
}
