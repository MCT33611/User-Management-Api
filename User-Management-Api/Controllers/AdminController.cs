using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using User_Management_Api.Data;
using User_Management_Api.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace User_Management_Api.Controllers
{
    [Authorize]
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
            try
            {
                return new JsonResult(_db.Users);

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        // GET api/<AdminController>/5
        [HttpGet("{id}")]
        public JsonResult Get(int id)
        {

            try
            {

                User? user = _db.Users.SingleOrDefault(user => id == user.Id);
                return new JsonResult(user == null ? new { status = "Error", message = "User Not foud" } : user);

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        [HttpPost("upsertUser")]
        public JsonResult upsertUser(User user)
        {

            try
            {
                if (_db.Users.SingleOrDefault((u) => user.Name == u.Name && user.Id != u.Id) != null)
                {
                    return new JsonResult(new { status = "Error", message = "Username Already exist" });
                }
                if (!user.Name.IsNullOrEmpty() && !user.HashedPassword.IsNullOrEmpty() && user.HashedPassword != null)
                {
                    user.RoleId = 2;

                    if (user.Id == 0)
                    {
                        _db.Users.Add(user);
                    }
                    else
                    {
                        user.Role = null;
                        _db.Users.Update(user);
                    }
                    
                    _db.SaveChanges();
                    return new JsonResult(new { status = "Success", message = "User added successfully" });
                }

                return new JsonResult(new { status = "Error", message = "Invalied inputs" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }

        // POST api/<AdminController>
        [HttpPost("Search")]
        public JsonResult Search(string query)
        {
            
            try
            {
                var users = _db.Users.Where(u => EF.Functions.Like(u.Name, $"%{query}%")).ToList();
                return new JsonResult(users);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }
        }


        // PUT api/<AdminController>/5
        [HttpPut("Access/{id}")]
        public JsonResult Put(int id)
        {


            try
            {
                User? user = _db.Users.SingleOrDefault(user => id == user.Id);
                if (user == null)
                {
                    return new JsonResult(new { status = "Error", message = "User Not foud" });
                }
                user.IsBlocked = !user.IsBlocked;
                _db.Users.Update(user);
                _db.SaveChanges();

                return new JsonResult(new { status = "Success", message = "User Access changed Successfuly" });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", message = ex.Message });
            }

        }

    }
}
