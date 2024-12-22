using Microsoft.AspNetCore.Mvc.Rendering;

namespace EduTech.ViewModels
{
    public class SwitchClassViewModel
    {
        public int CurrentClassId { get; set; }
        public int NewClassId { get; set; }
        public string SelectedStudentName { get; set; }
        public List<SelectListItem> Classes { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Students { get; set; } = new List<SelectListItem>();
    }
}
