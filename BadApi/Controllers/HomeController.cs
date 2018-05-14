using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BadApi.Models;
using System.Web.Mvc;
using System.Web;
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
        // 2018-05-14T18:34:28Z
        private const int _defaultCount = 100;

        public async Task<ActionResult> Index()
        {
            var me = DateTime.Parse("2016-01-01T00:00:00");

            var tweets = await GetTweets();

            var firstCount = tweets.Count;
            var newCount = Check(tweets);

            var viewModel = new ViewModel
            {
                Tweets = tweets,
            };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }

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
            if (apiContext == null)
            {
                apiContext = new TweetApi
                {
                    StartDate = _defaultStartDate,
                    EndDate = _defaultEndDate,
                    Count = _defaultCount
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
                    // 2016-01-07T10%3A06%3A52.5260237%2B00%3A00
                    var apiUrl = new Uri($"{_apiBaseUrl}?startDate={startDate}&endDate={endDate}");


                    // apiUrl = new Uri("https://badapi.iqvia.io/api/v1/Tweets?startDate=2016-01-07T10%3A06%3A52.5260237%2B00%3A00&endDate=2017-12-31T23%3A59%3A59");

                    var response = await client.GetAsync(apiUrl);
                    var responseStr = await response.Content.ReadAsStringAsync();

                    var currentSet = JsonConvert.DeserializeObject<List<Tweet>>(responseStr);

                    //if (currentSet.Any(x => x.Id == "976927357935194121"))
                    //{
                    //    var stop = true;
                    //    result = result.Distinct().ToList();
                    //}

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



                    // result.AddRange(currentSet);

                    if (result.Count == 14000)
                    {
                        apiContext.IsCompletedSearch = true;
                    }

                    prevSet = currentSet;
                    var lastTweetStamp = currentSet[currentSet.Count - 1].Stamp;
                    apiContext.StartDate = lastTweetStamp;

                    // We have to deal with duplicates right away. The 2nd call to the Api could already have duplicates that we got in the first 100 tweets. We have to check the id of the first tweet from every round and see if it exists in our results. 
                    // If it does => then we have to remove duplicates from the new results set by only adding from the non-duplicate id and onwards. 
                    // If it does not => then we simply add all the new results into our results

                    
                }
            }
            
            
            return result; 
        }

        public bool IsLaterThan()
        {
            var dateTime = DateTime.Parse(_defaultStartDate);
            // 635872032000000000
            return true;
        }

        [Route("api/v1/tweets/")]
        public async Task<List<Tweet>> TweetApi(TweetApi apiContext)
        {
            var tweets = await GetTweets(apiContext);

            return tweets;
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