using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }

        [NotMapped]
        public ICollection<Product> Products { get; set; }

        public decimal Value { get; set; }
        public int PointsValue { get; set; }
        public DateTime Date { get; set; }

        public string AdditionalNotes { get; set; }

    }
}
