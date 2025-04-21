using backend.Contracts;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AccountServices
{
    private readonly TodoDb db;

    public AccountServices(TodoDb db)
    {
        this.db = db;
    }

    public async Task<List<AccountResponse>> GetAllAccountsAsync()
    {
        var accounts = await db.Accounts.ToListAsync();
        var response = accounts.Select(x => new AccountResponse
        {
            Id = x.Id,
            FirstName = x.FirstName,
            Email = x.Email
        }).ToList();

        return response;
    }

    public async Task<AccountResponse> CreateAccountAsync(Account data)
    {
        var alreadyExists = await db.Accounts.AnyAsync(x => x.Email == data.Email);

        if (alreadyExists)
        {
            throw new Exception("Email already in use.");
        }

        //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);

        var account = new Account
        {
            FirstName = data.FirstName,
            Email = data.Email,
            // Password = hashedPassword
        };
        db.Accounts.Add(account);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw new Exception("An error occurred while creating account.");
        }
        return new AccountResponse
        {
            Id = account.Id,
            FirstName = account.FirstName,
            Email = account.Email
        };
    }
}