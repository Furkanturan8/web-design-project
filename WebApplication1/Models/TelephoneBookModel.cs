using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public class TelephoneBookModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID  { get; set; }
   
    
    [Required(ErrorMessage = "Name Field Required!")]
    [RegularExpression(@"^[A-Za-zğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Invalid Characters")]
    public string Name { get; set; }
    
    
    [Required(ErrorMessage = "Surname Field Required!")]
    [RegularExpression(@"^[A-Za-zğüşıöçĞÜŞİÖÇ\s]+$", ErrorMessage = "Invalid Characters")]
    public string Surname { get; set; }
    
    
    [Required(ErrorMessage = "Telephone Number Field Required!")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Invalid Telephone Number")]
    public string Telephone { get; set; }
    
    
    [Required(ErrorMessage = "Address Field Required!")]
    public string Address  { get; set; }
    
    
    [Required(ErrorMessage = "Email Field Required!")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
}