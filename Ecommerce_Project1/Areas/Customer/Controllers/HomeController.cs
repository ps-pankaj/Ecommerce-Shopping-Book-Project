using Ecommerce_Project1.DataAccess.Repository.IRepository;
using Ecommerce_Project1.Models;
using Ecommerce_Project1.Models.ViewModels;
using Ecommerce_Project1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecommerce_Project1.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string searching)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims =claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                     
            if(claims != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);

            }

            var productList = _unitOfWork.Product.GetAll(includeproperties:"Category,CoverType");
            if (String.IsNullOrEmpty(searching))
            {
                return View(productList);
            }
            else
            {
                productList = productList.Where(p => p.Title.Contains(searching) 
                || p.Author.Contains(searching) || p.Description.Contains(searching)).ToList();
            }

            return View(productList);
        }

       


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id )
        {
            var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id==id,
                includeProperties:"Category,CoverType");
            if (productInDb == null) return NotFound();
            var shoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
           
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return NotFound();
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingCartFromDb = _unitOfWork.ShoppingCart.FirstOrDefault
                    (sc=> sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingCart.ProductId);
                if (shoppingCartFromDb == null)
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                else
                    shoppingCartFromDb.Count += shoppingCart.Count;
                _unitOfWork.Save();
                return RedirectToAction("Index");
                

            }
            else
            {
                var productInDb = _unitOfWork.Product.FirstOrDefault(p => p.Id == shoppingCart.Id,
                includeProperties: "Category,CoverType");
                if (productInDb == null) return NotFound();
                var shoppingCartEdit = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };

                return View(shoppingCartEdit);
            }

            
        }
       
    
    }
}
