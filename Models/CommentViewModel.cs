using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EshopperMCV.Models
{
    public class CommentViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}