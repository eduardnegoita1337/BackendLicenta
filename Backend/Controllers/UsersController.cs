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
using Microsoft.AspNetCore.Cors;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        [EnableCors]
        [HttpPost("api/users/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Username);
                if (user == null)
                {
                    return NotFound(null);
                }
                PasswordEncrypter pe = new PasswordEncrypter();
                if (pe.DecodeFrom64(user.Password) == loginDto.Password)
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("licentakeylmao1234"));
                    var signingCredetials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var userClaims = new List<Claim>();
                    var claim = new Claim("id", user.Id.ToString());
                    var claim1 = new Claim("firstname", user.FirstName);
                    var claim2 = new Claim("lastname", user.LastName);


                    userClaims.Add(claim);
                    userClaims.Add(claim1);
                    userClaims.Add(claim2);
                    var tokenOptions = new JwtSecurityToken(
                        issuer: "https://localhost:44302",
                        audience: "https://localhost:4200",
                        claims: userClaims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: signingCredetials
                        );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                    LoginHistory lh = new LoginHistory();
                    lh.UserId = user.Id;
                    lh.LoginDate = DateTime.Now;
                    _context.LoginHistories.Add(lh);
                    foreach(var p in _context.ProductOrders)
                    {
                        _context.Remove(p);
                    }
                    await _context.SaveChangesAsync();
                    TokenDTO dto = new TokenDTO();
                    dto.Token = tokenString;
                    return Ok(dto);
                }
                else return BadRequest("WrongPassword");
            }
            catch (Exception ex)
            {
                return BadRequest(null);
            }
            
        }
        [EnableQuery]
        [HttpGet("api/users")]
        public async Task<IActionResult> Get( )
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [EnableQuery]
        [HttpGet("api/users/{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id.ToString() == id);
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
                user.Points = 0;
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
                return NotFound("User not found.");
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
