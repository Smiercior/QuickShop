using QuickShop.Data;
using QuickShop.Models;

namespace QuickShop.Services
{
    public interface ISellProductService
    {
        public bool ValidateSellProductModel(SellProductModel sellProductModel);

        public void SaveImageFiles();
    }

    public class SellProductService : ISellProductService
    {
        private readonly ApplicationDbContext _dbContext;

        public SellProductService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

            // Validate if delivery prices are correct
            float price = 0.00f;
            foreach(KeyValuePair<String, String> deliveryTypePrice in sellProductModel.DeliveryTypePrices)
            {
                price = 0.00f;
                if(float.TryParse(deliveryTypePrice.Value, out price))
                {
                    if(price <= 0 || price > 1000)
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

        public void SaveImageFiles()
        {

        }
    }
}