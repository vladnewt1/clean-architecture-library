using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Book entity and BookDto
/// </summary>
public class BookProfile : Profile
{
    public BookProfile()
    {
        // Book -> BookDto
        CreateMap<Book, BookDto>();
        
        // CreateBookDto -> Book
        CreateMap<CreateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableCopies, opt => opt.MapFrom(src => src.TotalCopies));
        
        // UpdateBookDto -> Book
        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
