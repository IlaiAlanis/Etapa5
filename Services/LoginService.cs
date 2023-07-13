using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using BankAPI.Data.BankModels;
using BankAPI.Data.DTOs;

namespace BankAPI.Services;

public class LoginService 
{
    private readonly BankContext _context;
    public LoginService(BankContext context)
    {
        _context = context;
    }

    /*Corroborar si hace mach los datos*/
    public async Task<Administrator?>  GetAdmin(AdminDto admin)
    {
        /*creamos objeto administrators y lo comprobamos con nuestros objeto admin
        si no coincide es null*/
        return await _context.Administrators.
                    SingleOrDefaultAsync(x => x.Email == admin.Email && x.Pwd == admin.Pwd);
    }
    
}