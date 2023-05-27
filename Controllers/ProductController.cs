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
using Newtonsoft.Json;

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
        public IActionResult SellProduct(string errorMessage = "")
        {
            // Get any error message
            if(!string.IsNullOrEmpty(errorMessage))
            {
                ViewData["ErrorMessage"] = errorMessage;
            }

            ViewBag.Categories = _dbContext.Categories.ToList();
            ViewBag.Conditions = _dbContext.Conditions.ToList();
            ViewBag.DeliveryTypes = _dbContext.DeliveryTypes.ToList();
            var sellProuctModel = new SellProductModel();
            return View(sellProuctModel);
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
                            return RedirectToAction("SellProduct", "Product", new {errorMessage = "Can't sell a product because the data is invalid"});
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
            return RedirectToAction("SellProduct", "Product", new {errorMessage = "Can't sell a product because the data is invalid"});
        }

        [HttpGet]
        public IActionResult Product(int id, string errorMessage = "")
        {
            // Get any error message
            if(!string.IsNullOrEmpty(errorMessage))
            {
                ViewData["ErrorMessage"] = errorMessage;
            }

            var currentUserId = _userManager.GetUserId(User);
            var product = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Condition)
                .Include(p => p.Person)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductPrices)
                .Include(p => p.DeliveryTypePrices)
                .ThenInclude(deliveryTypePrice => deliveryTypePrice.DeliveryType)
                .SingleOrDefault(p => p.Id == id);

            ViewBag.isAuthor = currentUserId == product?.Person.Id ? true : false;
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public IActionResult DelProduct(int id = -1)
        {
            string errorMessage = "";

            if(id != -1)
            {
                var product = _dbContext.Products
                    .Include(p => p.Person)
                    .SingleOrDefault(p => p.Id == id);

                if(product != null)
                {
                    if(_userManager.GetUserId(User) == product.Person.Id)
                    {
                        try
                        {
                            _dbContext.Products.Remove(product);
                            _dbContext.SaveChanges();
                            _sellProductService.DeleteImageFiles(id);
                            return RedirectToAction("Index", "Home", new {successMessage = "Removed product"});
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"Can't delete product, {ex.Message}");
                            errorMessage = "Can't delete product";
                            if(Request.Headers["Referer"].ToString().Contains("errorMessage"))
                            {
                                Request.Headers["Referer"] = Request.Headers["Referer"].ToString().Substring(0, Request.Headers["Referer"].ToString().IndexOf("?"));
                            }
                            return Redirect($"{Request.Headers["Referer"]}?errorMessage={Uri.EscapeDataString(errorMessage)}");
                        }   
                    }
                    else
                    {
                        errorMessage = "This product isn't yours";
                        if(Request.Headers["Referer"].ToString().Contains("errorMessage"))
                        {
                            Request.Headers["Referer"] = Request.Headers["Referer"].ToString().Substring(0, Request.Headers["Referer"].ToString().IndexOf("?"));
                        }
                        return Redirect($"{Request.Headers["Referer"]}?errorMessage={Uri.EscapeDataString(errorMessage)}");
                    }     
                }
            }
            Console.WriteLine($"Can't find a product with this id {id}");
            errorMessage = $"Can't find product";
            if(Request.Headers["Referer"].ToString().Contains("errorMessage"))
            {
                Request.Headers["Referer"] = Request.Headers["Referer"].ToString().Substring(0, Request.Headers["Referer"].ToString().IndexOf("?"));
            }
            return Redirect($"{Request.Headers["Referer"]}?errorMessage={Uri.EscapeDataString(errorMessage)}");
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditProduct(int id = -1, string errorMessage = ""){
            if(id != -1)
            {
                var product = _dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.Condition)
                    .Include(p => p.Person)
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductPrices)
                    .Include(p => p.DeliveryTypePrices)
                    .ThenInclude(deliveryTypePrice => deliveryTypePrice.DeliveryType)
                    .SingleOrDefault(p => p.Id == id);

                if(product != null)
                {
                    if(_userManager.GetUserId(User) == product.Person.Id)
                    {
                        // Get any error message
                        if(!string.IsNullOrEmpty(errorMessage))
                        {
                            ViewData["ErrorMessage"] = errorMessage;
                        }

                        ViewBag.Categories = _dbContext.Categories.ToList();
                        ViewBag.Conditions = _dbContext.Conditions.ToList();
                        ViewBag.DeliveryTypes = _dbContext.DeliveryTypes.ToList();
                        ViewBag.Product = product;
                        ViewBag.ActualPrice = product.ProductPrices.OrderByDescending(pp => pp.Datetime).FirstOrDefault().Price;
                        ViewBag.Images = JsonConvert.SerializeObject(_sellProductService.GetImageFiles(product.Id, product.ProductImages.ToList()), Formatting.Indented);
                        var sellProuctModel = new SellProductModel();
                        return View(sellProuctModel);
                    }
                    else
                    {
                        errorMessage = "This product isn't yours";
                        if(Request.Headers["Referer"].ToString().Contains("errorMessage"))
                        {
                            Request.Headers["Referer"] = Request.Headers["Referer"].ToString().Substring(0, Request.Headers["Referer"].ToString().IndexOf("?"));
                        }
                        return Redirect($"{Request.Headers["Referer"]}?errorMessage={Uri.EscapeDataString(errorMessage)}");
                    }     
                }
            }
            Console.WriteLine($"Can't find a product with this id {id}");
            errorMessage = $"Can't find product";
            if(Request.Headers["Referer"].ToString().Contains("errorMessage"))
            {
                Request.Headers["Referer"] = Request.Headers["Referer"].ToString().Substring(0, Request.Headers["Referer"].ToString().IndexOf("?"));
            }
            return Redirect($"{Request.Headers["Referer"]}?errorMessage={Uri.EscapeDataString(errorMessage)}");
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProduct(SellProductModel sellProductModel, int id)
        {
            string errorMessage = "";
            
            if(id != -1)
            {
                var product = _dbContext.Products
                    .Include(p => p.Person)
                    .SingleOrDefault(p => p.Id == id);

                if(product != null)
                {
                    if(_userManager.GetUserId(User) == product.Person.Id)
                    {
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

                                        // Update product data
                                        product.Name = sellProductModel.Name;
                                        product.Amount = sellProductModel.Amount;
                                        product.Description = sellProductModel.Description;
                                        product.PersonId = _userManager.GetUserId(User);
                                        product.CategoryId = category.Id;
                                        product.ConditionId = condition.Id;
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

                                        // Remove old images files
                                        var imagesToDelete = _dbContext.ProductImages.Where(productImage => productImage.ProductId == product.Id);
                                        _dbContext.ProductImages.RemoveRange(imagesToDelete);
                                        _dbContext.SaveChanges();

                                        // Save images files if they are included
                                        if(sellProductModel.imageFiles != null)
                                        {
                                            List<string> imageLinks;
                                            if(_sellProductService.SaveImageFilesWithDelete(sellProductModel.imageFiles, out imageLinks, product.Id) == false)
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
                                        return RedirectToAction("EditProduct", "Product", new {id = id, errorMessage = "Can't edit product because the data is invalid"});
                                    }
                                }
                                return RedirectToAction("Product","Product", new {id = id});
                            }
                            else
                            {
                                errorMessage = "Can't edit product because the data is invalid";
                                return RedirectToAction("EditProduct", "Product", new {id = id, errorMessage = errorMessage});
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
                            errorMessage = "Can't edit product because the data is invalid";
                            return RedirectToAction("EditProduct", "Product", new {id = id, errorMessage = errorMessage});
                        }

                    }
                    else
                    {
                        Console.WriteLine($"User {_userManager.GetUserId(User)} tried to edit product with id {id}");
                        errorMessage = "This product isn't yours";
                        return RedirectToAction("EditProduct", "Product", new {id = id, errorMessage = errorMessage});
                    }
                }
                else
                {
                    Console.WriteLine($"Product with {id} is null");
                    errorMessage = $"Can't find product";
                }
            }
            else
            {
                Console.WriteLine($"Can't find a product with this id {id}");
                errorMessage = $"Can't find product";
            }
            if(id == -1)
            {
                return RedirectToAction("Index", "Home", new {errorMessage = errorMessage});
            }
            else
            {
                return RedirectToAction("EditProduct", "Product", new {id = id, errorMessage = errorMessage});
            }
            
        }

        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _dbContext.Categories.ToList();
            return Ok(categories);
        }
    }
}