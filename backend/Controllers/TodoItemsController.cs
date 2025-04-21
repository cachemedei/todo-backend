using backend.Contracts;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Route("api/todo")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoItemServices todoItemServices;
    private readonly TodoDb db;

    public TodoItemsController(TodoItemServices todoItemServices, TodoDb db)
    {
        this.todoItemServices = todoItemServices;
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<TodoItem>>> GetTodoItems() //all todos
    {
        return await db.TodoItems.ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<List<TodoItem>>> GetUsersTodos(int id) //todos for user
    {
        var usersTodos = await db.TodoItems
            .Where<TodoItem>(x => x.AccountId == id)
            .ToListAsync();

        return usersTodos;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem data)
    {
        await todoItemServices.CreateTodoAsync(data);
        return NoContent();
    }

    [HttpDelete("{id}")] // pass in request
    public async Task<ActionResult<TodoItem>> DeleteTodoItem(int id)
    {
        await todoItemServices.DeleteTodoAsync(id);
        return NoContent();
    }

    [HttpPatch]
    public async Task<ActionResult<TodoItem>> UpdateExistingTodo(UpdateTodoContract data)
    {
        await todoItemServices.UpdateTodoAsync(data);
        return NoContent();
    }
}