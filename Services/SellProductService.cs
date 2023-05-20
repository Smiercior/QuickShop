using QuickShop.Data;
using QuickShop.Models;
using System.Text.RegularExpressions;
using System.Globalization;

namespace QuickShop.Services
{
    public interface ISellProductService
    {
        public bool ValidateSellProductModel(SellProductModel sellProductModel);

        public bool SaveImageFiles(List<IFormFile> imageFiles, out List<string> imageLinks, int productId);

        public void DeleteImageFiles(int productId);
    }

    public class SellProductService : ISellProductService
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly IWebHostEnvironment _hostEnvironment;

        private string imagesFolder;

        public SellProductService(ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment)
        {
            _dbContext = dbContext;
            _hostEnvironment = hostEnvironment;
            imagesFolder = Path.Combine(_hostEnvironment.WebRootPath, "Media\\ProductImages");
        }

        public bool ValidateSellProductModel(SellProductModel sellProductModel)
        {
            // Validate if category name is correct
            bool isValidCategory = false;
            List<Category> categories = _dbContext.Categories.ToList();
            if(sellProductModel.Category == null)
            {
                Console.WriteLine("Category is null");
                return false;
            }
            foreach(var category in categories)
            {
                if(sellProductModel.Category == category.Name || sellProductModel.Category == "None") isValidCategory = true;
            }
            if(isValidCategory == false) 
            {
                Console.WriteLine("Category name is't correct");
                return false;
            }

            // Validate if condition name is correct
            bool isValidCondition = false;
            List<Condition> conditions = _dbContext.Conditions.ToList();
            if(sellProductModel.Condition == null)
            {
                Console.WriteLine("Condition is null");
                return false;
            }
            foreach(var condition in conditions)
            {
                if(sellProductModel.Condition == condition.Name) isValidCondition = true;
            }
            if(isValidCondition == false)
            {
                Console.WriteLine("Condition name is't correct");
                return false;
            }

            // Validate if delivery types is correct
            List<DeliveryType> deliveryTypes = _dbContext.DeliveryTypes.ToList();
            bool isCorrect;
            foreach(var deliveryTypeCheckbox in sellProductModel.DeliveryTypeCheckboxes)
            {
                isCorrect = false;
                foreach(var deliveryType in deliveryTypes)
                {
                    if(deliveryTypeCheckbox == deliveryType.Name)
                    {
                        isCorrect = true;
                        break;
                    }
                }
                if(isCorrect == false)
                {
                    Console.WriteLine($"Delivery type name incorrect");
                    return false;
                }
            }

            // Validate if price is correct
            float price = 0.00f;
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
            if(float.TryParse(sellProductModel.Price, styles, cultureInfo, out price))
            {
                if(price <= 0 || price >= 10000)
                {
                    Console.WriteLine($"Price should be grater than 0 and lesser than 10000");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Price is invalid");
                return false;
            }

            // Validate if delivery prices are correct
            float deliveryPrice = 0.00f;
            foreach(KeyValuePair<String, String> deliveryTypePrice in sellProductModel.DeliveryTypePrices)
            {
                deliveryPrice = 0.00f;

                if(Regex.IsMatch(deliveryTypePrice.Value, @"^(:?[\d,]+\.)*\d+$"))
                {
                    cultureInfo = new CultureInfo("en-US");
                }
                else if(Regex.IsMatch(deliveryTypePrice.Value, @"^(:?[\d.]+,)*\d+$"))
                {
                    cultureInfo = new CultureInfo("pl-PL");
                }

                if(float.TryParse(deliveryTypePrice.Value, styles, cultureInfo, out deliveryPrice))
                {
                    if(deliveryPrice <= 0 || deliveryPrice >= 1000)
                    {
                        Console.WriteLine($"Delivery type {deliveryTypePrice.Key} price should be grater than 0 and lesser than 1000");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"Delivery type {deliveryTypePrice.Key} price is invalid");
                    return false;
                }
            }

            // Validate image files if they are included
            List<string> availableExtension = new List<string>{"png", "jpeg", "jpg"};
            const float maxImagesSize = 5.00f;
            float imageFilesSize = 0.00f;

            if(sellProductModel.imageFiles != null)
            {
                // Validate image files number
                if(sellProductModel.imageFiles.Count > 5)
                {
                    Console.WriteLine("Only 5 images are allowed");
                    return false;
                }

                // Validate image files extensions
                foreach(var file in sellProductModel.imageFiles)
                {
                    if(!availableExtension.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')+1).ToLower()))
                    {
                        Console.WriteLine("Only *.png;*.jpeg;*.jpg file extensions");
                        return false;
                    }
                }

                // Validate image files size
                foreach(var file in sellProductModel.imageFiles)
                {
                    imageFilesSize += file.Length;
                }
                if(imageFilesSize/1048576 > maxImagesSize)
                {
                    Console.WriteLine("Maks images size is 5MB");
                    return false;
                }
            }

            return true;
        }

        public bool SaveImageFiles(List<IFormFile> imageFiles, out List<string> imageLinks, int productId)
        {
            string productImagesFolderPath = Path.Combine(imagesFolder, productId.ToString());
            string filePath = "";
            imageLinks = new List<string>();
            
            if(!Directory.Exists(productImagesFolderPath))
            {
                Directory.CreateDirectory(productImagesFolderPath);

                foreach(var file in imageFiles)
                {
                    try
                    {
                        if(file.Length > 0)
                        {
                            filePath = Path.Combine(imagesFolder, productId.ToString(), file.FileName);
                            imageLinks.Add(file.FileName);
                            using(Stream fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }
                        }
                    }
                    catch(NullReferenceException ex)
                    {
                        Console.WriteLine("There aren't any image files " + ex.Message);
                        return false;
                    }
                    catch(UnauthorizedAccessException ex)
                    {
                        Console.WriteLine($"Don't have access to save file in the path {filePath} " + ex.Message);
                        return false;
                    }
                    catch(DirectoryNotFoundException ex)
                    {
                        Console.WriteLine($"The directory {productImagesFolderPath} isn't exist " + ex.Message);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void DeleteImageFiles(int productId)
        {
            string productImagesFolderPath = Path.Combine(imagesFolder, productId.ToString());
            if(Directory.Exists(productImagesFolderPath))
            {
                Directory.Delete(productImagesFolderPath, true);
            }
        }
    }
}