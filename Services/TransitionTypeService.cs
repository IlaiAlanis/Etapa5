using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;

namespace BankAPI.Services;

public class TransactionTypeService 
{
    private readonly BankContext _context;

    public TransactionTypeService (BankContext context)
    {
        _context = context;
    }

    public async Task<TransitionType?> GetByID(int id) 
    {
        return await _context.TransitionTypes.FindAsync(id);
    }
}