using System.Collections.Generic;
using System.Threading.Tasks;
using FilmLibraryManagement.Models;

public interface IReviewRepository
{
    // Review retrieval
    Task<ICollection<Review>> GetReviewsAsync();
    Task<Review> GetReviewByIdAsync(int id);
    Task<ICollection<Review>> GetReviewsByMovieAsync(int movieId);

    // CRUD operations
    Task<bool> AddReviewAsync(Review review);
    Task<bool> UpdateReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(Review review);
}
