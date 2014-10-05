using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.DAL.Entities
{
    public class Portal
    {
        public Portal()
        {
        }

        [Key]
        public int id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; }
    }
}
