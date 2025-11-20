using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Events;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities;

public class Member : AggregateRoot<int>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Address Address { get; private set; }
    public MembershipType MembershipType { get; private set; }
    public DateTime MembershipDate { get; private set; }
    public DateTime? MembershipExpiryDate { get; private set; }
    public bool IsActive { get; private set; }
    public int MaxBooksAllowed { get; private set; }
    public string LibraryCardNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Navigation properties
    private readonly List<Loan> _loans = new();
    public IReadOnlyCollection<Loan> Loans => _loans.AsReadOnly();
    
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.Now.Year - DateOfBirth.Year;

    // Private constructor for EF Core
    private Member() 
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        Address = null!;
        LibraryCardNumber = string.Empty;
    }

    private Member(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        DateTime dateOfBirth,
        Address address,
        MembershipType membershipType)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Address = address;
        MembershipType = membershipType;
        MembershipDate = DateTime.UtcNow;
        MembershipExpiryDate = DateTime.UtcNow.AddYears(1);
        IsActive = true;
        MaxBooksAllowed = CalculateMaxBooksAllowed(membershipType);
        LibraryCardNumber = GenerateLibraryCardNumber();
        CreatedAt = DateTime.UtcNow;
    }

    public static Member Create(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        DateTime dateOfBirth,
        Address address,
        MembershipType membershipType)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (address == null)
            throw new ArgumentNullException(nameof(address));

        var member = new Member(firstName, lastName, email, phoneNumber, dateOfBirth, address, membershipType);
        
        return member;
    }

    public void RegisterMember()
    {
        // Викликаємо подію ПІСЛЯ того як Id встановлено (після збереження в БД)
        RaiseDomainEvent(new MemberRegisteredEvent(Id, LibraryCardNumber, DateTime.UtcNow));
    }

    public void UpdatePersonalInfo(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        DateTime dateOfBirth,
        Address address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpgradeMembership(MembershipType newMembershipType)
    {
        if (newMembershipType <= MembershipType)
            throw new InvalidOperationException("New membership type must be higher than current");

        MembershipType = newMembershipType;
        MaxBooksAllowed = CalculateMaxBooksAllowed(newMembershipType);
        MembershipExpiryDate = DateTime.UtcNow.AddYears(1);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RenewMembership()
    {
        if (!IsActive)
            throw new InvalidOperationException("Cannot renew inactive membership");

        MembershipExpiryDate = DateTime.UtcNow.AddYears(1);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanBorrowBooks()
    {
        if (!IsActive)
            return false;

        if (MembershipExpiryDate.HasValue && MembershipExpiryDate.Value < DateTime.UtcNow)
            return false;

        var activeLoansCount = _loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue);
        return activeLoansCount < MaxBooksAllowed;
    }

    public int GetActiveLoansCount()
    {
        return _loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue);
    }

    private static int CalculateMaxBooksAllowed(MembershipType membershipType)
    {
        return membershipType switch
        {
            MembershipType.Standard => 3,
            MembershipType.Premium => 5,
            MembershipType.VIP => 10,
            _ => 3
        };
    }

    private static string GenerateLibraryCardNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmm");
        var random = new Random().Next(1000, 9999);
        return $"LIB{timestamp}{random}";
    }
}
