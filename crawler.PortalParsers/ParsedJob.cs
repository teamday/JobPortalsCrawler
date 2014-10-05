using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.PortalParsers
{
    public struct ParsedJob
    {
        public string WebIdJob;
        public string LinkJob;
        public string JobName;
        public string JobUrl;
        public string ComapnyName;
        public string Region;
        public decimal? Salary;
        public string Description;
        public List<string> Categories;
        public string Portal;
    }
}
