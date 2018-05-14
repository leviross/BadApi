using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadApi.Models
{
    public class ViewModel
    {
        public IEnumerable<Tweet> Tweets { get; set; }
    }
}