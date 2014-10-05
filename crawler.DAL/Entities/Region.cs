using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crawler.DAL.Entities
{
    public class Region
    {
        public Region() { }

        [Key]
        public int id { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }
    }
}
