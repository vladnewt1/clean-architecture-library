using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Application.Mappings;

/// <summary>
/// AutoMapper profile for mapping between Member entity and MemberDto
/// </summary>
public class MemberProfile : Profile
{
    public MemberProfile()
    {
        // Member -> MemberDto
        CreateMap<Member, MemberDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        
        // Address -> AddressDto
        CreateMap<Address, AddressDto>();
        
        // AddressDto -> Address
        CreateMap<AddressDto, Address>()
            .ConstructUsing(dto => Address.Create(dto.Street, dto.City, dto.State, dto.ZipCode, dto.Country));
        
        // CreateMemberDto -> Member (використовуємо фабричний метод)
        CreateMap<CreateMemberDto, Member>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ConstructUsing((dto, context) =>
            {
                var address = context.Mapper.Map<Address>(dto.Address);
                return Member.Create(
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.DateOfBirth,
                    address,
                    dto.MembershipType
                );
            });
        
        // Member -> MemberStatisticsDto
        CreateMap<Member, MemberStatisticsDto>()
            .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalLoans, opt => opt.MapFrom(src => src.Loans.Count))
            .ForMember(dest => dest.ActiveLoans, opt => opt.MapFrom(src => src.GetActiveLoansCount()))
            .ForMember(dest => dest.CompletedLoans, opt => opt.MapFrom(src => src.Loans.Count(l => l.ReturnDate.HasValue)))
            .ForMember(dest => dest.OverdueLoans, opt => opt.MapFrom(src => src.Loans.Count(l => l.IsOverdue)))
            .ForMember(dest => dest.TotalLateFees, opt => opt.MapFrom(src => src.Loans.Where(l => l.LateFee.HasValue).Sum(l => l.LateFee!.Value)))
            .ForMember(dest => dest.CanBorrowBooks, opt => opt.MapFrom(src => src.CanBorrowBooks()))
            .ForMember(dest => dest.RemainingBorrowCapacity, opt => opt.MapFrom(src => 10 - src.GetActiveLoansCount()));
    }
}
