namespace Application.Responses;

public class StatementWithCustomerIdResponse : StatementResponse
{
    public Guid CustomerId { get; set; }
}
