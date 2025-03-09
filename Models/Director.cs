using System.Collections.Generic;

namespace FilmLibraryManagement.Models
{
    public class Director
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }

        // Navigation property for Many-to-Many relationship
        public ICollection<MovieDirector> MovieDirectors { get; set; }
    }
}
