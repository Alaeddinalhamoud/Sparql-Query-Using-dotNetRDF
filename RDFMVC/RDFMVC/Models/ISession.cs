using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFMVC.Models
{
  public  interface ISession
    {
       void Save(Session _Session);
        Session SessionByUserId(string Id);
        Session SessionByFileId(int Id);
    }
}
