using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.PortalParsers.rabotaua
{
    public class RabotaUACrawler:BaseCrawler
    {       

        public CrawlResult Crawl()
        {
            IWebCrawler crawler = InitCrawler();

            Uri uriToCrawl = new Uri("http://rabota.ua/jobsearch/vacancy_list"); //http://rabota.ua/jobsearch/vacancy_list?pg=1000

            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                if (pageToCrawl.Uri.AbsoluteUri.Contains(@"rabota.ua/jobsearch/vacancy_list")
                    && !pageToCrawl.Uri.AbsoluteUri.Contains(@"period"))
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
                var findedJobsNode = page.HtmlDocument.DocumentNode.SelectSingleNode(@"//span[@id='centerZone_vacancyList_ltCount']");

                if (findedJobsNode != null && findedJobsNode.InnerText.Contains("Найден") && findedJobsNode.InnerText.Contains("ваканс"))
                {
                    return true;
                }
            }
            return false;
        }


        private static void parseAndSavePageToStack(CrawledPage page)
        {
            var findedJobsTable = page.HtmlDocument.DocumentNode.SelectNodes(@"//table[@id='centerZone_vacancyList_gridList']/tr");           

                foreach (var job in findedJobsTable)
                {
                    if (job.Attributes["id"] != null)
                    {
                        ParsedJob jobParsed = new ParsedJob();

                        jobParsed.WebIdJob = job.Attributes["id"].Value;

                        var linkJob = job.SelectSingleNode(".//div[@class='rua-g-clearfix']/a");
                        jobParsed.JobName = linkJob.InnerText;
                        jobParsed.JobUrl = linkJob.Attributes["href"].Value;

                        var JobInfo = job.SelectSingleNode(".//div[@class='rua-g-clearfix']/div[@class='s']").InnerText.Replace("\n", "").Replace("\r", "").Replace("\t", "").Split('•');

                        //JobInfo 3items  1 companyName 2 Region 3 Salary
                        if (JobInfo.Length > 0)
                            jobParsed.ComapnyName = JobInfo[0];
                        if (JobInfo.Length > 1)
                            jobParsed.Region = JobInfo[1];
                        if (JobInfo.Length > 2)
                            jobParsed.Salary = extractSalary(JobInfo[2]);

                        jobParsed.Description = job.SelectSingleNode(".//div[@class='d']").InnerText;

                        var categories = job.SelectNodes(".//div[@class='tags']/a");

                        jobParsed.Categories = new List<string>();
                        if (categories != null)
                        {
                            foreach (var category in categories)
                            {
                                jobParsed.Categories.Add(category.InnerText);
                            }
                        }
                        jobParsed.Portal = "rabota.ua";
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
