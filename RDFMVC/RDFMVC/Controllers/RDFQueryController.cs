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
        // GET: RDFQuery
        [HttpGet]
        public ActionResult Index()
        {
            string AtacehedFileName = GetXmlDbName();

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
            string _XmlDbFileName = GetXmlDbName(); 
            if (!_XmlDbFileName.Equals(""))
            { 
                string _OWLFilePath = Server.MapPath("~/DataSets/"+_XmlDbFileName); 
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

        public string GetXmlDbName()
        {

            string _FileName=null;
            string realPath;
            realPath = Server.MapPath("~/DataSets/AttachedDB.xml");
            XElement xelement = XElement.Load(realPath);
            IEnumerable<XElement> nodes = xelement.Elements();

            foreach (var node in nodes)
            {
                _FileName = node.Attribute("name").Value;
                
            }
            return _FileName;
        }
        
        //[WebMethod]
            //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
            //public JsonResult GetQueryDataJSON()
            //{

            //    // Create a Triple Collection with only a subject index
            //    BaseTripleCollection tripleCollection = new TreeIndexedTripleCollection(
            //        true, false, false, false, false, false, MultiDictionaryMode.AVL);

            //    // Create a Graph using the customized triple collection
            //    //  Graph g = new Graph(tripleCollection);

            //    TripleStore store = new TripleStore();
            //    string realPath;
            //    realPath = Server.MapPath("~/DataSets/My.owl");
            //    store.LoadFromFile(realPath);

            //    ISparqlDataset ds = new InMemoryDataset(store);

            //    LeviathanQueryProcessor processor = new LeviathanQueryProcessor(ds);


            //    SparqlQueryParser parser = new SparqlQueryParser();



            //    //  Then we can parse a SPARQL string into a query

            //    SparqlQuery q = parser.ParseFromString("PREFIX db: <http://www.co7216coursework1.com/Group4OnlineShopping.owl#> \n" +

            //                                                "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#> \n" +

            //                                                "SELECT  ?Warehouse ?warehouseName ?history \n" +

            //                                               "WHERE{ \n" +

            //                                               "?Warehouse a   db:Warehouse; \n" +

            //                                               "  db:WarehouseName ?warehouseName; \n" +

            //                                               "  db:WarehouseHasOrderHistory ?history. \n" +

            //                                               " }");



            //    // First we need an instance of the SparqlQueryParser




            //    var results = processor.ProcessQuery(q);



            //    SparqlResultSet rset = (SparqlResultSet)results;

            //    List<object> a = rset.Results.ToList<object>();

            //    List<SparqlResult> list = rset.ToList<SparqlResult>();


            //  //  //  string output = new JavaScriptSerializer().Serialize(list);
            //  //  var pe = JsonConvert.SerializeObject(list,
            //  //                        Formatting.None,
            //  //                    new JsonSerializerSettings()
            //  //                     {
            //  //ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //  //               });


            //  //  JObject o = JObject.Parse(pe);
            //  //  Response.ContentType = "application/json; charset=utf-8";
            //  //  Response.AddHeader("Content-Disposition", "inline;filename=\"applications.json\"");
            //  //  Response.Write(o);

            //    IEnumerable<string> _Variables = rset.Variables;//To get the var (*)
            //    string data;
            //    List<SparqlResult> res = new List<SparqlResult>();

            //    foreach (SparqlResult result in rset)
            //    {

            //        foreach (var _Variable in _Variables)
            //        { 
            //            INode n;

            //            if (result.TryGetValue(_Variable, out n))
            //            {
            //                switch (n.NodeType)
            //                {
            //                    case NodeType.Uri:
            //                        data = ((IUriNode)n).Uri.AbsoluteUri;
            //                        break;
            //                    //case NodeType.Blank:
            //                    //    data = ((IBlankNode)n).Label;
            //                    //    break;
            //                    case NodeType.Literal:
            //                        //You may want to inspect the DataType and Language properties and generate
            //                        //a different string here
            //                        data = ((ILiteralNode)n).Value;
            //                        break;
            //                    default:
            //                        throw new RdfOutputException("Unexpected Node Type");
            //                }

            //            }
            //            else
            //            {
            //                data = String.Empty;
            //            }


            //                //Do what you want with the extracted string
            //           // res.Add(obj);


            //        }//End Of getting data

            //    } //End of Vars

            //    ////Now save this to disk as SPARQL JSON
            //    //SparqlJsonWriter writer = new SparqlJsonWriter();
            //    //writer.Save(rset, Server.MapPath("~/DataSets/example.srj"));

            //    var json = JsonConvert.SerializeObject(a,
            //        Formatting.Indented,
            //        new JsonSerializerSettings()
            //        {
            //            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize
            //        });
            //    //var logJson = JsonConvert.DeserializeObject<object>(json);
            //    //JavaScriptSerializer js = new JavaScriptSerializer();
            //    //var json = js.Serialize(list);

            //    JToken jt = JToken.Parse(json);
            //    string formatted = jt.ToString(Newtonsoft.Json.Formatting.Indented);


            //    //// return chartData;
            //    return Json(formatted, JsonRequestBehavior.AllowGet);
            //}
        }
}