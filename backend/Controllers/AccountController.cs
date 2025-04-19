using backend.Contracts;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/account")]
[ApiController]
public class AccountControllers : ControllerBase
{
    private readonly AccountServices accountServices;
    private readonly TodoDb db;

    public AccountControllers(AccountServices accountServices, TodoDb db)
    {
        this.accountServices = accountServices;
        this.db = db;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAccount([FromBody] Account data)
    {
        try
        {
            var newAccount = await accountServices.CreateAccountAsync(data);
            return Ok(newAccount);
        }
        catch (Exception e)
        {
            return Problem(
                detail: e.Message,
                statusCode: StatusCodes.Status400BadRequest
            );
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<AccountResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAccounts()
    {
        var list = await accountServices.GetAllAccountsAsync();

        return Ok(list);
    }
}