using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Services;

public class BankTransactionService 
{
    private readonly BankContext _context;
    public BankTransactionService(BankContext context)
    {
        _context = context;
    }

  public async Task<IEnumerable<BankTransactionDtoOut?>> GetDtoById(int id)
    {
        return await _context.BankTransactions.
            Where(a => a.AccountId == id).
            Select(a => new BankTransactionDtoOut
            {
                Id = a.Id,
                AccountName = a.TransitionTypeNavigation.Name,
                Amount = a.Amount,
                ExternalAccount = a.ExternalAccount == null ? 0 : a.ExternalAccount,
                RegDate = a.RegDate
            }).ToListAsync();/*Devuelve un objeto tipo dto o un null*/
    }

    public async Task<Account?> GetByID(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task TransactionCash(BankTransactionDtoIn transaction)
    {
        var existingAccount = await GetByID(transaction.AccountId);

        if (existingAccount is not null && existingAccount.Balance != 0)
        {
            existingAccount.Balance -= transaction.Amount; 

            await _context.SaveChangesAsync();
        }

    }

    public async Task TransactionTransfer(BankTransactionDtoIn transaction)
    {
        var existingAccount = await GetByID(transaction.AccountId);
        var existingExternalAccount = await GetByID(transaction.ExternalAccount ?? 0);

        if (existingAccount is not null && existingAccount.Balance != 0 && existingExternalAccount is not null)
        {
            existingAccount.Balance -= transaction.Amount;
            existingExternalAccount.Balance += transaction.Amount;  

            await _context.SaveChangesAsync();
        }

    }

    public async Task TransactionDeposit(BankTransactionDtoIn transaction)
    {
        var existingAccount = await GetByID(transaction.AccountId);

        if (existingAccount is not null)
        {
            existingAccount.Balance += transaction.Amount; 
            await _context.SaveChangesAsync();
        }

    }

    public async Task Delete(int id)
    {
        var accountToDelete = await GetByID(id);
        if (accountToDelete is not null && accountToDelete.Balance == 0 )
        {
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
    }

}