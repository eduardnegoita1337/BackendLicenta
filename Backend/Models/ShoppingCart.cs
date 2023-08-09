using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public User User { get; set; }
        [NotMapped]
        public ICollection<Product> Products { get; set; }
        public Guid UserId { get; set; }
        public decimal Value { get; set; }
        public int PointsValue { get; set; }
        
    }
}
