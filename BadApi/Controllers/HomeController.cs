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
        private const string _defaultStartDate = "2016-01-01T00:00:00";
        private const string _defaultEndDate = "2016-12-12T23:59:59";

        public async Task<ActionResult> Index()
        {
            var tweets = await GetTweets();

            var viewModel = new ViewModel
            {
                Tweets = null,
            };

            return View("~/Views/Home/Index.cshtml", viewModel);
        }

        public async Task<List<Tweet>> GetTweets(TweetApi apiContext = null)
        {
            if (apiContext == null)
            {
                apiContext = new TweetApi
                {
                    StartDate = _defaultStartDate,
                    EndDate = _defaultEndDate
                };
            }

            var result = new List<Tweet>();

            using (var client = new HttpClient())
            {
                var uri = new Uri($"{_apiBaseUrl}?startDate={apiContext.StartDate}&endDate={apiContext.EndDate}");

                var response = await client.GetAsync(uri);
                var textResult = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<Tweet>>(textResult);
                var me = textResult;
            }

            return result; 
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