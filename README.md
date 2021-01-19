# CrawlerVNEXPRESS
## Feature
Tool to crawl data from news site vnexpress.net. Extracting data from HTML. Get the link of the latest news, from these links continue to each link to get the Subject, Title, Link, the content of the news (including the content structure) then save it to the database. The application will be configured to run on the server every 60 minutes.

The data will be served by the API app ([TodoAPI](https://github.com/rambothanh/TodoApi)). And the client app ([VNexpressClient](https://github.com/rambothanh/VnexpressClient)) will call the API to get the data and show it to the end user

## Technology
#### .Net 5
#### HtmlAgilityPack, Xpath, Css Selector
#### EntityFrameworkCore
## Console output
![result](https://user-images.githubusercontent.com/28246617/103145089-f0f8b700-4766-11eb-8331-69c470f08177.png)
