using System;
using System.ComponentModel.DataAnnotations;

namespace EduTech.Models.ViewModel;

public class StudentViewModel
{
    public string? Id { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? Password { get; set; } = string.Empty;

    public string UserType { get; set; } = string.Empty;
}
