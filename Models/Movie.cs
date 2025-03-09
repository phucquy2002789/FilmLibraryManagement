using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace FilmLibraryManagement.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public double Rating { get; set; }

        // Navigation properties for Many-to-Many relationships
        public ICollection<MovieGenre> MovieGenres { get; set; }
        public ICollection<MovieDirector> MovieDirectors { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
