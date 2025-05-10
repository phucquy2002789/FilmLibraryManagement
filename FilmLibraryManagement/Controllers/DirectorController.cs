using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using FilmLibraryManagement.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class DirectorController : ControllerBase
{
    private readonly IDirectorRepository _directorRepository;
    private readonly IMapper _mapper;

    public DirectorController(IDirectorRepository directorRepository, IMapper mapper)
    {
        _directorRepository = directorRepository;
        _mapper = mapper;
    }

    
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]

    public async Task<IActionResult> GetDirectors()
    {
        try
        {
            var directors = await _directorRepository.GetDirectorsAsync();
            var directorDtos = _mapper.Map<List<DirectorDto>>(directors);
            return Ok(directorDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDirectorById(int id)
    {
        try
        {
            var director = await _directorRepository.GetDirectorByIdAsync(id);
            if (director == null) return NotFound("Director not found.");

            var directorDto = _mapper.Map<DirectorDto>(director);
            return Ok(directorDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetDirectorsByMovie(int movieId)
    {
        try
        {
            var directors = await _directorRepository.GetDirectorsByMovieAsync(movieId);
            if (directors == null || directors.Count == 0) return NotFound("No directors found for the movie.");

            var directorDtos = _mapper.Map<List<DirectorDto>>(directors);
            return Ok(directorDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateDirector([FromBody] DirectorDto directorCreate)
    {
        try
        {
            if (directorCreate == null) return BadRequest("Invalid director data.");

            var existingDirector = (await _directorRepository.GetDirectorsAsync())
                .FirstOrDefault(d => d.Name.Trim().ToUpper() == directorCreate.Name.Trim().ToUpper());

            if (existingDirector != null) return Conflict("Director already exists.");

            var director = _mapper.Map<Director>(directorCreate);
            if (!await _directorRepository.AddDirectorAsync(director)) return StatusCode(500, "An error occurred while saving.");

            return Ok("Successfully created director.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDirector(int id, [FromBody] DirectorDto directorDto)
    {
        try
        {
            if (directorDto == null) return BadRequest("Invalid director data.");

            var director = await _directorRepository.GetDirectorByIdAsync(id);
            if (director == null) return NotFound("Director not found.");

            directorDto.Id = id;
            _mapper.Map(directorDto, director);

            if (!await _directorRepository.UpdateDirectorAsync(director)) return StatusCode(500, "An error occurred while updating.");

            return Ok("Successfully updated director.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDirector(int id)
    {
        try
        {
            var director = await _directorRepository.GetDirectorByIdAsync(id);
            if (director == null) return NotFound("Director not found.");

            if (!await _directorRepository.DeleteDirectorAsync(director)) return StatusCode(500, "An error occurred while deleting.");

            return Ok("Successfully deleted director.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }
}
