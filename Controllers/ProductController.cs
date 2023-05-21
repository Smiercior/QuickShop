using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Data;
using QuickShop.Services;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            // Validate data
            Console.WriteLine(sellProductModel.Price);
            if(ModelState.IsValid)
            {
                if(_sellProductService.ValidateSellProductModel(sellProductModel))
                {
                    // Save data to database and save all image files if they are uploded by user
                    using(var transaction = _dbContext.Database.BeginTransaction())
                    {
                        bool filesCreated = false;
                        int productId = -1;

                        try
                        {
                            var category = _dbContext.Categories.FirstOrDefault(c => c.Name == sellProductModel.Category);
                            var condition = _dbContext.Conditions.FirstOrDefault(cond => cond.Name == sellProductModel.Condition);

                            // Add product object
                            var product = new Product();
                            product.Name = sellProductModel.Name;
                            product.Amount = sellProductModel.Amount;
                            product.Description = sellProductModel.Description;
                            product.PersonId = _userManager.GetUserId(User);
                            product.CategoryId = category.Id;
                            product.ConditionId = condition.Id;
                            _dbContext.Products.Add(product);
                            _dbContext.SaveChanges();

                            // Add product price object
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

                            // Add delivery type price objects
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

                            // Save images files if they are included
                            if(sellProductModel.imageFiles != null)
                            {
                                List<string> imageLinks;
                                if(_sellProductService.SaveImageFiles(sellProductModel.imageFiles, out imageLinks, product.Id) == false)
                                {
                                    throw new Exception("Can't save files");
                                }
                                else
                                {
                                    filesCreated = true;
                                    productId = product.Id;
                                }
                                foreach(var imageLink in imageLinks)
                                {
                                    var productImage = new ProductImage();
                                    productImage.ImageLink = imageLink;
                                    productImage.ProductId = product.Id;
                                    _dbContext.ProductImages.Add(productImage);
                                }
                            }

                            // Save and commit all changes
                            _dbContext.SaveChanges();
                            transaction.Commit();
                            return RedirectToAction("Product", "Product", new {id = product.Id});
                        }
                        catch(Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error occured during saving data: {ex.Message}");
                            if(filesCreated == true)
                            {
                                Console.WriteLine(productId);
                                _sellProductService.DeleteImageFiles(productId);
                            }
                            return RedirectToAction("SellProduct", "Home", new {errorMessage = "Can't sell a product because the data is invalid"});
                        }
                    }
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
        public IActionResult Product(int id)
        {
            //var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            var product = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Condition)
                .Include(p => p.Person)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductPrices)
                .Include(p => p.DeliveryTypePrices)
                .ThenInclude(deliveryTypePrice => deliveryTypePrice.DeliveryType)
                .SingleOrDefault(p => p.Id == id);
            return View(product);
        }

        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }
    }
}