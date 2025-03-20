using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using X.PagedList;
namespace WebApplication1.Controllers;

public class FileOperationsController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static TelephoneBookDbContext _context;
    private int column = 6;
    private Document document;
    private Font fontStyle;
    private Font fontStyle2;
    private PdfPTable table = new PdfPTable(6);
    private PdfPTable table2 = new PdfPTable(2);

    private PdfPCell pdfPCell;
    private MemoryStream memoryStream = new MemoryStream();
    
    public FileOperationsController(ILogger<HomeController> logger,TelephoneBookDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult FileOperations(string param, int pageNumber = 1)
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
    
    public byte[] usersPDF()
    {
        document = new Document(PageSize.A4, 0f,0f,0f,0f);
        document.SetPageSize(PageSize.A4);
        document.SetMargins(10f, 10f, 10f, 10f);
        table.WidthPercentage = 100;
        table.HorizontalAlignment = Element.ALIGN_LEFT;
        fontStyle = FontFactory.GetFont("Times New Roman", 8f, 1);
        PdfWriter.GetInstance(document,memoryStream);
        document.Open();
        table.SetWidths(new float[] {20f, 100f, 100f, 150f, 150f, 150f});
        
        usersPDFHeader();
        usersPDFBody();
        table.HeaderRows = 2;
        document.Add(table);
        document.Close();

        return memoryStream.ToArray();
    }

    public void usersPDFHeader()
    {
        fontStyle = FontFactory.GetFont("Times New Roman", 11f, 1);
        pdfPCell = new PdfPCell(new Phrase("Phonebook",fontStyle));
        pdfPCell.Colspan = column;
        pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
        pdfPCell.Border = 0;
        pdfPCell.BackgroundColor = BaseColor.WHITE;
        pdfPCell.ExtraParagraphSpace = 10;
        table.AddCell(pdfPCell);
        table.CompleteRow();
    }
    
    public void usersPDFBody()
    {
        var users = _context.TelephoneBook.ToList();
        string[] titles = { " ", "Name", "Surname", "Address", "Telephone", "Email" };
        for (int i = 0; i < titles.Length; i++)
        {
            fontStyle = FontFactory.GetFont("Times New Roman", 8f, 1);
            pdfPCell = new PdfPCell(new Phrase(titles[i] ,fontStyle));
            pdfPCell.Colspan = 1;
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(pdfPCell);
        }
        table.CompleteRow();
        fontStyle = FontFactory.GetFont("Times New Roman", 8f, 0);
        int number = 1;
        foreach (TelephoneBookModel user in users)
        {
            pdfPCell = new PdfPCell(new Phrase(number++.ToString(),fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            pdfPCell = new PdfPCell(new Phrase(user.Name, fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            pdfPCell = new PdfPCell(new Phrase(user.Surname, fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            pdfPCell = new PdfPCell(new Phrase(user.Address ,fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            pdfPCell = new PdfPCell(new Phrase(user.Telephone, fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            pdfPCell = new PdfPCell(new Phrase(user.Email, fontStyle));
            pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfPCell.BackgroundColor = BaseColor.WHITE;
            table.AddCell(pdfPCell);
            
            table.CompleteRow();
        }

    }
    
    public byte[] userPDF(int id)
    {
        document = new Document(PageSize.A4, 0f,0f,0f,0f);
        document.SetPageSize(PageSize.A4);
        document.SetMargins(150f, 150f, 10f, 0f);
        table2.WidthPercentage = 100;
        table2.HorizontalAlignment = Element.ALIGN_LEFT;
        fontStyle = FontFactory.GetFont("Times New Roman", 8f, 1);
        PdfWriter.GetInstance(document,memoryStream);
        document.Open();
        table2.SetWidths(new float[] {50f, 150f});

        userPDFHeader(id);
        userPDFBody(id);
        table2.HeaderRows = 2;
        document.Add(table2);
        document.Close();

        return memoryStream.ToArray();
    }
    
    public void userPDFHeader(int id)
    {
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);
        string name = user.Name;
        string title = "Information about " + name;
        fontStyle = FontFactory.GetFont("Times New Roman", 11f, 1);
        pdfPCell = new PdfPCell(new Phrase(title, fontStyle));
        pdfPCell.Colspan = column;
        pdfPCell.HorizontalAlignment = Element.ALIGN_CENTER;
        pdfPCell.Border = 0;
        pdfPCell.BackgroundColor = BaseColor.WHITE;
        pdfPCell.ExtraParagraphSpace = 10;
        table2.AddCell(pdfPCell);
        table2.CompleteRow();
    }
    
    public void userPDFBody(int id)
    {
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);
        string[] userData = { user.Name, user.Surname, user.Address, user.Telephone, user.Email };
        string[] titles = { "Name", "Surname", "Address", "Telephone", "Email" };
        fontStyle = FontFactory.GetFont("Times New Roman", 8f, 1);
        fontStyle2 = FontFactory.GetFont("Times New Roman", 8f, 0);

        for (int i = 0; i < titles.Length; i++)
        {
            // Başlık hücresi oluştur
            PdfPCell titleCell = new PdfPCell(new Phrase(titles[i], fontStyle));
            titleCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            titleCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            titleCell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table2.AddCell(titleCell);

            // Kullanıcı verisi hücresi oluştur
            PdfPCell dataCell = new PdfPCell(new Phrase(userData[i], fontStyle2)); // Kullanıcı nesnesinin uygun özelliğini kullanarak veriyi alın
            dataCell.HorizontalAlignment = Element.ALIGN_LEFT;
            dataCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            dataCell.BackgroundColor = BaseColor.WHITE;
            table2.AddCell(dataCell);
            table2.CompleteRow();
        }
    }
    
    public IActionResult PrintUser(int id)
    {
        var user = _context.TelephoneBook.FirstOrDefault(u => u.ID == id);        
        
        if (user == null)
        {
            return NotFound(); // Kullanıcı bulunamazsa 404 hatası döndür
        }

        byte[] userData = userPDF(id);
        return File(userData, "application/pdf");
    }

    public IActionResult PrintUsers()
    {
        byte[] usersData = usersPDF();
        return File(usersData, "application/pdf");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}