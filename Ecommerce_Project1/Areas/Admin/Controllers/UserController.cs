using Ecommerce_Project1.DataAccess.Data;
using Ecommerce_Project1.DataAccess.Repository.IRepository;
using Ecommerce_Project1.Models;
using Ecommerce_Project1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Project1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+","+ SD.Role_Employee)]

    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]

        public IActionResult GetAll()
        {
            var userList = _context.ApplicationUsers.ToList(); //AspNetUser
            var roles = _context.Roles.ToList(); //AspNetRoles
            var userRoles = _context.UserRoles.ToList(); //AspNetUserRole

            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };

                }
                if (user.CompanyId == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }

            }
            //admin user remove from user list
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);


            return Json(new { data = userList });

        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            bool isLocked=false;
            var userInDB =_context.ApplicationUsers.FirstOrDefault(u=>u.Id == id);
            if (userInDB == null)
                return Json(new { success = false, message = "Something went wrong while lock or unlock " });
            if(userInDB != null && userInDB.LockoutEnd>DateTime.Now)
            {
                userInDB.LockoutEnd=DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDB.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true,message=isLocked==true?
                "User successfully locked":"User successfully unlocked"});
        }
        #endregion
    }
}
