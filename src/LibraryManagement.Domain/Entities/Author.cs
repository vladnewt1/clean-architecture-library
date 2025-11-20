namespace LibraryManagement.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Nationality { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public string FullName => $"{FirstName} {LastName}";
}
