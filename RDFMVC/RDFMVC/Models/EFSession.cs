using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class EFSession : ISession
    {
        EFDbContext context = new EFDbContext();
        public void Save(Session _Session)
        {
            string User="admin@admin.com";//Defualt User For Setting.

            if (_Session.UserId.Equals(""))
            {
                try { 
                //Update public Session...if user does not login(Select file,Query).
                Session _Temp_Session =  context.Sessions.FirstOrDefault(m => m.UserId.Equals(User));
                _Temp_Session.UserId = User;
                _Temp_Session.FileId = _Session.FileId;
                _Temp_Session.CreatedDate = _Session.CreatedDate;
                _Temp_Session.UpdateDate = _Session.UpdateDate;
                
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    //Insert public Session...First Time and Once only.
                    Session _Temp_Session = new Session();
                    _Temp_Session.UserId = User;
                    _Temp_Session.FileId = _Session.FileId;
                    _Temp_Session.CreatedDate = _Session.CreatedDate;
                    _Temp_Session.UpdateDate = _Session.UpdateDate;
                    context.Sessions.Add(_Temp_Session);
                }
                
            }
            else{
                
                try {
                    //Update exist User Session 
                    Session _Temp_Session =context.Sessions.FirstOrDefault(m => m.UserId.Equals(_Session.UserId));
                    _Temp_Session.UserId = _Session.UserId;
                    _Temp_Session.FileId = _Session.FileId;
                    _Temp_Session.CreatedDate = _Session.CreatedDate;
                    _Temp_Session.UpdateDate = _Session.UpdateDate;
                   
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Session _Temp_Session = new Session();
                    //First Time Attache file.
                    _Temp_Session.UserId = _Session.UserId;
                    _Temp_Session.FileId = _Session.FileId;
                    _Temp_Session.CreatedDate = _Session.CreatedDate;
                    _Temp_Session.UpdateDate = _Session.UpdateDate;
                    context.Sessions.Add(_Temp_Session);
                }

              
            }
            context.SaveChanges();
        }

        public Session SessionByFileId(int Id)
        {
            Session _Session = context.Sessions.Find(Id);
            return _Session;
        }

        //Will return only 1 Session , Becouse not allowed to add more that one .
        public Session SessionByUserId(string Id)
        {
            Session _Session = context.Sessions.FirstOrDefault(m => m.UserId.Equals(Id));
            return _Session;
        }
    }
}