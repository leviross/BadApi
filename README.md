# BadApi

## Summary

Provides a way for users to search for all Tweets from 2016-2017. You can also search for a Tweet by Id from within that time frame.
I am leaving the magical final number of total Tweets a secret. Please clone the repo and load the project in VS and load the UI.

### Additional Features

I also timed the beginning and ending of each controller action so you can see the total time in seconds that it takes to complete 
both of the 2 operations. 

If I am incorrect in my total Tweets calculation, the app will result in an unfortunate state for the user! But if I am correct, that 
won't happen:) 

### Additional Documentation

I allowed for some extensibility in the app by adding a `TweetApi` class and allow users to pass that object into the `GetTweets` method. 
This would potentially allow users to search for a range of Tweets within custom dates. 

As you can see in the `GetTweets` method, I increment the startDate after each request to BadApi and then I deal with duplicate Tweets right away. 
I was thinking about allowing duplicate Tweets and filtering them at the end, but left this in for 2 reasons: 
1. To allow you to see how I would deal with a real-world technical challenge. 
2. Because it would take more time to have to iterate through such a large collection at the end. Its more efficient to iterate through only some of the requests. Im impressed with my total resulting time in seconds for the whole operation. 

Thanks for your time and I would love to get an interview! Look forward to hearing back from your team:) 

Levi
