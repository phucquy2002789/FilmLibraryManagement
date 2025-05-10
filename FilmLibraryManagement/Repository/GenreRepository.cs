using FilmLibraryManagement.Data;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class GenreRepository : IGenreRepository
{
    private readonly DataContext _context;

    public GenreRepository(DataContext context)
    {
        _context = context;
    }

    // Retrieve all genres
    public async Task<List<Genre>> GetGenresAsync()
    {
        try
        {
            return await _context.Genres.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving genres", ex);
        }
    }

    // Retrieve a genre by ID
    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        try
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving genre by ID", ex);
        }
    }

    // Retrieve genres associated with a movie
    public async Task<List<Genre>> GetGenresByMovieAsync(int movieId)
    {
        try
        {
            return await _context.MovieGenres
                .Where(mg => mg.MovieId == movieId)
                .Select(mg => mg.Genre)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving genres by movie", ex);
        }
    }

    // Add a new genre
    public async Task<bool> AddGenreAsync(Genre genre)
    {
        try
        {
            await _context.Genres.AddAsync(genre);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding genre", ex);
        }
    }

    // Update an existing genre
    public async Task<bool> UpdateGenreAsync(Genre genre)
    {
        try
        {
            _context.Genres.Update(genre);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating genre", ex);
        }
    }

    // Delete a genre
    public async Task<bool> DeleteGenreAsync(Genre genre)
    {
        try
        {
            _context.Genres.Remove(genre);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting genre", ex);
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
            throw new Exception("Error saving changes", ex);
        }
    }
}
