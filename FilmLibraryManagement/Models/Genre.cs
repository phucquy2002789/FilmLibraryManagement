using System.Collections.Generic;

namespace FilmLibraryManagement.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property for Many-to-Many relationship
        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
