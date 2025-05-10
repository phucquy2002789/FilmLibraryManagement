using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using FilmLibraryManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace FilmLibraryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieController(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        [HttpGet("movies/{pageNumber}/{pageSize}")]
        
        public async Task<IActionResult> GetMovies(int pageNumber, int pageSize)
        {
            try
            {
                var movies = await _movieRepository.GetMoviesAsync(pageNumber, pageSize);
                return Ok(_mapper.Map<List<MovieDto>>(movies.Items));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieByIdAsync(id);
                if (movie == null) return NotFound("Movie not found.");

                return Ok(_mapper.Map<MovieDto>(movie));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> GetRecommendation([FromQuery] int userId, [FromQuery] int movieId)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://localhost:5014/api/recommendation?userId={userId}&movieId={movieId}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error fetching recommendations.");
                }

                string result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
        }
        [HttpGet("checkdb")]
        public async Task<IActionResult> CheckDatabase()
        {
            using (HttpClient client = new HttpClient())
            {
                // Change the URL to use the same API that is working
                string apiUrl = "https://localhost:7038/api/checkdb";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "Error fetching data from API.");
                }

                string result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
        }





        [HttpGet("byGenreName/{genreName}")]
        public async Task<IActionResult> GetMoviesByGenreName(string genreName)
        {
            try
            {
                var movies = await _movieRepository.GetMoviesByGenreNameAsync(genreName);
                if (!movies.Any()) return NotFound($"No movies found for genre: {genreName}");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("byYear/{year}")]
        public async Task<IActionResult> GetMoviesByYear(int year)
        {
            try
            {
                var movies = await _movieRepository.GetMoviesByYearAsync(year);
                if (!movies.Any()) return NotFound("No movies found for this year.");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("byDirectorName/{directorName}")]
        public async Task<IActionResult> GetMoviesByDirectorName(string directorName)
        {
            try
            {
                var movies = await _movieRepository.GetMoviesByDirectorNameAsync(directorName);
                if (!movies.Any()) return NotFound($"No movies found for director: {directorName}");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("years")]
        public async Task<IActionResult> GetMovieYears()
        {
            try
            {
                var years = await _movieRepository.GetMovieYearsAsync();
                if (!years.Any()) return NotFound("No movie years found.");

                return Ok(years);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromQuery] int genreId, [FromQuery] int directorId, [FromBody] MovieDto movieCreate)
        {
            try
            {
                if (movieCreate == null) return BadRequest("Invalid movie data.");

                var existingMovie = await _movieRepository.GetMovieTrimToUpperAsync(movieCreate);
                if (existingMovie != null) return Conflict("Movie already exists.");

                var movie = _mapper.Map<Movie>(movieCreate);
                if (!await _movieRepository.AddMovieAsync(movie, genreId, directorId))
                    return StatusCode(500, "An error occurred while saving the movie.");

                return Ok("Successfully created the movie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieDto movieDto)
        {
            try
            {
                if (movieDto == null) return BadRequest("Invalid movie data.");

                var movie = await _movieRepository.GetMovieByIdAsync(id);
                if (movie == null) return NotFound("Movie not found.");

                _mapper.Map(movieDto, movie);
                if (!await _movieRepository.UpdateMovieAsync(movie))
                    return StatusCode(500, "An error occurred while updating.");

                return Ok("Successfully updated movie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            try
            {
                var movie = await _movieRepository.GetMovieByIdAsync(id);
                if (movie == null) return NotFound("Movie not found.");

                if (!await _movieRepository.DeleteMovieAsync(movie))
                    return StatusCode(500, "An error occurred while deleting.");

                return Ok("Successfully deleted movie.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingMovies()
        {
            try
            {
                var trendingMovies = await _movieRepository.GetTrendingMoviesAsync();
                if (!trendingMovies.Any()) return NotFound("No trending movies found.");

                return Ok(trendingMovies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("bestByGenre/{genreName}")]
        public async Task<IActionResult> GetBestMoviesByGenre(string genreName)
        {
            try
            {
                var movies = await _movieRepository.GetBestMoviesByGenreAsync(genreName);
                if (!movies.Any()) return NotFound($"No best movies found for genre: {genreName}");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("bestByDirector/{directorName}")]
        public async Task<IActionResult> GetBestMoviesByDirector(string directorName)
        {
            try
            {
                var movies = await _movieRepository.GetBestMoviesByDirectorAsync(directorName);
                if (!movies.Any()) return NotFound($"No best movies found for director: {directorName}");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            try
            {
                var movies = await _movieRepository.SearchMoviesAsync(query);
                if (!movies.Any()) return NotFound("No movies found matching the query.");

                return Ok(movies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
