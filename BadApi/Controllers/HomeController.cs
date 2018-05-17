using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BadApi.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BadApi.Controllers
{
    public class HomeController : Controller
    {
        private const string _apiBaseUrl = "https://badapi.iqvia.io/api/v1/Tweets";
        private const string _defaultStartDate = "2016-01-01T00:00:00Z";
        private const string _defaultEndDate = "2017-12-31T23:59:59Z";
        private List<Tweet> _tweets;

        public ActionResult Index()
        {
            var viewModel = new ViewModel { Category = ViewCategory.Index };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        [Route("tweets")]
        public async Task<ActionResult> Tweets()
        {
            var reqStartTime = DateTime.Now;

            _tweets = await GetTweets();

            var reqEndTime = DateTime.Now;

            var firstCount = _tweets.Count;
            var newCount = Check(_tweets);

            var viewModel = new ViewModel
            {
                Tweets = _tweets,
                RequestSeconds = (reqEndTime - reqStartTime).Seconds
            };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> TweetsById(ViewModel model)
        {
            var reqStartTime = DateTime.Now;

            _tweets = await GetTweets();

            var reqEndTime = DateTime.Now;

            var tweetById = _tweets.Where(x => x.Id == model.Id);

            var viewModel = new ViewModel
            {
                Tweets = tweetById,
                RequestSeconds = (reqEndTime - reqStartTime).Seconds
            };
           
            return View("~/Views/Home/Index.cshtml", viewModel);
        }
        
        // Not needed, used to check a counts to make sure I was getting correct counts.
        public int Check(List<Tweet> tweets)
        {
            var dict = new Dictionary<string, int>();

            foreach (var tweet in tweets)
            {
                if (!dict.ContainsKey(tweet.Id))
                {
                    dict.Add(tweet.Id, 1);
                }
            }

            return dict.Count;
        }

        public async Task<List<Tweet>> GetTweets(TweetApi apiContext = null)
        {
            // I added for extensibility, users could potentially add start/end dates and other data.
            if (apiContext == null)
            {
                apiContext = new TweetApi
                {
                    StartDate = _defaultStartDate,
                    EndDate = _defaultEndDate,
                };
            }

            var result = new List<Tweet>();
            var prevSet = new List<Tweet>();

            using (var client = new HttpClient())
            {
                while (!apiContext.IsCompletedSearch)
                {
                    var startDate = Url.Encode(apiContext.StartDate);
                    var endDate = Url.Encode(apiContext.EndDate);
                    var apiUrl = new Uri($"{_apiBaseUrl}?startDate={startDate}&endDate={endDate}");

                    var response = await client.GetAsync(apiUrl);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    var currentSet = JsonConvert.DeserializeObject<List<Tweet>>(responseStr);

                    var firstTweetId = currentSet.FirstOrDefault()?.Id;

                    if (prevSet.Any(x => x.Id == firstTweetId) == true)
                    {
                        var prevDict = prevSet.ToDictionary(x => x.Id, x => 1);

                        for (var i = 0; i < currentSet.Count; i++)
                        {
                            if (!prevDict.ContainsKey(currentSet[i].Id))
                            {
                                currentSet = currentSet.GetRange(i, currentSet.Count - i);
                                result.AddRange(currentSet);
                                i = currentSet.Count;
                            }                            
                            else if (i == currentSet.Count - 1 && currentSet.Count == prevSet.Count)
                            {
                                apiContext.IsCompletedSearch = true;
                            }
                        }
                    }
                    else
                    {
                        result.AddRange(currentSet);
                    }

                    prevSet = currentSet;
                    var lastTweetStamp = currentSet[currentSet.Count - 1].Stamp;
                    apiContext.StartDate = lastTweetStamp;
                }
            }
                        
            return result; 
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}