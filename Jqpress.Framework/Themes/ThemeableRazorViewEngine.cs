using System.Collections.Generic;
using System.Web.Mvc;

namespace Jqpress.Framework.Themes
{
    public class ThemeableRazorViewEngine : ThemeableBuildManagerViewEngine
    {
        public ThemeableRazorViewEngine()
        {
            AreaViewLocationFormats = new[]
                                          {
                                              //themes
                                              "~/Areas/{2}/StoreThemes/{3}/Views/{0}.cshtml", 
                                              "~/Areas/{2}/StoreThemes/{3}/Views/Shared/{0}.cshtml", 
                                              
                                              //default
                                              "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                              "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                          };

            AreaMasterLocationFormats = new[]
                                            {
                                                //themes
                                                "~/Areas/{2}/StoreThemes/{3}/Views/{0}.cshtml", 
                                                "~/Areas/{2}/StoreThemes/{3}/Views/Shared/{0}.cshtml", 


                                                //default
                                                "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                            };

            AreaPartialViewLocationFormats = new[]
                                                 {
                                                     //themes
                                                    "~/Areas/{2}/StoreThemes/{3}/Views/{0}.cshtml", 
                                                    "~/Areas/{2}/StoreThemes/{3}/Views/Shared/{0}.cshtml", 
                                                    
                                                    //default
                                                    "~/Areas/{2}/Views/{1}/{0}.cshtml", 
                                                    "~/Areas/{2}/Views/Shared/{0}.cshtml", 
                                                 };

            ViewLocationFormats = new[]
                                      {

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml",


                                            //Admin
                                            "~/Administration/Views/{1}/{0}.cshtml",
                                            "~/Administration/Views/Shared/{0}.cshtml",

                                             //themes
                                            "~/StoreThemes/{2}/Views/{0}.cshtml", 
                                            "~/StoreThemes/{2}/Views/Shared/{0}.cshtml",
                                      };

            MasterLocationFormats = new[]
                                        {
                                            //themes
                                            "~/StoreThemes/{2}/Views/{0}.cshtml", 
                                            "~/StoreThemes/{2}/Views/Shared/{0}.cshtml", 

                                            //default
                                            "~/Views/{1}/{0}.cshtml", 
                                            "~/Views/Shared/{0}.cshtml", 
                                        };

            PartialViewLocationFormats = new[]
                                             {
                                                 //themes
                                                "~/StoreThemes/{2}/Views/{0}.cshtml", 
                                                "~/StoreThemes/{2}/Views/Shared/{0}.cshtml", 

                                                //default
                                                "~/Views/{1}/{0}.cshtml", 
                                                "~/Views/Shared/{0}.cshtml", 

                                                //Admin
                                                "~/Administration/Views/{1}/{0}.cshtml",
                                                "~/Administration/Views/Shared/{0}.cshtml",
                                             };

            FileExtensions = new[] { "cshtml" };
        }

        protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
        {
            string layoutPath = null;
            var runViewStartPages = false;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions);
            //return new RazorView(controllerContext, partialPath, layoutPath, runViewStartPages, fileExtensions, base.ViewPageActivator);
        }

        protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
        {
            string layoutPath = masterPath;
            var runViewStartPages = true;
            IEnumerable<string> fileExtensions = base.FileExtensions;
            return new RazorView(controllerContext, viewPath, layoutPath, runViewStartPages, fileExtensions);
        }
    }
}
