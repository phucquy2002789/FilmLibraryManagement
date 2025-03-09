using FilmLibraryManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IDirectorRepository
{
    // Retrieve all directors
    Task<List<Director>> GetDirectorsAsync();

    // Retrieve a specific director by their ID
    Task<Director?> GetDirectorByIdAsync(int id);

    // Retrieve directors associated with a specific movie
    Task<List<Director>> GetDirectorsByMovieAsync(int movieId);

    // Add a new director
    Task<bool> AddDirectorAsync(Director director);

    // Update an existing director
    Task<bool> UpdateDirectorAsync(Director director);

    // Delete a director
    Task<bool> DeleteDirectorAsync(Director director);

    // Save changes to the database
    Task<bool> SaveAsync();
}
