using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTech.Models.ViewModel;

public class MergeClassesViewModel
{
    public int ClassAId { get; set; }
    public int ClassBId { get; set; }
    public List<SelectListItem> Classes { get; set; }
}