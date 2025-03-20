using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static TelephoneBookDbContext _context;

    public HomeController(ILogger<HomeController> logger,TelephoneBookDbContext telephoneBookDbContext)
    {
        _logger = logger;
        _context = telephoneBookDbContext;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult MainInformation()
    {
        var userCount = _context.TelephoneBook.ToList().Count;
        return View(userCount);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}