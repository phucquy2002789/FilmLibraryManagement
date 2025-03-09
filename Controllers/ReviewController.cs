using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmLibraryManagement.Interfaces;
using FilmLibraryManagement.Models;
using FilmLibraryManagement.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IMapper _mapper;

    public ReviewController(IReviewRepository reviewRepository, IMovieRepository movieRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        try
        {
            var reviews = await _reviewRepository.GetReviewsAsync();
            return Ok(_mapper.Map<List<ReviewDto>>(reviews));
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        try
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return NotFound("Review not found.");

            return Ok(_mapper.Map<ReviewDto>(review));
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpGet("byMovieId/{movieId}")]
    public async Task<IActionResult> GetReviewsByMovie(int movieId)
    {
        try
        {
            var reviews = await _reviewRepository.GetReviewsByMovieAsync(movieId);
            if (reviews == null || reviews.Count == 0) return NotFound("No reviews found for this movie.");

            return Ok(_mapper.Map<List<ReviewDto>>(reviews));
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromQuery] int movieId, [FromBody] ReviewDto reviewDto)
    {
        try
        {
            if (reviewDto == null) return BadRequest("Invalid review data.");

            var movie = await _movieRepository.GetMovieByIdAsync(movieId);
            if (movie == null) return NotFound("Movie not found.");

            var review = _mapper.Map<Review>(reviewDto);
            review.MovieId = movieId;

            if (!await _reviewRepository.AddReviewAsync(review))
                return StatusCode(500, "An error occurred while saving the review.");

            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, _mapper.Map<ReviewDto>(review));
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewDto reviewDto)
    {
        try
        {
            if (reviewDto == null) return BadRequest("Invalid review data.");

            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return NotFound("Review not found.");

            _mapper.Map(reviewDto, review);

            if (!await _reviewRepository.UpdateReviewAsync(review))
                return StatusCode(500, "An error occurred while updating the review.");

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        try
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return NotFound("Review not found.");

            if (!await _reviewRepository.DeleteReviewAsync(review))
                return StatusCode(500, "An error occurred while deleting the review.");

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "An unexpected error occurred. Please try again later.");
        }
    }
}
