namespace TestBankAPI.Data.DTOs;

public class BankTransactionDtoIn
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int TransitionType { get; set; }

    public decimal Amount { get; set; }

    public int? ExternalAccount { get; set; }

    public DateTime RegDate { get; set; }
}