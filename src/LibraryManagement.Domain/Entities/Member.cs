using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.ValueObjects;

namespace LibraryManagement.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; } = new Address();
    public MembershipType MembershipType { get; set; }
    public DateTime MembershipDate { get; set; }
    public DateTime? MembershipExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public int MaxBooksAllowed { get; set; } = 3;
    public string LibraryCardNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    
    public string FullName => $"{FirstName} {LastName}";
    public int Age => DateTime.Now.Year - DateOfBirth.Year;
}
