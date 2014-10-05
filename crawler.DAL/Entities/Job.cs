using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.DAL.Entities
{
    public class Job
    {
        public Job() {
            JobCategory = new HashSet<JobCategory>();
        }

        [Key]
        public int id { get; set; }

        public DateTime ParsedDateTime { get; set; }
        public string Url { get; set; }

        public int idCompany { get; set; }
        [ForeignKey("idCompany")]
        public virtual Company Company { get; set; }
        
        public virtual ICollection<JobCategory> JobCategory { get; set; }

        public int idRegion { get; set; }
        [ForeignKey("idRegion")]
        public virtual Region Region { get; set; }

        public int? idPortal { get; set; }

        [ForeignKey("idPortal")]
        public virtual Portal Portal { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "Money")]
        public decimal? Salary { get; set; }

        [MaxLength(8000)]
        public string Description { get; set; }

        [MaxLength(256)]
        public string WebIdJob { get; set; }

        [Column(TypeName = "Date")]
        public DateTime PublishDate { get; set; }


    }
}
