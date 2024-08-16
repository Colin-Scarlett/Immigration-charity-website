using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Immigration_charity_website.Models
{
    public class Service
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Details { get; set; }
    }
}