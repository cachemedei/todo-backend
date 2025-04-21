using backend.Contracts;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TodoItemServices
{
    private readonly TodoDb db;

    public TodoItemServices(TodoDb db)
    {
        this.db = db;
    }

    public async Task CreateTodoAsync(TodoItem data)
    {
        // var accountExists = await db.Accounts.AnyAsync(x => x.Id == data.AccountId);

        // if (!accountExists)
        // {
        //     throw new Exception("Need valid account");
        // };

        db.TodoItems.Add(data);
        await db.SaveChangesAsync();
    } 

    public async Task DeleteTodoAsync(int id)
    {
        var todoToDelete = await db.TodoItems.FindAsync(id)
            ?? throw new Exception("Cannot find task to delete");
        
        db.TodoItems.Remove(todoToDelete);
        await db.SaveChangesAsync();
    }

    public async Task UpdateTodoAsync(UpdateTodoContract data){
        var todoToUpdate = await db.TodoItems.FindAsync(data.Id)
            ?? throw new Exception("Cannot find todo to update");
        
        if (data.Title != null)
            todoToUpdate.Title = data.Title;
        
        if (data.Description != null) 
            todoToUpdate.Description = data.Description;
        
        if (data.IsCompleted.HasValue) 
            todoToUpdate.IsCompleted = data.IsCompleted.Value;

        await db.SaveChangesAsync();
    }    
}