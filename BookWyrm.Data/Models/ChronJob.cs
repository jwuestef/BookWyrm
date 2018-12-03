using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.Models
{
    public class ChronJob
    {
        public int ChronJobId { get; set; }

        public DateTime DateRan { get; set; }

        public Guid RandomKey { get; set; }


    }
}
