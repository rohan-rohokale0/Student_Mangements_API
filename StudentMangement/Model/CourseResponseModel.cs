namespace StudentMangement.Model
{
    public class CourseResponseModel
    {
        public Guid Id { get; set; }
        public string? CourseName { get; set; }
        public string? TeacherName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CourseImage { get; set; }
        public Guid? categoryId { get; set; }
        public string? categoryName { get; set; }
        public decimal? price { get; set; } 
    }
}
