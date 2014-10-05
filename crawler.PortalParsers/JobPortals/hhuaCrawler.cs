using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;

namespace crawler.PortalParsers.rabotaua
{
    public class hhuaCrawler : BaseCrawler
    {
        public CrawlResult Crawl()
        {
            IWebCrawler crawler = InitCrawler();
            
            //http://hh.ua/search/vacancy?no_magic=true&items_on_page=100&currency_code=UAH&clusters=true&page=0    //-- ALL
            // --- UA
            Uri uriToCrawl = new Uri("http://hh.ua/search/vacancy?no_magic=true&items_on_page=100&clusters=true&currency_code=UAH&area=5&page=0"); 


            //var urlPattern=@"^http://hh\.ua/search/vacancy\?no_magic=true&items_on_page=100&currency_code=UAH&clusters=true&page=[0-9]+$"; // -- ALL
            var urlPattern = @"^http://hh\.ua/search/vacancy\?no_magic=true&items_on_page=100&clusters=true&currency_code=UAH&area=5&page=[0-9]+$"; // -- UA

            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {

                if (Regex.IsMatch(pageToCrawl.Uri.ToString(), urlPattern, RegexOptions.IgnoreCase))
                    return new CrawlDecision { Allow = true };

                return new CrawlDecision { Allow = false, Reason = "Parse only job pages" };
            });

            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            CrawlResult result = crawler.Crawl(uriToCrawl);
            return result;
        }

        private static bool isJobListPage(CrawledPage page)
        {
            if (page.HttpWebResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var findedJobsNode = page.HtmlDocument.DocumentNode.SelectSingleNode(@"//div[@class='resumesearch__result-count' and @data-qa='vacancy-serp__found']");

                if (findedJobsNode != null && findedJobsNode.InnerText.Contains("Найден") && findedJobsNode.InnerText.Contains("ваканс"))
                {
                    return true;
                }
            }
            return false;
        }


        private static void parseAndSavePageToStack(CrawledPage page)
        {

            var findedJobsTable = page.HtmlDocument.DocumentNode.SelectNodes(@"//table[@data-qa='vacancy-serp__results']/tbody/tr");

            foreach (var job in findedJobsTable)
            {
                var jobLink = job.SelectSingleNode(".//span[@class='b-marker']/a");
                if (jobLink != null)
                {
                    var jobParsed = new ParsedJob();
                    
                    jobParsed.JobName = jobLink.InnerText;
                    jobParsed.JobUrl = jobLink.Attributes["href"].Value;
                    jobParsed.WebIdJob = jobParsed.JobUrl.Split('/').Last();

                    var postedDate = job.SelectSingleNode(".//span[@class='b-vacancy-list-date' and @data-qa='vacancy-serp__vacancy-date']");

                    jobParsed.Region = job.SelectSingleNode(".//span[@class='searchresult__address']").InnerText.Split(',').First();

                    var companyName = job.SelectSingleNode(".//a[@data-qa='vacancy-serp__vacancy-employer']");
                    if (companyName != null)
                    {
                        jobParsed.ComapnyName = companyName.InnerText;
                    }
                    else
                    {
                        jobParsed.ComapnyName = "Unknown";
                    }

                    var salary=job.SelectSingleNode(".//div[@class='b-vacancy-list-salary' and @data-qa='vacancy-serp__vacancy-compensation']");
                    if (salary != null)
                    {
                        var salaryCurrency =
                            salary.SelectNodes(".//meta").First(n => n.Attributes["itemprop"].Value == "salaryCurrency")
                                .Attributes["content"].Value;

                        var baseSalary =
                            salary.SelectNodes(".//meta").First(n => n.Attributes["itemprop"].Value == "baseSalary")
                                .Attributes["content"].Value;

                        jobParsed.Salary = extractSalary(baseSalary + " " + salaryCurrency);
                    }


                    /*   jobParsed.Description = job.SelectSingleNode(".//div[@class='d']").InnerText;

                    var categories = job.SelectNodes(".//div[@class='tags']/a");
                    /                    jobParsed.Categories = new List<string>();
                    if (categories != null)
                    {
                        foreach (var category in categories)
                        {
                            jobParsed.Categories.Add(category.InnerText);
                        }
                    }*/
                    jobParsed.Portal = "hh.ua";
                    ParsedJobStack.Instance.Push(jobParsed);
                }
            }
        }

        #region Crawler Events
        private static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            var page = e.CrawledPage;
            if (isJobListPage(page))
            {
                parseAndSavePageToStack(page);
            }
        }

        private static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e) { }


        private static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e) { }


        private static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e) { }


        #endregion


    }
}
