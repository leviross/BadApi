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
    }

    public enum ViewCategory
    {
        Index,
        Search
    }
}