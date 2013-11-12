using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Framework.Mvc;


namespace Jqpress.Web.Models
{
    public class BaseModel
    {
        public string Css { get; set; }

        public string ThemeName { get; set; }

        public class PostPageList : BasePageableModel { };

    }
}