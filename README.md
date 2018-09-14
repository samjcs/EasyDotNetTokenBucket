# EasyDotNetTokenBucket
This is a simple implementation of a TokenBucket Algorithm used for making continuous API request to a rate limited API. It can be used for anything where a simple throttle is needed. This is a asynchrous task that will always return true.

You need to figure how many requests can be made over a timespan in seconds. For example, you can call an API up to 100 times every 10 seconds. 

The intuition is simple, you have a certain number of tokens that can be issued during a span of time. In this example we have a bucket that hold up to 100 tokens.  We use a counter that starts at 0 to represent the buckets capacity. 

Every time before we make a request to an API or need to do something that should be throttled we call GetToken() and it tries to add a token to out bucket.  The way we keep track of how many tokens are in the bucket is by incramenting the counter by 1 every time we call GetToken().

There are three things that can happen
* If our counter CurrentTokens <= MaxCapacity we add a token to the bucket and GetToken() returns.
* Our bucket is full, and the program halts until the next interval is reached. In this case every 10 seconds. If we each out bucket capacity at 8 seconds, it halts for 2 seconds. Once the interval is reached it sets the current tokens to 0, and adds a token for the request that is waiting.
* Our bucket is not full, but the current interval comes to a end. The bucket sets the capacity back to 0 and starts over.
We can use the TokenBucket like this
```
var Throttle = new TokenBucket(int numberOfRequest, int intravalInSeconds);

if (await throttle.GetToken()) {
... Call a api, run some code that need to be throttled
}
```

throttle.GetToken() will always return true, but will halt when a token is not available.

Thats all there is to it. Please feel free to submit pull request.



