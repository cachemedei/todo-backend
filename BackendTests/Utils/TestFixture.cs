using backend.Data;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

//test fixture is responsible for setting up & tearing down all the services & environment needed
//for tests to run in isolation
public class UnitTestsFixture : IDisposable
{

    private SqliteConnection? connection;
    //sqlite connection field: used to keep an open inMemory db connection alive during test run
    private ServiceProvider? container;
    //container field: built in dependency injection container
    private IServiceScope? testScope;
    //a scoped lifetime for resolving services during a test -- mimics what happens in http request lifecycle
    public T GetRequiredService<T>() where T : notnull => testScope!.ServiceProvider.GetRequiredService<T>();

    public async Task SetupAsync(Action<IServiceCollection>? setupHook = null) //async because it performs ef core migrations
    {
        testScope?.Dispose();
        container?.Dispose();
        connection?.Close();
        
        connection = new SqliteConnection($"Filename=file:{Guid.NewGuid()}?mode=memory&cache=shared");
        await connection.OpenAsync();
        
        var services = new ServiceCollection();
        services.AddDbContext<TodoDb>(options =>
            options.UseSqlite(connection));

        services.AddScoped<TodoItemServices>();
        services.AddScoped<AccountServices>();

        container = services.BuildServiceProvider();

        //run migrations
        using var scope = container.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
        await db.Database.MigrateAsync();
        db.ChangeTracker.Clear();

        testScope = container.CreateScope();
    }

    public void Dispose()
    {
        testScope?.Dispose();
        container?.Dispose();
        connection?.Dispose();
    }
}