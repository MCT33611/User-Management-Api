using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using User_Management_Api.Data;
using User_Management_Api.Model;
using User_Management_Api.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace User_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        public AccountController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }



        [HttpPost("Register")]
        public JsonResult Register(User user)
        {
           
            try
            {
                if (_db.Users.FirstOrDefault((u) => user.Name == u.Name) != null)
                {
                    return new JsonResult(new { status = "Error", message = "Username Already exist" });
                }
                if (!user.Name.IsNullOrEmpty() && !user.HashedPassword.IsNullOrEmpty() && user.HashedPassword != null)
                {
                    user.RoleId = 2;
                    _db.Users.Add(user);
                    _db.SaveChanges();
                    return new JsonResult(new { status = "Success", message = "User regitered successfully" });
                }

                return new JsonResult(new { status = "Error", message = "Invalied inputs" });
            }
            catch(Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            

            try
            {
                User? user = await _db.Users.FirstOrDefaultAsync(u => u.Name == model.Name);
                user.Role = await _db.Roles.FirstOrDefaultAsync(u => u.RoleId == user.RoleId);

                if (user == null)
                {
                    return new JsonResult(new { status = "Error", message = "Username does not exist" });
                }
                if (user.IsBlocked)
                {
                    return new JsonResult(new { status = "Error", message = "User is blocked by Admin" });
                }

                if (user.HashedPassword != model.HashedPassword)
                {
                    return new JsonResult(new { status = "Error", message = "Password is wrong" });
                }

                if (user.Role != null && user.Role.RoleName == "ADMIN")
                {
                    return new JsonResult(new { status = "Success", token = GenerateJwtToken(user.Name,user.Id,true), RoleId = user.RoleId, message = "Successfully logged in",userId = user.Id });
                }

                return new JsonResult(new { status = "Success", token = GenerateJwtToken(user.Name,user.Id), message = "Successfully logged in" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        private string GenerateJwtToken(string? username, int userId,bool isAdmin = false)
        {
            var key = Encoding.ASCII.GetBytes(_config["JwtSettings:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("userId", userId.ToString()), 
                new Claim("isAdmin", isAdmin.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
