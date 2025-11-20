using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public AddressDto Address { get; set; } = new AddressDto();
    public MembershipType MembershipType { get; set; }
    public DateTime MembershipDate { get; set; }
    public DateTime? MembershipExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public int MaxBooksAllowed { get; set; }
    public string LibraryCardNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
}

public class CreateMemberDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public AddressDto Address { get; set; } = new AddressDto();
    public MembershipType MembershipType { get; set; }
}

public class UpdateMemberDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = new AddressDto();
    public MembershipType MembershipType { get; set; }
    public bool IsActive { get; set; }
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
