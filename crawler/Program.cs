using System.Threading;
using Abot.Poco;
using crawler.DAL;
using System;
using System.Threading.Tasks;
using crawler.PortalParsers.rabotaua;
using crawler.PortalParsers;

namespace crawler
{
    class Program
    {
        static JobDbContext ctx = new JobDbContext();
        static bool _crawlComplete = false;

     

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var rabotaUACrawler = new RabotaUACrawler();
            var hhuaCrawler = new hhuaCrawler();

            var rabotaUAParseTask = new Task<CrawlResult>(rabotaUACrawler.Crawl);
            var hhuaCrawlerParseTask = new Task<CrawlResult>(hhuaCrawler.Crawl);

            var saveJobsToDB = new Task(new Action(SaveJobFromStackToDB));

            rabotaUAParseTask.Start();
            hhuaCrawlerParseTask.Start();

            saveJobsToDB.Start();

            rabotaUAParseTask.Wait();
            hhuaCrawlerParseTask.Wait();

            _crawlComplete = true;

            saveJobsToDB.Wait();
            Console.ReadLine();
        }


        private static void SaveJobFromStackToDB()
        {
            while (!_crawlComplete)
            {
                Thread.Sleep(10);

                if (ParsedJobStack.Instance.Count > 0)
                {                    
                    ctx.SaveParsedJob(ParsedJobStack.Instance.Pop());
                }
            }
        }


    }

}
