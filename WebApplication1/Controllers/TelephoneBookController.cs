using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;
using X.PagedList;

namespace WebApplication1.Controllers;

public class TelephoneBookController : Controller
{
    private readonly ILogger<TelephoneBookController> _logger;
    private static TelephoneBookDbContext _context;
    
    public TelephoneBookController(ILogger<TelephoneBookController> logger,TelephoneBookDbContext telephoneBookDbContext)
    {
        _logger = logger;
        _context = telephoneBookDbContext;
    }

    public IActionResult Users(string param,int pageNumber = 1)
    {
        var users = _context.TelephoneBook.AsQueryable();
        
        if (!string.IsNullOrEmpty(param))
        {
            users = users.Where(u =>
                u.Name.IndexOf(param, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.Surname.IndexOf(param, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.Telephone.IndexOf(param, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.Email.IndexOf(param, StringComparison.OrdinalIgnoreCase) >= 0 ||
                u.Address.IndexOf(param, StringComparison.OrdinalIgnoreCase) >= 0
            );
        }

        var pagedUsers = users.ToPagedList(pageNumber, 11);

        return View(pagedUsers); 
    }
    
    public IActionResult NewUser()
    {
        ViewData["Title"] = "New User";  // page name

        return View();
    }
    [HttpPost]
    public IActionResult NewUser(TelephoneBookModel telephoneBookModel)
    {
        ViewData["Title"] = "New User";  // page name
        
        if (_context.TelephoneBook.Any(u => u.Telephone == telephoneBookModel.Telephone))
        {
            ModelState.AddModelError("Telephone", "This phone number has been registered before!");
        }

        if (_context.TelephoneBook.Any(u => u.Email == telephoneBookModel.Email ))
        {
            ModelState.AddModelError("Email", "This email has been registered before!");
        }

        if (!EmailValidaton(telephoneBookModel.Email))
        {
            ModelState.AddModelError("Email", "Please don't forget to use '.com .org .net .info' at the end of your Email!");
        }

        if (!ModelState.IsValid)
        {
            ViewData["CustomErrorMessage"] = "Please fill out the form completely and enter correct information.";
            return View(telephoneBookModel);
        }
        
        // Gerekli alanlar dolu ise ve model doğrulanmışsa devam edin
        _context.TelephoneBook.Add(telephoneBookModel);
        _context.SaveChanges();
        
        return RedirectToAction("Users");
    }

    public IActionResult Edit(int id)
    {
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);
        if (user == null)
        {
            return NotFound(); // Kullanıcı bulunamadıysa 404 hatası döndür
        }
        return View(user);
    }
    
    [HttpPost]
    public IActionResult Edit(int id, TelephoneBookModel telephoneBookModel)
    {
        
        // Veritabanından kullanıcıyı bul
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);
        if (user == null)
        {
            return NotFound(); // Kullanıcı bulunamazsa 404 hatası döndür
        }
        
        if (_context.TelephoneBook.Any(u => u.Telephone == telephoneBookModel.Telephone && u.ID != id))
        {
            ModelState.AddModelError("Telephone", "This phone number has been registered before!");
        }

        if (_context.TelephoneBook.Any(u => u.Email == telephoneBookModel.Email && u.ID != id))
        {
            ModelState.AddModelError("Email", "This email has been registered before!");
        }

        if (!EmailValidaton(telephoneBookModel.Email))
        {
            ModelState.AddModelError("Email", "Please don't forget to use '.com .org .net .info .hr' at the end of your Email!");
        }

        if (!ModelState.IsValid)
        {
            ViewData["CustomErrorMessage"] = "Please fill out the form completely and enter correct information.";
            return View(telephoneBookModel);
        }
        
        // Veritabanında kullanıcıyı güncelle
        user.Name = telephoneBookModel.Name;
        user.Surname = telephoneBookModel.Surname;
        user.Address = telephoneBookModel.Address;
        user.Email = telephoneBookModel.Email;
        user.Telephone = telephoneBookModel.Telephone;

        _context.SaveChanges();
        
        return RedirectToAction("Users");
    }
    
    public IActionResult Delete(int id, TelephoneBookModel telephoneBookModel)
    {
        // find the user
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);
        
        if (user == null)
        {
            return NotFound(); // if user wouldn't find, return error 404 !
        }
        
        _context.TelephoneBook.Remove(user); 
        _context.SaveChanges(); 

        return RedirectToAction("Users");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public bool EmailValidaton(string email)
    {
        string[] acceptedDomains = { ".com", ".org", ".net", ".info", ".hr", ".gov", ".edu", ".mil", ".biz" };
        
        bool hasAcceptedDomain = false;
        foreach (var domain in acceptedDomains)
        {
            if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                hasAcceptedDomain = true;
                break;
            }
        }
        
        return hasAcceptedDomain;
    }
}