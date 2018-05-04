using RDFMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml;

namespace RDFMVC.Controllers
{
    public class UploadFileController : Controller
    {
        private readonly IRdfFile _IRdfFile;

        public UploadFileController(IRdfFile _repo)
        {
            _IRdfFile = _repo;
        }
        // GET: UploadFile
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadGateway);
            }
            RdfFile model = _IRdfFile.Details(Id);
            
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpPost, ActionName("Delete")]
        [Authorize]
        public ActionResult DeleteConfirm(int? Id)
        {

            RdfFile model = _IRdfFile.Details(Id);

            if (model.UserId == User.Identity.Name)
            {
                RdfFile model_deleted = _IRdfFile.Delete(Id);

                if (model_deleted != null)
                {
                    TempData["message"] = string.Format("{0} has been deleted successfully", model.FileName);
                }
                return RedirectToAction("LoadOwlPublicFiles");
            }
            TempData["message"] = string.Format("You Can't Delete this File {0} , it is not your file.", model.FileName);
            
            return RedirectToAction("LoadOwlPublicFiles");
        }
        public ActionResult LoadOwlPublicFiles()
        {
            IEnumerable<RdfFile> _list;
            _list = _IRdfFile.RdfFileIQueryable.Where(m =>m.IsPublic==true);//Show Public OWl Only
            return PartialView(_list);
        }
        public ActionResult LoadMyOwlFiles()
        {
            IEnumerable<RdfFile> _list;
            _list = _IRdfFile.RdfFileIQueryable.Where(m => m.UserId==User.Identity.Name);//Show My OWl Only
            return PartialView(_list);
        }
        //Old Version
        //public ActionResult LoadXMLFile()
        //{

        //    List<XMLFile> _list=new List<XMLFile>();
        //    string realPath;
        //    realPath = Server.MapPath("~/DataSets/Database.xml");
        //    XElement xelement = XElement.Load(realPath);
        //    IEnumerable<XElement> nodes = xelement.Elements();

        //    foreach (var node in nodes)
        //    {
        //        XMLFile model = new XMLFile();
        //        model.filename = node.Attribute("name").Value;
        //        model.Date = node.Attribute("date").Value;
        //        _list.Add(model);
        //    } 
        //    return PartialView(_list);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
       public JsonResult Upload()
        {
            RdfFile model = new RdfFile();
            string _FileName = string.Empty;

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var file = System.Web.HttpContext.Current.Request.Files["MyFile"];
                var IsPublic = System.Web.HttpContext.Current.Request.Form["IsPublic"];//True Make it Pulic
                 
                //Read OWL only...
                var _ext = Path.GetExtension(file.FileName);
                if (_ext.Equals(".owl"))
                {

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    Random rng = new Random();
                   int rand= rng.Next(1000);

                    var _comPath = Server.MapPath("/DataSets/Id_") + rand +"_" + fileName;
                    _FileName = "Id_" + rand + "_" + fileName;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving file in Original Mode
                    file.SaveAs(path);


                        //here to add Image Path to You Database ,
                        model.FileName = _FileName;
                        model.UserId = User.Identity.Name;//Need to read it form Identity
                        model.IsPublic = Convert.ToBoolean(IsPublic);
                        model.UploadDate = DateTime.Now;
                        model.Size = (file.ContentLength/1024);
                        _IRdfFile.Save(model);
                        // SaveToXMLDB(_FileName);  //From Old Version
                        // AttachOWLFile(_FileName);//From Old Version
                    }
                }
                else
                {
                    TempData["FileType"] = string.Format("");
                   return Json("false");
                }

            }

            return Json(Convert.ToString(_FileName), JsonRequestBehavior.AllowGet);
        }
        //Old Version
        //public void SaveToXMLDB(string filename)
        //{
        //    string realPath;
        //    realPath = Server.MapPath("~/DataSets/Database.xml");
        //    XmlDocument db = new XmlDocument();
        //    db.Load(realPath);
        //    Create the node name
        //    XmlNode Files = db.SelectSingleNode("Files");
        //    XmlNode File = db.CreateNode(XmlNodeType.Element, "File", null);
        //    XmlAttribute name = db.CreateAttribute("name");
        //    XmlAttribute Date = db.CreateAttribute("date");
        //    name.Value = filename;
        //    Date.Value = DateTime.Now.ToString();
        //    File.Attributes.Append(name);
        //    File.Attributes.Append(Date);
        //    Files.AppendChild(File);
        //    db.Save(realPath);
        //}


        public ActionResult SelectFile(string id)
        {
            AttachOWLFile(id); 
            return RedirectToAction("LoadOwlPublicFiles"); 
        }

        public void AttachOWLFile(string filename)
        {
            string realPath;
            realPath = Server.MapPath("~/DataSets/AttachedDB.xml"); 
            XmlWriter writer = XmlWriter.Create(realPath);
            writer.WriteStartDocument(); 
            writer.WriteStartElement("Files");
            writer.WriteStartElement("File");
            writer.WriteAttributeString("name", filename); 
            writer.WriteEndElement();
            writer.WriteEndDocument(); 
            writer.Flush();
            writer.Close(); 
            TempData["message"] = string.Format("You Have Select the File" + " " + filename);
        }

    }
}