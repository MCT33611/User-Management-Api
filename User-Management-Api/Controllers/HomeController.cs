using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using User_Management_Api.Data;
using User_Management_Api.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User_Management_Api.Controllers
{
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
            var user = _db.Users.SingleOrDefault(user => id == user.Id)  ;
            return new JsonResult(user == null ? new { status = "Error", message = "User Not foud" }:user);
        }

        [HttpPut("profile")]
        public JsonResult Put(User user)
        {
            if (_db.Users.SingleOrDefault((u) => user.Name == u.Name && user.Id != u.Id) != null)
            {
                return new JsonResult(new { status = "Error", message = "Username Already exist" });
            }
            if (!user.Name.IsNullOrEmpty() && !user.HashedPassword.IsNullOrEmpty() && user.HashedPassword != null)
            {
                _db.Users.Update(user);
                _db.SaveChanges();
                return new JsonResult("Success");
            }

            return new JsonResult(new { status = "Error", message = "Invalied inputs" });
        }


        // DELETE api/<AdminController>/5
        [HttpDelete("Delete")]
        public JsonResult Delete(int id)
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
    }


    
}
