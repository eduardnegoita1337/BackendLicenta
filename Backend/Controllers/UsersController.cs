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
    public class UsersController : Controller
    {
        private readonly RestaurantContext _context;

        public UsersController(RestaurantContext context)
        {
            _context = context;
        }
        [EnableQuery]
        [HttpGet("api/users")]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [EnableQuery]
        [HttpGet("api/users/{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [EnableQuery]
        [HttpPost("api/users")]
        public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
        {
            var user = new User();
            if (ModelState.IsValid)
            {
                user.Email = userDTO.Email;
                var pe = new PasswordEncrypter();
                user.Password = pe.EncodePasswordToBase64(userDTO.Password);
                user.FirstName = userDTO.FirstName;
                user.LastName = userDTO.LastName;
                user.RegistrationDate = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            return Ok(user);
        }

        [EnableQuery]
        [HttpPut("api/users/{id}")]

        public async Task<IActionResult> Edit([FromRoute] Guid id, [FromBody] UserEditDTO userDTO)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(userDTO.Email != null)
                    {
                        user.Email = userDTO.Email;
                    }
                    if(userDTO.FirstName != null)
                    {
                        user.FirstName = userDTO.FirstName;
                    }
                    if(userDTO.LastName != null)
                    {
                        user.LastName = userDTO.LastName;
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Ok(user);
        }



        [EnableQuery]
        [HttpDelete("api/users/{id}")]

        public async Task<IActionResult> DeleteConfirmed([FromRoute] Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
