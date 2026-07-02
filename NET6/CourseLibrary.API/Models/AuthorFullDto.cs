namespace CourseLibrary.API.Models
{
    public class AuthorFullDto
    {
        public Guid guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public string MainCategory { get; set; }
    }
}
