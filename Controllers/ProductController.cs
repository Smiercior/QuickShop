using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Data;
using QuickShop.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace QuickShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        private readonly ISellProductService _sellProductService;

        public ProductController(ILogger<HomeController> logger, ApplicationDbContext dbContext, ISellProductService sellProductService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sellProductService = sellProductService;
        }

        [Authorize]
        [HttpPost]
        public IActionResult Product(SellProductModel sellProductModel)
        {
            // foreach(var file in sellProductModel.imageFiles)
            // {
            //     Console.WriteLine(file.FileName);
            // }
            // foreach(var deliveryTypeCheckbox in sellProductModel.DeliveryTypeCheckboxes)
            // {
            //     Console.WriteLine(deliveryTypeCheckbox);
            //     Console.WriteLine(sellProductModel.DeliveryTypePrices[deliveryTypeCheckbox]);
            // }   
            if(ModelState.IsValid)
            {
                if(_sellProductService.ValidateSellProductModel(sellProductModel))
                {
                    return View();
                }    
            }
            else
            {
                foreach(var modelStateEntity in ModelState.Values)
                {
                    foreach (var error in modelStateEntity.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }        
            }
            return RedirectToAction("SellProduct", "Home", new {errorMessage = "Can't sell a product because the data is invalid"});
        }

        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }
    }
}