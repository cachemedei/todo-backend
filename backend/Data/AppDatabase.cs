using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options)
    {  
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<TodoItem>(e => 
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Title).IsRequired();
            e.Property(x => x.Description).IsRequired();
            e.Property(x => x.IsCompleted).IsRequired();
        });

        modelBuilder.Entity<Account>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.Property(x => x.FirstName).IsRequired();
            e.Property(x => x.LastName).IsRequired();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.Password).IsRequired();

            e.HasMany(x => x.TodoItems)
                .WithOne(x => x.Account)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}