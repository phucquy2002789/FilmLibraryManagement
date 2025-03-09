using FilmLibraryManagement.Data;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReviewRepository : IReviewRepository
{
    private readonly DataContext _context;
    private readonly IMovieRepository _movieRepository; // Injected movie repository

    public ReviewRepository(DataContext context, IMovieRepository movieRepository)
    {
        _context = context;
        _movieRepository = movieRepository;
    }

    // Retrieve all reviews
    public async Task<ICollection<Review>> GetReviewsAsync()
    {
        try
        {
            return await _context.Reviews.ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving reviews: {ex.Message}");
            return new List<Review>();
        }
    }

    // Retrieve a review by ID
    public async Task<Review> GetReviewByIdAsync(int id)
    {
        try
        {
            return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving review by ID: {ex.Message}");
            return null;
        }
    }

    // Retrieve reviews by movie ID
    public async Task<ICollection<Review>> GetReviewsByMovieAsync(int movieId)
    {
        try
        {
            return await _context.Reviews.Where(r => r.MovieId == movieId).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving reviews by movie ID: {ex.Message}");
            return new List<Review>();
        }
    }

    // Add a new review
    public async Task<bool> AddReviewAsync(Review review)
    {
        try
        {
            var movie = await _movieRepository.GetMovieByIdAsync(review.MovieId);
            if (movie == null)
                return false; // Prevent foreign key error

            await _context.Reviews.AddAsync(review);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding review: {ex.Message}");
            return false;
        }
    }

    // Update an existing review
    public async Task<bool> UpdateReviewAsync(Review review)
    {
        try
        {
            _context.Reviews.Update(review);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating review: {ex.Message}");
            return false;
        }
    }

    // Delete a review
    public async Task<bool> DeleteReviewAsync(Review review)
    {
        try
        {
            _context.Reviews.Remove(review);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting review: {ex.Message}");
            return false;
        }
    }
}