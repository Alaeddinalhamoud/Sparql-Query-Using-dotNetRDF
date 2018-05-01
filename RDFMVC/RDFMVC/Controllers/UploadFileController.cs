using RDFMVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace RDFMVC.Controllers
{
    public class UploadFileController : Controller
    {
        // GET: UploadFile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadXMLFile()
        {
           
            List<XMLFile> _list=new List<XMLFile>();
            string realPath;
            realPath = Server.MapPath("~/DataSets/Database.xml");
            XElement xelement = XElement.Load(realPath);
            IEnumerable<XElement> nodes = xelement.Elements();
         
            foreach (var node in nodes)
            {
                XMLFile model = new XMLFile();
                model.filename = node.Attribute("name").Value;
                model.Date = node.Attribute("date").Value;
                _list.Add(model);
            }
           
            return PartialView(_list);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Upload()
        {
            string _FileName = string.Empty;

            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var file = System.Web.HttpContext.Current.Request.Files["MyFile"];

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
                    SaveToXMLDB(_FileName);
                    AttachOWLFile(_FileName);
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

        public void SaveToXMLDB(string filename)
        {
            string realPath;
            realPath = Server.MapPath("~/DataSets/Database.xml");
            XmlDocument db = new XmlDocument();
            db.Load(realPath);
            //Create the node name 
            XmlNode Files = db.SelectSingleNode("Files");
            XmlNode File = db.CreateNode(XmlNodeType.Element, "File", null);
            XmlAttribute name = db.CreateAttribute("name");
            XmlAttribute Date = db.CreateAttribute("date");
            name.Value = filename;
            Date.Value =DateTime.Now.ToString();
            File.Attributes.Append(name);
            File.Attributes.Append(Date);
            Files.AppendChild(File);
            db.Save(realPath);
        }

            
      public ActionResult SelectFile(string id)
        {
            AttachOWLFile(id); 
            return RedirectToAction("LoadXMLFile"); 
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
            TempData["FileSelected"] = string.Format("You Have Select the File" + " " + filename);
        }

    }
}