

using Microsoft.OpenApi.Models;
using TodoApi;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ToDoDbContext>();

builder.Services.AddCors(option =>{
    option.AddPolicy("openPolicy",
                     policy =>
                     {
                      policy.WithOrigins("*")
                                         .AllowAnyHeader()
                                         .AllowAnyMethod();
                                             
                      });
});

var app = builder.Build();

app.UseCors("openPolicy");

app.MapGet("/", () => "Hello World!");

app.MapGet("/items", (ToDoDbContext context) =>
{
    return context.Items.ToList();
});

app.MapPost("/items/{name}",async(ToDoDbContext context,string name)=>{
    Item item=new Item();
    item.Name=name;
    item.IsComplete=false;
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async(ToDoDbContext context, [FromBody]Item item, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    existItem.Name = item.Name;
    existItem.IsComplete = item.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

   app.MapDelete("/items/{id}", async(ToDoDbContext context, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    context.Items.Remove(existItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});



app.Run();
