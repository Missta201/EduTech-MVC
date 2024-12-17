namespace EduTech.Models.ViewModel;

public class ExamResultsViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public List<StudentGrade> Grades { get; set; } = [];

}