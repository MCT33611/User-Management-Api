using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using User_Management_Api.Data;
using User_Management_Api.Model;
using User_Management_Api.ViewModel;

namespace User_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db)
        {
            _db = db;
        }



        [HttpPost("Register")]
        public JsonResult Register(User user)
        {
            if(_db.Users.SingleOrDefault((u) => user.Name == u.Name) != null)
            {
                return new JsonResult(new { status="Error",message="Username Already exist"});
            }
            if(!user.Name.IsNullOrEmpty() && !user.HashedPassword.IsNullOrEmpty() && user.HashedPassword != null)
            {
                user.Role = null;
                _db.Users.Add(user);
                _db.SaveChanges();
                return new JsonResult("Success");
            }

            return new JsonResult(new {status= "Error",message = "Invalied inputs" });
        }

        [HttpPost("Login")]
        public JsonResult Login(LoginViewModel model)
        {
            User? user = (User?)_db.Users.Where(u => u.Name == model.Name);
            
            if (user == null || user.HashedPassword == null || model.HashedPassword == null)
            {
                return new JsonResult(new { status = "Error", message = "Username not exitst " });
            }
            
            if (user.HashedPassword!= model.HashedPassword)
            {
                return new JsonResult(new { status = "Error", message = "Password is wrong " });
            }
            if (user.Role != null)
            {
                if (user.Role.RoleName == "ADMIN")
                {
                    return new JsonResult(new { status = "Success", Role = user.Role.RoleName ,  message = "Success fully logined " });
                }
            }
            
            return new JsonResult(new { status = "Success", message = "Success fully logined " });


        }
    }
}
