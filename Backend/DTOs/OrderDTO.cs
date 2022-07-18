using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class OrderDTO
    {
       public Guid UserId { get; set; }

       public List<int> ProductIds { get; set; }

       public decimal Value { get; set; }

       public int PointsValue { get; set; }

       public DateTime Date { get; set; }

       public string AdditionalNotes { get; set; }
    }
}
