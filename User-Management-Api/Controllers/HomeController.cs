using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using User_Management_Api.Data;
using User_Management_Api.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User_Management_Api.Controllers
{
    [Authorize]

    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {   
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            
            try
            {
                var user = _db.Users.SingleOrDefault(user => id == user.Id);
                return new JsonResult(user == null ? new { status = "Error", message = "User Not foud" } : user);

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public IActionResult Put(User user)
        {


            try
            {
                var existingUser = _db.Users.FirstOrDefault(u => u.Name == user.Name && u.Id != user.Id);
                if (existingUser != null)
                {
                    return new JsonResult(new { status = "Error", message = "Username already exists" });
                }

                if (!string.IsNullOrEmpty(user.Name) && !string.IsNullOrEmpty(user.HashedPassword))
                {
                    _db.Users.Update(user);
                    _db.SaveChanges();
                    return new JsonResult(new { status = "Success", message = "Profile updated successfully" });
                }

                return new JsonResult(new { status = "Error", message = "Invalid inputs" });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }


        // DELETE api/<AdminController>/5
        [HttpDelete("Delete")]
        public JsonResult Delete(int id)
        {

            try
            {
                User? user = _db.Users.SingleOrDefault(user => id == user.Id);
                if (user == null)
                {
                    return new JsonResult(new { status = "Error", message = "User Not foud" });
                }
                _db.Users.Remove(user);
                _db.SaveChanges();
                return new JsonResult(new { status = "Success", message = "Account Deleted Successfully" });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }
    }


    
}
