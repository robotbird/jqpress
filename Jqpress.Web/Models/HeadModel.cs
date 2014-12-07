using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class HeadModel:BaseModel
    {
        public HeadModel() 
        {
            NavLinks = new List<LinkInfo>();
        }
        /// <summary>
        /// 导航
        /// </summary>
        public List<LinkInfo> NavLinks { get; set; }        

    }
}