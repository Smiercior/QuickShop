using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Data;
using QuickShop.Services;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace QuickShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ISellProductService _sellProductService;
        private readonly UserManager<Person> _userManager;

        public ProductController(ILogger<HomeController> logger, ApplicationDbContext dbContext, ISellProductService sellProductService, UserManager<Person> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _sellProductService = sellProductService;
            _userManager = userManager;
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

            // Validate data
            Console.WriteLine(sellProductModel.Price);
            if(ModelState.IsValid)
            {
                if(_sellProductService.ValidateSellProductModel(sellProductModel))
                {
                    // Save data to database and save all image files if they are uploded by user
                    using(var transaction = _dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var category = _dbContext.Categories.FirstOrDefault(c => c.Name == sellProductModel.Category);
                            var condition = _dbContext.Conditions.FirstOrDefault(cond => cond.Name == sellProductModel.Condition);

                            var product = new Product();
                            product.Name = sellProductModel.Name;
                            product.Amount = sellProductModel.Amount;
                            product.Description = sellProductModel.Description;
                            product.PersonId = _userManager.GetUserId(User);
                            product.CategoryId = category.Id;
                            product.ConditionId = condition.Id;
                            _dbContext.Products.Add(product);
                            _dbContext.SaveChanges();

                            var productPrice = new ProductPrice();
                            var cultureInfo = CultureInfo.InvariantCulture;
                            NumberStyles styles = NumberStyles.Number;
                            if(Regex.IsMatch(sellProductModel.Price, @"^(:?[\d,]+\.)*\d+$"))
                            {
                                cultureInfo = new CultureInfo("en-US");
                            }
                            else if(Regex.IsMatch(sellProductModel.Price, @"^(:?[\d.]+,)*\d+$"))
                            {
                                cultureInfo = new CultureInfo("pl-PL");
                            }
                            productPrice.Datetime = DateTime.Now;
                            productPrice.Price = float.Parse(sellProductModel.Price, styles, cultureInfo);
                            productPrice.ProductId = product.Id;
                            _dbContext.ProductPrices.Add(productPrice);

                            foreach(var deliveryTypeCheckbox in sellProductModel.DeliveryTypeCheckboxes)
                            {
                                var deliveryType = _dbContext.DeliveryTypes.FirstOrDefault(dT => dT.Name == deliveryTypeCheckbox);
                                var deliveryTypePrice = new DeliveryTypePrice();
                                if(Regex.IsMatch(sellProductModel.DeliveryTypePrices[deliveryTypeCheckbox], @"^(:?[\d,]+\.)*\d+$"))
                                {
                                    cultureInfo = new CultureInfo("en-US");
                                }
                                else if(Regex.IsMatch(sellProductModel.DeliveryTypePrices[deliveryTypeCheckbox], @"^(:?[\d.]+,)*\d+$"))
                                {
                                    cultureInfo = new CultureInfo("pl-PL");
                                }
                                deliveryTypePrice.Price = float.Parse(sellProductModel.DeliveryTypePrices[deliveryTypeCheckbox], styles, cultureInfo);
                                deliveryTypePrice.DeliveryTypeId = deliveryType.Id;
                                deliveryTypePrice.ProductId = product.Id;
                                _dbContext.DeliveryTypePrices.Add(deliveryTypePrice);
                            }

                            _dbContext.SaveChanges();
                            transaction.Commit();
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error occured during saving data: {ex.Message}");
                        }
                    }
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