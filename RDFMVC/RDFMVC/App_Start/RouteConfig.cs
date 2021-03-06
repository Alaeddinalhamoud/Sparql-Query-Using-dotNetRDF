﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RDFMVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
           // routes.IgnoreRoute("Document/Folder/{*path}.{extension}");
          routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Old Version 1.5
          /*  routes.MapRoute(
              name: "SelectFileRoute",
              url: "{controller}/{action}",
              defaults: new { controller = "UploadFile", action = "SelectFile" }
          ); */
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
           
        }
    }
}
