using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace BackendTests.UnitTests;

public class TodoItemTest : IClassFixture<UnitTestsFixture>
{
    private readonly UnitTestsFixture fixture;

    public TodoItemTest(UnitTestsFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task CreateTodoItemAsync()
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

        var testTodo = new TodoItem
        {
            Title = "test title",
            Description = "test description",
            IsCompleted = false,
            AccountId = testAccount.Id,
            Account = testAccount
        };

        //act
        db.TodoItems.Add(testTodo);
        await db.SaveChangesAsync();


        //test
        var todos = await db.TodoItems.ToListAsync();
        Assert.Single(todos);
        Assert.Equal("test title", todos[0].Title);
    }

    [Fact]
    public async Task CheckForAccountToPostTodoAsync()
    {
        await fixture.SetupAsync();
        var db = fixture.GetRequiredService<TodoDb>();
        var service = fixture.GetRequiredService<TodoItemServices>();

        var todo = new TodoItem
        {
            Title = "test title",
            Description = "test description",
            IsCompleted = false,
            AccountId = 1000,
        };

        var ex = await Assert.ThrowsAsync<Exception>(() => 
        service.CreateTodoAsync(todo));

        Assert.Equal("Need valid account", ex.Message);
    }
}