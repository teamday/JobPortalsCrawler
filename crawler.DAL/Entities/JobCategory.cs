using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.DAL.Entities
{
    [Table("JobCategory")]
    public class JobCategory
    {
        public JobCategory() { }

        [Key]
        public int id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }

        public virtual ICollection<Job> Job { get; set; }
       
    }
}
