using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadApi.Models
{
    public class ViewModel
    {
        public IEnumerable<Tweet> Tweets { get; set; }
        public ViewCategory Category { get; set; }
        public string Id { get; set; }
        public int RequestSeconds { get; set; }
    }

    // Not needed, but could maintain state if app is built out.
    public enum ViewCategory
    {
        Index,
        Search
    }
}