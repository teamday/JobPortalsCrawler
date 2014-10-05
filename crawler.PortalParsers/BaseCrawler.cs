using Abot.Crawler;
using Abot.Poco;


namespace crawler.PortalParsers
{
   
    public abstract class BaseCrawler
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static IWebCrawler InitCrawler()
        {
            IWebCrawler crawler = GetCustomBehaviorUsingLambdaWebCrawler();
            return crawler;
        }

        private static IWebCrawler GetManuallyConfiguredWebCrawler()
        {
            //Create a config object manually
            var config = new CrawlConfiguration
            {
                CrawlTimeoutSeconds = 0,
                DownloadableContentTypes = "text/html, text/plain",
                IsExternalPageCrawlingEnabled = false,
                IsExternalPageLinksCrawlingEnabled = false,
                IsRespectRobotsDotTextEnabled = false,
                IsUriRecrawlingEnabled = false,
                MaxConcurrentThreads = 10,
                MaxPagesToCrawl = 0,
                MaxPagesToCrawlPerDomain = 0,
                MaxCrawlDepth = 10000,
                MinCrawlDelayPerDomainMilliSeconds = 10,
                UserAgentString = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0"
            };
            return new PoliteWebCrawler(config, null, null, null, null, null, null, null, null);
        }

        private static IWebCrawler GetCustomBehaviorUsingLambdaWebCrawler()
        {
            IWebCrawler crawler = GetManuallyConfiguredWebCrawler();         

            crawler.ShouldDownloadPageContent((crawledPage, crawlContext) => new CrawlDecision { Allow = true });

            crawler.ShouldCrawlPageLinks((crawledPage, crawlContext) =>
            {
                if (!crawledPage.IsInternal)
                    return new CrawlDecision { Allow = false, Reason = "We dont crawl links of external pages" };
                return new CrawlDecision { Allow = true };
            });

            return crawler;
        }

        protected static decimal? extractSalary(string salary)
        {
            var RURExchange = (decimal)0.35;
            var BYRExchange = (decimal)0.0012;
            var KZTExchange = (decimal)0.071;
            var USDExchange = (decimal)12.95;
            var EURExchange = (decimal)16.20;
            if (!string.IsNullOrWhiteSpace(salary))
            {
                var sl = salary.Split(' ');
                if (sl.Length > 1)
                {
                    if (sl[1].Contains("грн") || sl[1].Contains("UAH"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", ""));
                    }
                    else if (sl[1].Contains("RUR"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", ""))*RURExchange;
                    }
                    else if (sl[1].Contains("BYR"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", "")) * BYRExchange;
                    }
                    else if (sl[1].Contains("KZT"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", "")) * KZTExchange;
                    }
                    else if (sl[1].Contains("USD"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", "")) * USDExchange;
                    }
                    else if (sl[1].Contains("EUR"))
                    {
                        return decimal.Parse(sl[0].Replace(" ", "")) * EURExchange;
                    }
                    else
                    {
                        Log.Warn(string.Format("Unsupported currency {0}", sl[1]));
                    }
                }
            }
            return null;
        }
    }
}
