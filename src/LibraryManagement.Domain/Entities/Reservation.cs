namespace LibraryManagement.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int MemberId { get; set; }
    public Member Member { get; set; } = null!;
    public DateTime ReservationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsFulfilled { get; set; }
    public DateTime? FulfilledDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
