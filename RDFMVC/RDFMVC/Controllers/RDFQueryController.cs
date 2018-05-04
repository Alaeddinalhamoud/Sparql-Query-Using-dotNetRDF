using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RDFMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;
using VDS.Common.Collections;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Writing;

namespace RDFMVC.Controllers
{
    public class RDFQueryController : Controller
    {

        private readonly ISession _ISession;
        private readonly IRdfFile _IRdfFile;

        public RDFQueryController(ISession _ISessionRepo, IRdfFile _IRdfFileRepo)
        {
            _ISession = _ISessionRepo;
            _IRdfFile = _IRdfFileRepo;
        }
        // GET: RDFQuery
        [HttpGet]
        public ActionResult Index()
        {
            string AtacehedFileName = GetAttachedOwlFile();

            TempData["FileName"] = string.Format("You Are Using "+AtacehedFileName);

            Query model = new Query();
            model.query = "select * where { ?subject ?predicate ?object} Limit 10";

            return View(model);
        }
         
        [HttpGet]
        [ValidateInput(false)]
        public PartialViewResult ExecuteQuery(Query data )
        {
            ResultSet _ResultSet = new ResultSet(); 
            
            SparqlResultSet _SparqlResultSet = null; 
            //string query = Request.Unvalidated["SparqlQuery"]; 
            // Create a Triple Collection with only a subject index
            BaseTripleCollection tripleCollection = new TreeIndexedTripleCollection(
                true, false, false, false, false, false, MultiDictionaryMode.AVL); 
                // Create a Graph using the customized triple collection
                //  Graph g = new Graph(tripleCollection); 
                TripleStore _TripleStore = new TripleStore(); 
            string _OwlFileName = GetAttachedOwlFile(); 
            if (!_OwlFileName.Equals(""))
            { 
                string _OWLFilePath = Server.MapPath("~/DataSets/"+ _OwlFileName); 
                _TripleStore.LoadFromFile(_OWLFilePath); 
                ISparqlDataset _ISparqlDataset = new InMemoryDataset(_TripleStore); 
                LeviathanQueryProcessor _LeviathanQueryProcessor = new LeviathanQueryProcessor(_ISparqlDataset); 
                SparqlQueryParser _SparqlQueryParser = new SparqlQueryParser(); 
            //  Then we can parse a SPARQL string into a query
            if (ModelState.IsValid)
            {
                try { 
                SparqlQuery _SparqlQuery = _SparqlQueryParser.ParseFromString(data.query);
                var results = _LeviathanQueryProcessor.ProcessQuery(_SparqlQuery);
                    _SparqlResultSet = (SparqlResultSet)results;

                        ///
                        _ResultSet.Columns= _SparqlResultSet.Variables;
                        string value = null;
                        foreach (SparqlResult _SparqlResult in _SparqlResultSet)
                        {
                            foreach (var _Col in _ResultSet.Columns)
                            {
                                INode _Node;
                                if (_SparqlResult.TryGetValue(_Col, out _Node))
                                {
                                    switch (_Node.NodeType)
                                    {
                                        case NodeType.Uri:
                                            value = ((IUriNode)_Node).Uri.AbsoluteUri;
                                            break;
                                        case NodeType.Literal:

                                            value = ((ILiteralNode)_Node).Value;
                                            break;
                                        default:
                                            Console.WriteLine("Error Node");
                                            ModelState.AddModelError("", "Error in The Node Type ");
                                            break;
                                    } 
                                }
                                else
                                {
                                    value = String.Empty;
                                }
                                //Data To Rows  
                                _ResultSet.Rows.Enqueue(value);
                            }
                        }

                        ///



                    }
                 catch (RdfParseException e)
                 {
                        //  Console.SetError(e.ToString());
                        ModelState.AddModelError("", "Error in The Syntax " + e.Message);
                        TempData["message"] = string.Format("Syntaxerror"); 
                } 
            }
            else
            {
                TempData["message"] = string.Format("EmptyQuery"); 
                }
            }
            else
            {
                TempData["message"] = string.Format("ChooseDb"); 
            } 
            return PartialView("_ExecuteQuery", _ResultSet);
        }

        public string GetAttachedOwlFile()
        {
            Session model_Session = new Session();
            RdfFile model_RdfFile = new RdfFile();

            string CurrentUser = User.Identity.Name;

            if (CurrentUser.Equals(""))
            {
                //If there is no Session(User does not login) will user the defualt user admin@admin.com
                CurrentUser = "admin@admin.com";
                model_Session = _ISession.SessionByUserId(CurrentUser);//To get the FileId from Session Table by UserId
                model_RdfFile = _IRdfFile.Details(model_Session.FileId);//To get the File Name from RdfFile Table by FileId
            }
            else {
             model_Session = _ISession.SessionByUserId(CurrentUser);//To get the FileId from Session Table by UserId
             model_RdfFile = _IRdfFile.Details(model_Session.FileId);//To get the File Name from RdfFile Table by FileId
            }
            return model_RdfFile.FileName;
        }

        //Old Verion 1.5
        //public string GetXmlDbName()
        //{

        //    string _FileName=null;
        //    string realPath;
        //    realPath = Server.MapPath("~/DataSets/AttachedDB.xml");
        //    XElement xelement = XElement.Load(realPath);
        //    IEnumerable<XElement> nodes = xelement.Elements();

        //    foreach (var node in nodes)
        //    {
        //        _FileName = node.Attribute("name").Value;

        //    }
        //    return _FileName;
        //}

    }
}