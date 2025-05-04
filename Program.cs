using Supabase;
using Supabase.Interfaces;
using ToDoList.Models;
using DotNetEnv;
using static Postgrest.Constants;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(
        Environment.GetEnvironmentVariable("SUPABASE_URL"),
        Environment.GetEnvironmentVariable("SUPABASE_KEY"),
        new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        }));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern:"{controller=ToDo}/{action=Index}/{id?}");

app.MapGet("/todos", async (
    string? category,
    string? priority,
    string? sortBy,
    Supabase.Client client) =>
{
    var query = client.From<Todo>();

    if (!string.IsNullOrWhiteSpace(category))
        query.Filter("category", Operator.Equals, category);

    if (!string.IsNullOrWhiteSpace(priority))
        query.Filter("priority", Operator.Equals, priority);

    if (!string.IsNullOrWhiteSpace(sortBy))
    {
        if (sortBy.Equals("category", StringComparison.OrdinalIgnoreCase))
            query.Order("category", Ordering.Ascending);
        else if (sortBy.Equals("priority", StringComparison.OrdinalIgnoreCase))
            query.Order("priority", Ordering.Ascending);
    }

    var response = await query.Get();

    var results = response.Models.Select(todo => new ToDoResponse
    {
        Id = todo.Id,
        Title = todo.Title,
        IsCompleted = todo.IsCompleted,
        Category = todo.Category,
        Priority = todo.Priority
    });

    return Results.Ok(results);
});

app.MapPost("/todos", async (
    CreateToDoRequest request,
    Supabase.Client client) =>
{
    var todo = new Todo
    {
        Title = request.Title,
        IsCompleted = request.IsCompleted,
        Category = request.Category,
        Priority = request.Priority
    };

    var response = await client.From<Todo>().Insert(todo);
    var newTodo = response.Models.First();

    return Results.Ok(newTodo.Id);
});

app.MapGet("/todos/{id}", async (int Id, Supabase.Client client) =>
{
    var response = await client
        .From<Todo>()
        .Where(n => n.Id == Id)
        .Get();

    var todo = response.Models.FirstOrDefault();
    if (todo is null) return Results.NotFound();

    return Results.Ok(new ToDoResponse
    {
        Id = todo.Id,
        Title = todo.Title,
        IsCompleted = todo.IsCompleted,
        Category = todo.Category,
        Priority = todo.Priority
    });
});

app.MapDelete("/todos/{id}", async (int Id, Supabase.Client client) =>
{
    await client.From<Todo>().Where(n => n.Id == Id).Delete();
    return Results.NoContent();
});

app.Run();
