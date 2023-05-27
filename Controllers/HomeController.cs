using Microsoft.AspNetCore.Mvc;
using QuickShop.Models;
using QuickShop.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace QuickShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(string successMessage = "")
        {
            // Get any success message
            if(!string.IsNullOrEmpty(successMessage))
            {
                ViewData["SuccessMessage"] = successMessage;
            }

            return View();
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
    }
}