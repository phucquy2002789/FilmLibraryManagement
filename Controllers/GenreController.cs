using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using FilmLibraryManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GenreController(IGenreRepository genreRepository, IMapper mapper)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetGenres()
    {
        try
        {
            var genres = await _genreRepository.GetGenresAsync();
            return Ok(_mapper.Map<List<GenreDto>>(genres));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGenreById(int id)
    {
        try
        {
            var genre = await _genreRepository.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound("Genre not found.");

            return Ok(_mapper.Map<GenreDto>(genre));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("movie/{movieid}")]
    public async Task<IActionResult> GetGenreByMovie(int movieid)
    {
        try
        {
            var genres = await _genreRepository.GetGenresByMovieAsync(movieid);
            if (genres == null || !genres.Any())
                return NotFound("No genres found for the movie.");

            return Ok(_mapper.Map<List<GenreDto>>(genres));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateGenre([FromBody] GenreDto genreCreate)
    {
        try
        {
            if (genreCreate == null)
                return BadRequest("Invalid genre data.");

            var existingGenre = (await _genreRepository.GetGenresAsync())
                .FirstOrDefault(g => g.Name.Trim().ToUpper() == genreCreate.Name.Trim().ToUpper());

            if (existingGenre != null)
                return Conflict("Genre already exists.");

            var genre = _mapper.Map<Genre>(genreCreate);
            if (!await _genreRepository.AddGenreAsync(genre))
                return StatusCode(500, "An error occurred while saving.");

            return Ok("Successfully created genre.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreDto genreDto)
    {
        try
        {
            if (genreDto == null)
                return BadRequest("Invalid genre data.");

            var genre = await _genreRepository.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound("Genre not found.");

            genreDto.Id = id;
            _mapper.Map(genreDto, genre);

            if (!await _genreRepository.UpdateGenreAsync(genre))
                return StatusCode(500, "An error occurred while updating.");

            return Ok("Successfully updated genre.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        try
        {
            var genre = await _genreRepository.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound("Genre not found.");

            if (!await _genreRepository.DeleteGenreAsync(genre))
                return StatusCode(500, "An error occurred while deleting.");

            return Ok("Successfully deleted genre.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
