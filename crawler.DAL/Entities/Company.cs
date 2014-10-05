using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.DAL.Entities
{
    [Table("Company")]
    public class Company
    {
        public Company() { }

        [Key]
        public int id {get;set;}

        [MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(1024)]
        public string LogoUrl { get; set; }

        [MaxLength(8000)]
        public string Description { get; set; }

    }
}
