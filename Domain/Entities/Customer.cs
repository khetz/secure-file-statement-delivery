namespace Domain.Entities;

public class Customer
{
    public Guid Id {  get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string FullName { get; set; }
    public required int AccountNumber { get; set; }
    public bool Active { get; set; }

    public ICollection<Statement> Statements { get; set; } = new List<Statement>();
}
