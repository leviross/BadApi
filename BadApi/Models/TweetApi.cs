using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BadApi.Models
{
    public class TweetApi
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsCompletedSearch { get; set; }
    }
}