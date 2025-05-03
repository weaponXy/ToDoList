using Supabase;
using Supabase.Interfaces;
using ToDoList.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<Supabase.Client>(_ =>
    new Supabase.Client(
        builder.Configuration["SupabaseUrl"],
        builder.Configuration["SupabaseKey"],
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

//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthorization();
//app.MapControllers();

// Sample home route
app.MapGet("/", () => "ToDoList API is running!");

// API Endpoints
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
