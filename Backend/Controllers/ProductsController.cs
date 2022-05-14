using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNet.OData;
using Backend.DTOs;
using Microsoft.AspNetCore.Http;

namespace Backend.Controllers
{

    [ApiController]
    public class ProductsController : Controller
    {
        private readonly RestaurantContext _context;

        public ProductsController(RestaurantContext context)
        {
            _context = context;
        }
        
        [EnableQuery]
        [HttpGet("api/products")]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [EnableQuery]
        [HttpGet("api/products/{id}")]
        public async Task<IActionResult> Get([FromRoute] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [EnableQuery]
        [HttpPost("api/products")]
        public async Task<IActionResult> Create([FromBody] ProductDTO productDTO)
        {
            var product = new Product();
            if (ModelState.IsValid)
            {
                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                _context.Add(product);
                await _context.SaveChangesAsync();
            }
            return Ok(product);
        }

        [EnableQuery]
        [HttpPut("api/products/{id}")]
        
        public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] ProductDTO productDTO)
        {
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Name = productDTO.Name;
                    product.Description = productDTO.Description;
                    product.Price = productDTO.Price;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Ok(product);
        }



        [EnableQuery]
        [HttpDelete("api/products/{id}")]
        
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
