using System.ComponentModel.DataAnnotations;

namespace EduTech.Models;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }
    public Class Class { get; set; }

    [Required]
    public string StudentId { get; set; } 
    public ApplicationUser Student { get; set; }

    [Required]
    public double Amount { get; set; }

    [Required]
    public InvoiceStatus Status { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;

}

public enum InvoiceStatus
{
    Unpaid,
    Paid
}