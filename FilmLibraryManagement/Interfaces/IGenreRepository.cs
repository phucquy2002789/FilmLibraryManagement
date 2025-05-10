using FilmLibraryManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IGenreRepository
{
    // Retrieve all genres
    Task<List<Genre>> GetGenresAsync();

    // Retrieve a specific genre by its ID
    Task<Genre?> GetGenreByIdAsync(int id);

    // Retrieve genres associated with a specific movie
    Task<List<Genre>> GetGenresByMovieAsync(int movieId);

    // Add a new genre
    Task<bool> AddGenreAsync(Genre genre);

    // Update an existing genre
    Task<bool> UpdateGenreAsync(Genre genre);

    // Delete a genre
    Task<bool> DeleteGenreAsync(Genre genre);

    // Save changes to the database
    Task<bool> SaveAsync();
}
