using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoginDate { get; set; }
    }
}
