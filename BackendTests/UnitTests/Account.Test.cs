using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BackendTests.UnitTests;

public class AccountTest : IClassFixture<UnitTestsFixture>
{
    private readonly UnitTestsFixture fixture;

    public AccountTest(UnitTestsFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task CreateNewAccountAsync()
    {
        //setup
        await fixture.SetupAsync();
        var db = fixture.GetRequiredService<TodoDb>();

        var testAccount = new Account
        {
            FirstName = "Jill",
            Email = "jill@mail.com",
        };

        db.Accounts.Add(testAccount);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        var accounts = await db.Accounts.ToListAsync();
        Assert.Single(accounts);
        Assert.Equal("Jill", accounts[0].FirstName);        
    }

    [Fact]
    public async Task CheckEmailIsUniqueAsync()
    {
        //setup
        await fixture.SetupAsync();
        var db = fixture.GetRequiredService<TodoDb>();
        var service = fixture.GetRequiredService<AccountServices>();

        var account = new Account
        {
            FirstName = "Jill",
            Email = "jill@mail.com",
        };

        await service.CreateAccountAsync(account);

        var duplicateEmail = new Account
        {
            FirstName = "Jane",
            Email = "jill@mail.com",
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => 
        service.CreateAccountAsync(duplicateEmail));

        Assert.Equal("Email already in use.", ex.Message);
    }
}