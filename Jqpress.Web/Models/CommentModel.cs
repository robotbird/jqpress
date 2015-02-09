using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class CommentModel
    {
        public CommentModel()
        {
            Comments = new List<CommentInfo>();
        }

        public List<CommentInfo> Comments { get; set; }

        public int EnableVerifyCode { get; set; }

        public int PostId { get; set; }
    }
}