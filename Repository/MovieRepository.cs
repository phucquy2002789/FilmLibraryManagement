using FilmLibraryManagement.Data;
using FilmLibraryManagement.Dto;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace FilmLibraryManagement.Repository
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MovieRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Retrieve all movies
        public async Task<ICollection<Movie>> GetMoviesAsync()
        {
            try
            {
                return await _context.Movies
                    .OrderBy(m => m.Id)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error fetching movies: {ex.Message}");
                return new List<Movie>();
            }
        }

        // Retrieve a paginated list of movies
        public async Task<PagedResult<MovieDto>> GetMoviesAsync(int pageNumber, int pageSize)
        {
            try
            {
                var totalMovies = await _context.Movies.CountAsync();

                var movies = await _context.Movies
                    .OrderBy(m => m.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<MovieDto>
                {
                    Items = _mapper.Map<List<MovieDto>>(movies),
                    TotalCount = totalMovies,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching paginated movies: {ex.Message}");
                return new PagedResult<MovieDto> { Items = new List<MovieDto>(), TotalCount = 0, PageNumber = pageNumber, PageSize = pageSize };
            }
        }

        // Retrieve a movie by ID
        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            try
            {
                return await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching movie by ID: {ex.Message}");
                return null;
            }
        }

        // Retrieve movies by director ID
        public async Task<ICollection<Movie>> GetMoviesByDirectorAsync(int directorId)
        {
            try
            {
                return await _context.MovieDirectors
                    .Where(md => md.DirectorId == directorId)
                    .Select(md => md.Movie)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching movies by director ID: {ex.Message}");
                return new List<Movie>();
            }
        }

        // Retrieve movies by director name
        public async Task<ICollection<Movie>> GetMoviesByDirectorNameAsync(string directorName)
        {
            try
            {
                return await _context.MovieDirectors
                    .Where(md => md.Director.Name == directorName)
                    .Select(md => md.Movie)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching movies by director name: {ex.Message}");
                return new List<Movie>();
            }
        }

        // Retrieve movies by genre ID
        public async Task<ICollection<Movie>> GetMoviesByGenreAsync(int genreId)
        {
            try
            {
                return await _context.MovieGenres
                    .Where(mg => mg.GenreId == genreId)
                    .Select(mg => mg.Movie)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                return new List<Movie>();
            }
        }

        // Retrieve movies by genre name
        public async Task<ICollection<Movie>> GetMoviesByGenreNameAsync(string genreName)
        {
            try
            {
                return await _context.MovieGenres
                    .Where(mg => mg.Genre.Name == genreName)
                    .Select(mg => mg.Movie)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Movie>();
            }
        }

        // Retrieve movies by release year
        public async Task<ICollection<Movie>> GetMoviesByYearAsync(int year)
        {
            try
            {
                return await _context.Movies
                    .Where(m => m.ReleaseYear == year)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Movie>();
            }
        }

        // Retrieve all distinct movie release years
        public async Task<ICollection<int>> GetMovieYearsAsync()
        {
            try
            {
                return await _context.Movies
                    .Select(m => m.ReleaseYear)
                    .Distinct()
                    .OrderBy(y => y)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<int>();
            }
        }

        // Retrieve trending movies based on rating
        public async Task<IEnumerable<Movie>> GetTrendingMoviesAsync()
        {
            try
            {
                return await _context.Movies
                    .OrderByDescending(m => m.Rating)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Movie>();
            }
        }

        // Retrieve best-rated movies by genre
        public async Task<ICollection<Movie>> GetBestMoviesByGenreAsync(string genreName)
        {
            try
            {
                return await _context.MovieGenres
                    .Where(mg => mg.Genre.Name == genreName)
                    .GroupBy(mg => mg.Movie.Id)
                    .OrderByDescending(g => g.Average(mg => mg.Movie.Rating))
                    .Select(g => g.First().Movie)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Movie>();
            }
        }

        // Retrieve best-rated movies by director
        public async Task<ICollection<Movie>> GetBestMoviesByDirectorAsync(string directorName)
        {
            try
            {
                return await _context.MovieDirectors
                    .Where(md => md.Director.Name == directorName)
                    .GroupBy(md => md.Movie.Id)
                    .OrderByDescending(g => g.Average(md => md.Movie.Rating))
                    .Select(g => g.First().Movie)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Movie>();
            }
        }

        public async Task<ICollection<Movie>> SearchMoviesAsync(string query)
        {
            try
            {
                query = query.ToLower();

                return await _context.Movies
                    .Where(m => m.Title.ToLower().Contains(query) ||
                                m.MovieDirectors.Any(md => md.Director.Name.ToLower().Contains(query)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching movies: {ex.Message}");
                return new List<Movie>();
            }
        }

        // Add a new movie along with its genre and director
        public async Task<bool> AddMovieAsync(Movie movie, int genreId, int directorId)
        {
            try
            {
                await _context.Movies.AddAsync(movie);
                await _context.SaveChangesAsync();

                var movieGenre = new MovieGenre { MovieId = movie.Id, GenreId = genreId };
                var movieDirector = new MovieDirector { MovieId = movie.Id, DirectorId = directorId };

                await _context.MovieGenres.AddAsync(movieGenre);
                await _context.MovieDirectors.AddAsync(movieDirector);

                return await SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Update an existing movie
        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            try
            {
                _context.Movies.Update(movie);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Delete an existing movie
        public async Task<bool> DeleteMovieAsync(Movie movie)
        {
            try
            {
                _context.Movies.Remove(movie);
                return await SaveAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Retrieve a movie by title (trimmed and uppercase for case-insensitive comparison)
        public async Task<Movie> GetMovieTrimToUpperAsync(MovieDto movieCreate)
        {
            try
            {
                return await _context.Movies
                    .Where(m => m.Title.Trim().ToUpper() == movieCreate.Title.Trim().ToUpper())
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Save changes to the database
        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
