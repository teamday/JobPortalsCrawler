using crawler.DAL.Entities;
using crawler.PortalParsers;
using System;
using System.Data.Entity;
using System.Linq;


namespace crawler.DAL
{
    public class JobDbContext:DbContext
    {
        public JobDbContext() : base("JobCrawlerConnectionString"){
            Database.SetInitializer<JobDbContext>(new CreateDatabaseIfNotExists<JobDbContext>());
        }

   
        public DbSet<Company> Company { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<JobCategory> JobCategory { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Portal> Portal { get; set; }
     
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().
                HasMany(c => c.JobCategory).
                WithMany(p => p.Job).
                Map(
                m =>
                {
                    m.MapLeftKey("idJob");
                    m.MapRightKey("idJobCategory");
                   m.ToTable("JobCategortyJobLink");
                });
          //  base.OnModelCreating(modelBuilder);
        }

        public void SaveParsedJob(ParsedJob jobParsed)
        {
            if (!Company.Any(f => f.Name.Equals(jobParsed.ComapnyName.Trim())))
            {
                Company.Add(new Company() { Name = jobParsed.ComapnyName.Trim() });
                SaveChanges();
            }
            int idCompany = Company.First(c => c.Name.Equals(jobParsed.ComapnyName.Trim())).id;

            if (!Region.Any(r => r.Name.Equals(jobParsed.Region.Trim())))
            {
                Region.Add(new Region() { Name = jobParsed.Region.Trim() });
                SaveChanges();
            }
            int idRegion = Region.First(r => r.Name.Equals(jobParsed.Region.Trim())).id;

            if (!Portal.Any(r => r.Name.Equals(jobParsed.Portal.Trim())))
            {
                Portal.Add(new Portal() { Name = jobParsed.Portal.Trim() });
                SaveChanges();
            }
            int idPortal = Portal.First(r => r.Name.Equals(jobParsed.Portal.Trim())).id;

            if (!Job.Any(r => r.WebIdJob.Equals(jobParsed.WebIdJob.Trim())))
            {
                Job.Add(new Job()
                {
                    Name = jobParsed.JobName.Trim(),
                    idCompany = idCompany,
                    idRegion = idRegion,
                    Description = jobParsed.Description,
                    ParsedDateTime = DateTime.Now,
                    Url = jobParsed.JobUrl,
                    Salary = jobParsed.Salary,
                    WebIdJob = jobParsed.WebIdJob,
                    idPortal = idPortal
                });
                SaveChanges();
            }
           // int idJob = Job.Where(r => r.WebIdJob.Equals(jobParsed.WebIdJob.Trim())).First().id;
            if (jobParsed.Categories != null)
            {
                foreach (string keyword in jobParsed.Categories)
                {
                    if (!JobCategory.Any(r => r.Name.Equals(keyword.Trim())))
                    {
                        JobCategory.Add(new JobCategory() {Name = keyword.Trim()});
                        SaveChanges();
                    }

                    var jobCategory = JobCategory.First(r => r.Name.Equals(keyword.Trim()));

                    var job = Job.First(r => r.WebIdJob.Equals(jobParsed.WebIdJob.Trim()));

                    if (!job.JobCategory.Any(jc => jc.Name.Equals(keyword.Trim())))
                    {
                        job.JobCategory.Add(jobCategory);
                        SaveChanges();
                    }
                }
            }
        }
    }
}
