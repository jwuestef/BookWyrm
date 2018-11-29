using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.Models
{
    public class Borrowing
    {

        public int BorrowingId { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime CheckOutDateTime { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? CheckInDateTime { get; set; }



    }
}
