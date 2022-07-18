using Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class CartOrderDTO
    {
        public List<ProductOrder> Products { get; set; }
        public string User { get; set; }
        public string AdditionalNotes { get; set; }
    }
}
