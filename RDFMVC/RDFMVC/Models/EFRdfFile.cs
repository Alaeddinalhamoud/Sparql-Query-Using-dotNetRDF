using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RDFMVC.Models
{
    public class EFRdfFile:IRdfFile
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<RdfFile> RdfFileIEnum => context.RdfFiles;

        public IQueryable<RdfFile> RdfFileIQueryable => context.RdfFiles.AsQueryable();

        public RdfFile Delete(int? Id)
        {
            RdfFile _RdfFile = context.RdfFiles.Find(Id);
            if(_RdfFile != null)
            {
                context.RdfFiles.Remove(_RdfFile);
                context.SaveChanges();
            }
            return _RdfFile;
        }

        public RdfFile Details(int? Id)
        {
            RdfFile _RdfFile = context.RdfFiles.Find(Id);
            return _RdfFile;
        }

        public void Save(RdfFile _RdfFile)
        {
            if (_RdfFile.FileId == 0)
            {
                context.RdfFiles.Add(_RdfFile);
            }
            else
            {
                RdfFile _New_RdfFile = context.RdfFiles.Find(_RdfFile.FileId);
                if(_New_RdfFile != null)
                {
                    _New_RdfFile.FileName = _RdfFile.FileName;
                    _New_RdfFile.UserId = _RdfFile.UserId;
                    _New_RdfFile.IsPublic = _RdfFile.IsPublic;
                    _New_RdfFile.Size = _RdfFile.Size;
                    _New_RdfFile.UploadDate = _RdfFile.UploadDate;
                    context.RdfFiles.Add(_New_RdfFile);
                }
            }
            context.SaveChanges();
        }
    }
}