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
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        return await db.TodoItems.ToListAsync();
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
        //only want to send id to delete
        // var todoToDelete = await db.TodoItems.FindAsync(id)
        //     ?? throw new Exception("Cannot find task to delete");

        // db.TodoItems.Remove(todoToDelete);

        // await db.SaveChangesAsync();

        return NoContent();
    }
}