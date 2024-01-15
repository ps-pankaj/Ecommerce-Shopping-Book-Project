using Ecommerce_Project1.DataAccess.Repository.IRepository;
using Ecommerce_Project1.Models;
using Ecommerce_Project1.Models.ViewModels;
using Ecommerce_Project1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce_Project1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
              _unitOfWork = unitOfWork;
              _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new {data=_unitOfWork.Product.GetAll()});
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb= _unitOfWork.Product.Get(id);
            if (productInDb == null)
                return Json(new { success = false, message = "Something Went Wrong" });
            _unitOfWork.Product.Remove(productInDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Data Successfully Deleted" });
        
        
        }
        #endregion
        public IActionResult Upsert(int? id) 
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem() {
                Text= cl.Name,
                Value=cl.Id.ToString()
                }),
                CoverTypeList=_unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem() {
                Text=cl.Name,
                Value=cl.Id.ToString()
                })
            };
            if (id == null) return View(productVM);
            productVM.Product=_unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null) return NotFound();
            return View(productVM);
        
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if(ModelState.IsValid) 
            {
                var webRootPath = _webHostEnvironment.WebRootPath; //it takes starting path
                var files =HttpContext.Request.Form.Files; 
                if(files.Count()>0)
                {
                    var fileName=Guid.NewGuid().ToString(); // generate random names and it can not be dublicate
                    var extension = Path.GetExtension(files[0].FileName); //it takes file extension
                    var uploads = Path.Combine(webRootPath,@"images\product"); 
                    if(productVM.Product.Id !=0)
                    { 
                        var imageExists=_unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;
                    
                    }
                    if(productVM.Product.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.Trim('\\'));
                        if(System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }

                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads,fileName + extension),FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl=@"\images\product\"+ fileName+extension;

                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitOfWork.Product.Get(productVM.Product.Id).ImageUrl;
                        productVM.Product.ImageUrl = imageExists;

                    }
                }
                if(productVM.Product.Id == 0)
                    _unitOfWork.Product.Add(productVM.Product);
                else
                    _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    })
                };
                if(productVM.Product.Id !=0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);

                }
                return View(productVM);
            }
        }

    }
}
