using FilmLibraryManagement.Data;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DirectorRepository : IDirectorRepository
{
    private readonly DataContext _context;

    public DirectorRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Director>> GetDirectorsAsync()
    {
        try
        {
            return await _context.Directors.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving directors", ex);
        }
    }

    public async Task<Director?> GetDirectorByIdAsync(int id)
    {
        try
        {
            return await _context.Directors.FirstOrDefaultAsync(d => d.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving director by ID", ex);
        }
    }

    public async Task<List<Director>> GetDirectorsByMovieAsync(int movieId)
    {
        try
        {
            return await _context.MovieDirectors
                .Where(md => md.MovieId == movieId)
                .Select(md => md.Director)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving directors by movie", ex);
        }
    }

    public async Task<bool> AddDirectorAsync(Director director)
    {
        try
        {
            await _context.Directors.AddAsync(director);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding director", ex);
        }
    }

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

    public async Task<bool> UpdateDirectorAsync(Director director)
    {
        try
        {
            _context.Directors.Update(director);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error updating director", ex);
        }
    }

    public async Task<bool> DeleteDirectorAsync(Director director)
    {
        try
        {
            _context.Directors.Remove(director);
            return await SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error deleting director", ex);
        }
    }
}