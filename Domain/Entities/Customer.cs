namespace Domain.Entities;

public class Customer
{
    public int Id {  get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public int AccountNumber { get; set; }
    public bool Active { get; set; }
}
