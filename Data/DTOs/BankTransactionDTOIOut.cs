namespace TestBankAPI.Data.DTOs;

public class BankTransactionDtoOut
{
    public int Id { get; set; }

    public string AccountName { get; set; } = null!;
    
    public decimal Amount { get; set; }

    public int? ExternalAccount { get; set; }

    public DateTime RegDate { get; set; }
}