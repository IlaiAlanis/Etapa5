using Microsoft.AspNetCore.Mvc;

using BankAPI.Services;
using BankAPI.Data.BankModels;
using TestBankAPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BankTransactionController : ControllerBase
{
    private readonly BankTransactionService bankTransactionService;
    private readonly TransactionTypeService transactionTypeService;
    private readonly AccountService accountService;

    public BankTransactionController(BankTransactionService bankTransactionService,
                             TransactionTypeService transactionTypeService,
                             AccountService accountService)
    {
        this.bankTransactionService = bankTransactionService;
        this.transactionTypeService = transactionTypeService;
        this.accountService = accountService;
    }

    [HttpGet("get/{id}")]
    /*Los objetos dentro de un controlador se conocen como acciones*/
    public async Task<IEnumerable<BankTransactionDtoOut>> GetById(int id)
    {
        var bankTransaction = await bankTransactionService.GetDtoById(id);
        //Devuelve un registro por ID

        if (bankTransaction is null)
            return (IEnumerable<BankTransactionDtoOut>)AccountNotFound(id);
        return bankTransaction;
    }

    /*Diferencia entre IActionResult y ActionResult<t> es que uno devuelve
    un resultado de cualquier tipo y el otro especificamos que tipo de datos o resultado
    devovlera*/

    //[Authorize(Policy = "SuperAdmin")]
    [HttpPut("transactioncash/{id}")]
    public async Task<IActionResult> TransactionCash(int id, BankTransactionDtoIn account) 
    {
        if (id != account.AccountId)
            return BadRequest(new { message = $"El ID ({id}) de la URL no coincide con el ID({account.AccountId}) del cuerpo de la solicitud."});
        var accountToUpdate = await bankTransactionService.GetByID(id);

        if (accountToUpdate is not null && accountToUpdate.Balance != 0) 
        {
           string validationResult = await ValidateAccount(account);
           if (!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult});
            await bankTransactionService.TransactionCash(account);
            return CreatedAtAction(nameof(GetById), new { id = account.AccountId}, account);
        }
        else
        {
            return AccountNotFound(id);
        }
    }

   // [Authorize(Policy = "SuperAdmin")]
    [HttpPut("transactiontransfer/{id}")]
    public async Task<IActionResult> TransactionTransfer(int id, BankTransactionDtoIn account) 
    {
        if (id != account.AccountId)
            return BadRequest(new { message = $"El ID ({id}) de la URL no coincide con el ID({account.AccountId}) del cuerpo de la solicitud."});
        var accountToUpdate = await bankTransactionService.GetByID(id);

        if (accountToUpdate is not null)
        {
           string validationResult = await ValidateAccount(account);
           if (!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult});
            await bankTransactionService.TransactionTransfer(account);
            return NoContent();
        }
        else
        {
            return AccountNotFound(id);
        }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("transactiontransfer/{id}")]
    public async Task<IActionResult> TransactionDeposit(int id, BankTransactionDtoIn account) 
    {

        if (id != account.AccountId)
            return BadRequest(new { message = $"El ID ({id}) de la URL no coincide con el ID({account.AccountId}) del cuerpo de la solicitud."});
        var accountToUpdate = await bankTransactionService.GetByID(id);

        if (accountToUpdate is not null)
        {
           string validationResult = await ValidateAccount(account);
           if (!validationResult.Equals("Valid"))
                return BadRequest(new { message = validationResult});
            await bankTransactionService.TransactionDeposit(account);
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
        var accountToDelete = await bankTransactionService.GetByID(id);

        if (accountToDelete is not null)
        {
            await bankTransactionService.Delete(id);
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
        return NotFound(new { message = $"La cuenta con ID = {id} no existe o no tiene slado suficiente."});
    }

    public async Task<string> ValidateAccount(BankTransactionDtoIn account)
    {
        string result = "Valid";

        var accountType = await transactionTypeService.GetByID(account.TransitionType);

        if (accountType is null)
            result = $"El tipo de transaccion  {account.TransitionType} no existe.";
     
        var existingaccount = await accountService.GetByID(account.AccountId);

        if (existingaccount is null)
            result = $"La cuenta {existingaccount.Id} no existe.";
        return result;
    }

}