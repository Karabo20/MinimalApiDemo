namespace MinimalApiDemo.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required decimal Budget { get; set; }
        public required int Year { get; set; }

    }
}
