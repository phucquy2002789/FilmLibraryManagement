using AutoMapper;
using FilmLibraryManagement.Dto;
using FilmLibraryManagement.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Movie Mapping
        CreateMap<Movie, MovieDto>().ReverseMap();

        // Genre Mapping
        CreateMap<Genre, GenreDto>().ReverseMap();

        // Director Mapping
        CreateMap<Director, DirectorDto>().ReverseMap();

        // Review Mapping
        CreateMap<Review, ReviewDto>().ReverseMap();
    }
}
