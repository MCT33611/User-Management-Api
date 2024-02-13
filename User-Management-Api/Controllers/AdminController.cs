using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User_Management_Api.Data;
using User_Management_Api.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User_Management_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public JsonResult GetAll()
        {
            return new JsonResult(_db.Users.ToList()) ;
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {

            User? user = _db.Users.SingleOrDefault(user => id == user.Id);
            return new JsonResult(user == null ? new { status = "Error", message = "User Not foud" } : user);
        }

        // POST api/<AdminController>
        [HttpPost("Search")]
        public JsonResult Search(string query)
        {
            var users = _db.Users.Where(u => EF.Functions.Like(u.Name, $"%{query}%")).ToList();
            return new JsonResult(users);
        }


        // PUT api/<AdminController>/5
        [HttpPut("Access")]
        public JsonResult Put(int id)
        {
            User? user = _db.Users.SingleOrDefault(user => id == user.Id);
            if(user == null)
            {
                return new JsonResult(new { status = "Error", message = "User Not foud" });
            }
            user.IsBlocked = !user.IsBlocked;
            _db.Users.Update(user);
            _db.SaveChanges();

            return new JsonResult(new { status = "Success", message = "User Access changed Successfuly" });

        }

    }
}
