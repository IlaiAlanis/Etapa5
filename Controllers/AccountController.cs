using Microsoft.AspNetCore.Mvc;

using BankAPI.Services;
using BankAPI.Data.BankModels;
using TestBankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountService accountService;
    private readonly AccountTypeService accountTypeService;
    private readonly ClientService clientService;

    public AccountController(AccountService accountService,
                             AccountTypeService accountTypeService,
                             ClientService clientService)
    {
        this.accountService = accountService;
        this.accountTypeService = accountTypeService;
        this.clientService = clientService;
    }

    [HttpGet("get")]
    public async Task<IEnumerable<AccountDtoOut>> Get()
    {
        
        return await accountService.GetAll();
        //Devuelve toda la informacion de la tabla clients como una lista
    }

    [HttpGet("get/{id}")]
    /*Los objetos dentro de un controlador se conocen como acciones*/
    public async Task<ActionResult<AccountDtoOut>> GetById(int id)
    {
        var account = await accountService.GetDtoById(id);
        //Devuelve un registro por ID

        if (account is null)
            
            return AccountNotFound(id);
        return account;
    }

    /*Diferencia entre IActionResult y ActionResult<t> es que uno devuelve
    un resultado de cualquier tipo y el otro especificamos que tipo de datos o resultado
    devovlera*/

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(AccountDTOIn account)
    {
       string validationResult = await ValidateAccount(account);
        if (!validationResult.Equals("Valid"))
            return BadRequest(new { message = validationResult});
        var newAccount = await accountService.Create(account);
        return CreatedAtAction(nameof(GetById), new { id = newAccount.Id}, newAccount);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, AccountDTOIn account) 
    {
        if (id != account.Id)
            return BadRequest(new { message = $"El ID ({id}) de la URL no coincide con el ID({account.Id}) del cuerpo de la solicitud."});
        var accountToUpdate = await accountService.GetByID(id);

        if (accountToUpdate is not null)
        {
           string validationResult = await ValidateAccount(account);
           if (!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult});
            await accountService.Update(account);
            return NoContent();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var accountToDelete = await accountService.GetByID(id);

        if (accountToDelete is not null)
        {
            await accountService.Delete(id);
            return Ok();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    public NotFoundObjectResult AccountNotFound(int id)
    {
        /*Es un objeto anonimo en el cual se va a convertir por default en un objeto json y sera
            parte de la respuesta*/
        return NotFound(new { message = $"El cliente con ID = {id} no existe."});
    }

    public async Task<string> ValidateAccount(AccountDTOIn account)
    {
        string result = "Valid";

        var accountType = await accountTypeService.GetByID(account.AccountType);

        if (accountType is null)
            result = $"El tipo de cuenta  {account.AccountType} no existe.";
        /*Hacemos que el valor de la propiedad no sea un nullo*/
        var clientID = account.ClientId.GetValueOrDefault();
        /*Estamos haciendo lo anterior porque el getbyid espera un valor entero
        no un entero tipo nullo*/
        var client = await clientService.GetByID(clientID);

        if (client is null)
            result = $"El lciente {clientID} no existe.";
        return result;
    }

}