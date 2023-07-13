using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using TestBankAPI.Data.DTOs;

namespace BankAPI.Services;

public class AccountService
{
    private readonly BankContext _context;
    public AccountService (BankContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountDtoOut>> GetAll()
    {
        return await _context.Accounts.Select(a => new AccountDtoOut
        {
            Id = a.Id,
            AccountName = a.AccountTypeNavigation.Name,
            /*Como esta acepta nullos entonces hacemos un condicional en el que si no es nullo asgignale
            el nombre sino una cadena vacia*/
            ClientName = a.Client != null ? a.Client.Name : "",
            Balance = a.Balance,
            RegDate = a.RegDate
        }).ToListAsync();
    }

     public async Task<AccountDtoOut?> GetDtoById(int id)
    {
        return await _context.Accounts.
            Where(a => a.Id == id).
            Select(a => new AccountDtoOut
            {
                Id = a.Id,
                AccountName = a.AccountTypeNavigation.Name,
                /*Como esta acepta nullos entonces hacemos un condicional en el que si no es nullo asgignale
                el nombre sino una cadena vacia*/
                ClientName = a.Client != null ? a.Client.Name : "",
                Balance = a.Balance,
                RegDate = a.RegDate
            }).SingleOrDefaultAsync();/*Devuelve un objeto tipo dto o un null*/
    }

    public async Task<Account?> GetByID(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> Create(AccountDTOIn newAccountDTO)
    {
        var newAccount = new Account();
        /*Hacemos conversion para poder realmente usar el metodo*/
        newAccount.AccountType = newAccountDTO.AccountType;
        newAccount.ClientId = newAccountDTO.ClientId;
        newAccount.Balance = newAccountDTO.Balance;
        
        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return newAccount;
    }

    public async Task Update(AccountDTOIn account)
    {
        var existingAccount = await GetByID(account.Id);

        if (existingAccount is not null)
        {
            existingAccount.AccountType = account.AccountType;
            existingAccount.ClientId = account.ClientId;
            existingAccount.Balance = account.Balance;

            await _context.SaveChangesAsync();
        }

    }

    public async Task Delete(int id)
    {
        var accountToDelete = await GetByID(id);
        if (accountToDelete is not null)
        {
            _context.Accounts.Remove(accountToDelete);
            await _context.SaveChangesAsync();
        }
    }
}