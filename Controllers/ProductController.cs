using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace QuickShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
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
                Console.WriteLine("oksdddddddddddddddddsdsdsdsdsdsdsd");
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
            return View();
        }

        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }
    }
}