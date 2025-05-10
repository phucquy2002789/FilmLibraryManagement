namespace FilmLibraryManagement.Services
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using FilmLibraryManagement.Data;
    using FilmLibraryManagement.Models;

    public class MovieRecommendationService
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _mlApiUrl = "http://localhost:5014/api/recommendation"; // Change if needed

        public MovieRecommendationService(DataContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task PredictAndStoreRatings(int userId)
        {
            var movies = await _context.Movies.ToListAsync();
            foreach (var movie in movies)
            {
                var rating = await GetPredictedRating(userId, movie.Id);
                if (rating.HasValue)
                {
                    movie.Rating = rating.Value;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<double?> GetPredictedRating(int userId, int movieId)
        {
            var requestUrl = $"{_mlApiUrl}?userId={userId}&movieId={movieId}";
            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode) return null;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var prediction = JsonSerializer.Deserialize<PredictionResponse>(jsonResponse);

                return prediction?.rating;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error predicting rating for Movie {movieId}: {ex.Message}");
                return null;
            }
        }
    }

    public class PredictionResponse
    {
        public int userId { get; set; }
        public int movieId { get; set; }
        public double rating { get; set; }
    }

}
