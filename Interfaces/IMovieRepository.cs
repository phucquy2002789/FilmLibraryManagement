using FilmLibraryManagement.Dto;
using FilmLibraryManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmLibraryManagement.Interfaces
{
    public interface IMovieRepository
    {
        // Pagination
        Task<PagedResult<MovieDto>> GetMoviesAsync(int pageNumber, int pageSize);

        // Movie retrieval
        Task<Movie> GetMovieByIdAsync(int id);
        Task<Movie> GetMovieTrimToUpperAsync(MovieDto movieCreate);
        Task<ICollection<Movie>> SearchMoviesAsync(string query);

        // Movies by relationships
        Task<ICollection<Movie>> GetMoviesByDirectorAsync(int directorId);
        Task<ICollection<Movie>> GetMoviesByGenreAsync(int genreId);
        Task<ICollection<Movie>> GetMoviesByGenreNameAsync(string genreName);
        Task<ICollection<Movie>> GetMoviesByYearAsync(int year);
        Task<ICollection<Movie>> GetMoviesByDirectorNameAsync(string directorName);

        // Trending and best movies
        Task<IEnumerable<Movie>> GetTrendingMoviesAsync();
        Task<ICollection<Movie>> GetBestMoviesByGenreAsync(string genreName);
        Task<ICollection<Movie>> GetBestMoviesByDirectorAsync(string directorName);

        // Movie years
        Task<ICollection<int>> GetMovieYearsAsync();

        // CRUD operations
        Task<bool> AddMovieAsync(Movie movie, int genreId, int directorId);
        Task<bool> UpdateMovieAsync(Movie movie);
        Task<bool> DeleteMovieAsync(Movie movie);
    }
}
