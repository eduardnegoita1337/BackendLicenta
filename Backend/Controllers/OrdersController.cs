using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNet.OData;
using Backend.DTOs;
using Backend.Helpers;

namespace Backend.Controllers
{
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly RestaurantContext _context;
        
        
        public OrdersController(RestaurantContext context)
        {
            _context = context;
        }
        [EnableQuery]
        [HttpPut("api/orders/addItemToCart")]
        public async Task<IActionResult> AddItemToCart(ProductOrder product)
        {
            var productToAdd = _context.ProductOrders.FirstOrDefault(p => p.Name == product.Name && p.PointsPrice == product.PointsPrice);
            if (productToAdd == null)
            {
                product.Quantity++;
                await _context.ProductOrders.AddAsync(product);
                await _context.SaveChangesAsync();
                var productOrders = await _context.ProductOrders.ToListAsync();
                return Ok(productOrders);
            }
            else
            {
                productToAdd.Quantity++;
                await _context.SaveChangesAsync();
                var productOrders = await _context.ProductOrders.ToListAsync();
                return Ok(productOrders);
            }
        }
        [EnableQuery]
        [HttpPost("api/orders/placeOrder")]
        public async Task<IActionResult> PlaceOrder(CartOrderDTO dto)
        {
            var order = new Order();
            foreach(var p in dto.Products)
            {
                order.Value += p.Price * p.Quantity;
                order.PointsValue += p.PointsPrice * p.Quantity;
            }
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == dto.User);
            if (order.PointsValue > 0 && order.PointsValue > user.Points)
            {
                return BadRequest("You don't have enough points");
            }
            if (order.PointsValue > 0 && order.PointsValue <= user.Points)
            {
                user.Points -= order.PointsValue;
            }
           
            user.Points += (int)order.Value*10;
            order.UserId = user.Id;
            order.AdditionalNotes = dto.AdditionalNotes;
            order.Date = DateTime.Now;
            foreach (var p in _context.ProductOrders)
            {
                _context.Remove(p);
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return Ok(order);

        }
        [EnableQuery]
        [HttpGet("api/orders/getOrdersFromUser/{userId}")]
        public async Task<IActionResult> GetOrdersFromUser([FromRoute] string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id.ToString() == userId);
            if (user != null)
            {
                var orders = _context.Orders.Where(o => o.UserId == user.Id).ToList();
                return Ok(orders);
            }
            else return NotFound();
        }

        [EnableQuery]
        [HttpPut("api/orders/removeItemFromCart")]
        public async Task<IActionResult> RemoveItemFromCart(ProductOrder product)
        {
            var productToRemove = _context.ProductOrders.FirstOrDefault(p => p.Name == product.Name && p.PointsPrice == product.PointsPrice);
            productToRemove.Quantity--;
            if(productToRemove.Quantity <= 0)
            {
                _context.ProductOrders.Remove(productToRemove);
            }
            await _context.SaveChangesAsync();
            var productOrders = await _context.ProductOrders.ToListAsync();
            return Ok(productOrders);
        }

        [EnableQuery]
        [HttpGet("api/orders/getCartOrder")]
        public async Task<IActionResult>GetCart()
        {
            var productOrders = await _context.ProductOrders.ToListAsync();
            return Ok(productOrders);
        }
        [EnableQuery]
        [HttpGet("api/orders")]
        public async Task<IActionResult> Get()
        {
            var orders = await _context.Orders.Include(o => o.Products).ToListAsync();
            return Ok(orders);
        }
        [EnableQuery]
        [HttpGet("api/orders/{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(o => o.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [EnableQuery]
        [HttpPost("api/orders")]
        public async Task<IActionResult> Create([FromBody] OrderDTO orderDTO)
        {
            var order = new Order();
            if (ModelState.IsValid)
            {
                order.Date = DateTime.Now;
                order.Value = orderDTO.Value;
                order.PointsValue = orderDTO.PointsValue;
                order.Products = new List<Product>();
                order.AdditionalNotes = orderDTO.AdditionalNotes;
                if(orderDTO.ProductIds.Count() > 0)
                {
                    foreach(var id in orderDTO.ProductIds)
                    {
                        var product = _context.Products.Find(id);
                        if(product != null)
                        {
                            order.Products.Add(product);
                        }
                
                    }
                }
                var user = _context.Users.Find(orderDTO.UserId);

                if(order.Value > 0 && order.PointsValue == 0)
                {
                    user.Points += (int)order.Value * 10;
                }
                else if(order.Value > 0 && order.PointsValue > 0)
                {
                    user.Points -= order.PointsValue;
                }
                else if(order.Value == 0 && order.PointsValue > 0)
                {
                    user.Points -= order.PointsValue;
                }
                if(user == null)
                {
                    return NotFound("User not found.");
                }
                order.UserId = user.Id;
                _context.Add(order);
                await _context.SaveChangesAsync();
            }
            return Ok(order);
        }

        [EnableQuery]
        [HttpPut("api/orders/{id}")]

        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] OrderDTO orderDTO)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(orderDTO.UserId != null)
                    {
                        order.UserId = orderDTO.UserId;
                    }
                    if(orderDTO.ProductIds.Count() > 0)
                    {
                        order.Products = new List<Product>();
                        foreach (var prodId in orderDTO.ProductIds)
                        {
                            var product = _context.Products.Find(prodId);
                            if (product != null)
                            {
                                order.Products.Add(product);
                            }

                        }
                    }
                    if(orderDTO.Value != null)
                    {
                        order.Value = orderDTO.Value;
                    }
                    if(orderDTO.PointsValue != null)
                    {
                        order.PointsValue = orderDTO.PointsValue;
                    }
                    if(orderDTO.AdditionalNotes != null)
                    {
                        order.AdditionalNotes = orderDTO.AdditionalNotes;
                    }
                    if(orderDTO.Date != null)
                    {
                        order.Date = orderDTO.Date;
                    }
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Ok(order);
        }



        [EnableQuery]
        [HttpDelete("api/orders/{id}")]

        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
