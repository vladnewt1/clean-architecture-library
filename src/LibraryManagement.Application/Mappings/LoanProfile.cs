using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Loan entity and LoanDto
/// </summary>
public class LoanProfile : Profile
{
    public LoanProfile()
    {
        // Loan -> LoanDto
        CreateMap<Loan, LoanDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book != null ? src.Book.Title : null))
            .ForMember(dest => dest.MemberName, opt => opt.MapFrom(src => src.Member != null ? src.Member.FullName : null));
        
        // CreateLoanDto -> Loan (використовуємо фабричний метод)
        CreateMap<CreateLoanDto, Loan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ConstructUsing(dto => Loan.Create(dto.BookId, dto.MemberId, dto.Notes ?? string.Empty));
    }
}
