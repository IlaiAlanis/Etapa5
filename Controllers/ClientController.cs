using Microsoft.AspNetCore.Mvc;

using BankAPI.Services;
using BankAPI.Data.BankModels;
using Microsoft.AspNetCore.Authorization;

namespace BankAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly ClientService _service;
    public ClientController(ClientService service)
    {
        _service = service;
    }


    [HttpGet("get")]
    
    public async Task<IEnumerable<Client>> Get()
    {
        
        return await _service.GetAll();
        //Devuelve toda la informacion de la tabla clients como una lista
    }

    [HttpGet("get/{id}")]
    /*Los objetos dentro de un controlador se conocen como acciones*/
    public async Task<ActionResult<Client>> GetById(int id)
    {
        var client = await _service.GetByID(id);
        //Devuelve un registro por ID

        if (client is null)
            
            return ClientNotFound(id);
        return client;
    }

    /*Diferencia entre IActionResult y ActionResult<t> es que uno devuelve
    un resultado de cualquier tipo y el otro especificamos que tipo de datos o resultado
    devovlera*/

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("create")]
    public async Task<IActionResult> Create(Client client)
    {
       var newClient = await _service.Create(client);
        return CreatedAtAction(nameof(GetById), new { id = newClient.Id}, newClient);
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, Client client) 
    {
        if (id != client.Id)
            return BadRequest(new { message = $"El ID ({id}) de la URL no coincide con el ID({client.Id}) del cuerpo de la solicitud."});
        var clientToUpdate = await _service.GetByID(id);

        if (clientToUpdate is not null)
        {
           await  _service.Update(id, client);
            return NoContent();
        }
        else
        {
            return ClientNotFound(id);
        }
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var clientToDelete = await _service.GetByID(id);

        if (clientToDelete is not null)
        {
            await _service.Delete(id);
            return Ok();
        }
        else
        {
            return ClientNotFound(id);
        }
    }

    public NotFoundObjectResult ClientNotFound(int id)
    {
        /*Es un objeto anonimo en el cual se va a convertir por default en un objeto json y sera
            parte de la respuesta*/
        return NotFound(new {message = $"El cliente con ID = {id} no existe."});
    }


}