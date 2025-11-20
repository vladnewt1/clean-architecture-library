using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<MemberDto?> GetByIdAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<IEnumerable<MemberDto>> GetAllAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(MapToDto);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto memberDto)
    {
        var address = Address.Create(
            memberDto.Address.Street,
            memberDto.Address.City,
            memberDto.Address.State,
            memberDto.Address.ZipCode,
            memberDto.Address.Country
        );

        var member = Member.Create(
            memberDto.FirstName,
            memberDto.LastName,
            memberDto.Email,
            memberDto.PhoneNumber,
            memberDto.DateOfBirth,
            address,
            memberDto.MembershipType
        );

        var createdMember = await _memberRepository.AddAsync(member);
        return MapToDto(createdMember);
    }

    public async Task UpdateAsync(int id, UpdateMemberDto memberDto)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
            throw new Exception($"Member with id {id} not found");

        var address = Address.Create(
            memberDto.Address.Street,
            memberDto.Address.City,
            memberDto.Address.State,
            memberDto.Address.ZipCode,
            memberDto.Address.Country
        );

        member.UpdatePersonalInfo(
            memberDto.FirstName,
            memberDto.LastName,
            member.Email,
            memberDto.PhoneNumber,
            member.DateOfBirth,
            address
        );

        if (!memberDto.IsActive && member.IsActive)
            member.Deactivate();
        else if (memberDto.IsActive && !member.IsActive)
            member.Activate();

        await _memberRepository.UpdateAsync(member);
    }

    public async Task DeleteAsync(int id)
    {
        await _memberRepository.DeleteAsync(id);
    }

    public async Task<MemberDto?> GetByEmailAsync(string email)
    {
        var member = await _memberRepository.GetByEmailAsync(email);
        return member == null ? null : MapToDto(member);
    }

    public async Task<IEnumerable<MemberDto>> GetActiveMembersAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Where(m => m.IsActive).Select(MapToDto);
    }

    private static MemberDto MapToDto(Member member)
    {
        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            PhoneNumber = member.PhoneNumber,
            DateOfBirth = member.DateOfBirth,
            Address = new AddressDto
            {
                Street = member.Address.Street,
                City = member.Address.City,
                State = member.Address.State,
                ZipCode = member.Address.ZipCode,
                Country = member.Address.Country
            },
            MembershipType = member.MembershipType,
            MembershipDate = member.MembershipDate,
            MembershipExpiryDate = member.MembershipExpiryDate,
            IsActive = member.IsActive,
            MaxBooksAllowed = member.MaxBooksAllowed,
            LibraryCardNumber = member.LibraryCardNumber,
            FullName = member.FullName,
            Age = member.Age
        };
    }

}
