using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Interfaces;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Application.Services;

public interface IMemberManagementService
{
    Task<MemberDto> RegisterMemberAsync(CreateMemberDto memberDto);
    Task UpgradeMembershipAsync(int memberId, MembershipType newType);
    Task RenewMembershipAsync(int memberId);
    Task DeactivateMemberAsync(int memberId);
    Task ActivateMemberAsync(int memberId);
    Task<MemberStatisticsDto> GetMemberStatisticsAsync(int memberId);
}

public class MemberManagementService : IMemberManagementService
{
    private readonly IMemberRepository _memberRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MemberManagementService(
        IMemberRepository memberRepository,
        ILoanRepository loanRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _loanRepository = loanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> RegisterMemberAsync(CreateMemberDto memberDto)
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

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var createdMember = await _memberRepository.AddAsync(member);
            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(createdMember);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task UpgradeMembershipAsync(int memberId, MembershipType newType)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new Exception($"Member with id {memberId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            member.UpgradeMembership(newType);
            await _memberRepository.UpdateAsync(member);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RenewMembershipAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new Exception($"Member with id {memberId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            member.RenewMembership();
            await _memberRepository.UpdateAsync(member);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task DeactivateMemberAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new Exception($"Member with id {memberId} not found");

        var activeLoans = await _loanRepository.GetActiveLoansByMemberIdAsync(memberId);
        if (activeLoans.Any())
            throw new InvalidOperationException("Cannot deactivate member with active loans");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            member.Deactivate();
            await _memberRepository.UpdateAsync(member);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task ActivateMemberAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new Exception($"Member with id {memberId} not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            member.Activate();
            await _memberRepository.UpdateAsync(member);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<MemberStatisticsDto> GetMemberStatisticsAsync(int memberId)
    {
        var member = await _memberRepository.GetByIdAsync(memberId);
        if (member == null)
            throw new Exception($"Member with id {memberId} not found");

        var allLoans = await _loanRepository.GetAllAsync();
        var memberLoans = allLoans.Where(l => l.MemberId == memberId).ToList();

        return new MemberStatisticsDto
        {
            MemberId = memberId,
            TotalLoans = memberLoans.Count,
            ActiveLoans = member.GetActiveLoansCount(),
            CompletedLoans = memberLoans.Count(l => l.Status == LoanStatus.Returned),
            OverdueLoans = memberLoans.Count(l => l.Status == LoanStatus.Overdue),
            TotalLateFees = memberLoans.Where(l => l.LateFee.HasValue).Sum(l => l.LateFee!.Value),
            CanBorrowBooks = member.CanBorrowBooks(),
            RemainingBorrowCapacity = member.MaxBooksAllowed - member.GetActiveLoansCount()
        };
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
