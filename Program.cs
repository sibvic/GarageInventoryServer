using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Swagger;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add EF and PostgreSQL config to builder
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    "Host=localhost;Port=5432;Database=garage_db;Username=postgres;Password=password";
builder.Services.AddDbContext<GarageInventoryContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- Projects CRUD ---
app.MapGet("/projects", async (GarageInventoryContext db) =>
    await db.Projects.ToListAsync());

app.MapGet("/projects/{id}", async (int id, GarageInventoryContext db) =>
    await db.Projects.FindAsync(id) is Project project ? Results.Ok(project) : Results.NotFound());

// Refactored: Use view model, set date on server
app.MapPost("/projects", async (ProjectCreateModel model, GarageInventoryContext db) =>
{
    var project = new Project
    {
        Name = model.Name,
        CreationDate = DateTime.UtcNow,
        Price = model.Price
    };
    db.Projects.Add(project);
    await db.SaveChangesAsync();
    return Results.Created($"/projects/{project.Id}", project);
});

app.MapPut("/projects/{id}", async (int id, Project updatedProject, GarageInventoryContext db) =>
{
    var project = await db.Projects.FindAsync(id);
    if (project is null) return Results.NotFound();
    project.Name = updatedProject.Name;
    project.Price = updatedProject.Price;
    await db.SaveChangesAsync();
    return Results.Ok(project);
});

app.MapDelete("/projects/{id}", async (int id, GarageInventoryContext db) =>
{
    var project = await db.Projects.FindAsync(id);
    if (project is null) return Results.NotFound();
    db.Projects.Remove(project);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// --- Locations CRUD ---
app.MapGet("/locations", async (GarageInventoryContext db) =>
    await db.Locations.ToListAsync());

app.MapGet("/locations/{id}", async (int id, GarageInventoryContext db) =>
    await db.Locations.FindAsync(id) is Location location ? Results.Ok(location) : Results.NotFound());

app.MapPost("/locations", async (Location location, GarageInventoryContext db) =>
{
    db.Locations.Add(location);
    await db.SaveChangesAsync();
    return Results.Created($"/locations/{location.Id}", location);
});

app.MapPut("/locations/{id}", async (int id, Location updatedLocation, GarageInventoryContext db) =>
{
    var location = await db.Locations.FindAsync(id);
    if (location is null) return Results.NotFound();
    location.Name = updatedLocation.Name;
    await db.SaveChangesAsync();
    return Results.Ok(location);
});

app.MapDelete("/locations/{id}", async (int id, GarageInventoryContext db) =>
{
    var location = await db.Locations.FindAsync(id);
    if (location is null) return Results.NotFound();
    db.Locations.Remove(location);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// --- Items CRUD ---
app.MapGet("/items", async (int? projectId, GarageInventoryContext db) =>
{
    var query = db.Items
        .Include(i => i.Location)
        .Include(i => i.Project)
        .AsQueryable();

    if (projectId.HasValue)
    {
        query = query.Where(i => i.ProjectId == projectId.Value);
    }

    query = query.OrderByDescending(i => i.InDate);

    return await query.ToListAsync();
});

app.MapGet("/items/{id}", async (int id, GarageInventoryContext db) =>
    await db.Items.Include(i => i.Location).Include(i => i.Project).FirstOrDefaultAsync(i => i.Id == id)
         is Item item ? Results.Ok(item) : Results.NotFound());

// Refactored: Use view model, set InDate server-side
app.MapPost("/items", async (ItemCreateModel model, GarageInventoryContext db) =>
{
    var project = await db.Projects.FindAsync(model.ProjectId);
    if (project is null) return Results.NotFound();
    if (string.IsNullOrEmpty(model.SKU))
    {
        model.SKU = $"{model.ProjectId:D3}-{project.LastIndex:D5}";
        project.LastIndex++;
        db.Projects.Update(project);
    }
    var item = new Item
    {
        Name = model.Name,
        ManufacturerNumber = model.ManufacturerNumber,
        LocationId = model.LocationId,
        SKU = model.SKU,
        InPrice = model.InPrice,
        InDate = DateTime.UtcNow,
        OutPrice = model.OutPrice,
        Status = model.Status,
        Description = model.Description,
        ProjectId = model.ProjectId
    };
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("/items/{id}", async (int id, Item updatedItem, GarageInventoryContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Name = updatedItem.Name;
    item.ManufacturerNumber = updatedItem.ManufacturerNumber;
    item.LocationId = updatedItem.LocationId;
    item.SKU = updatedItem.SKU;
    item.InPrice = updatedItem.InPrice;
    item.InDate = updatedItem.InDate;
    item.OutPrice = updatedItem.OutPrice;
    item.OutDate = updatedItem.OutDate;
    item.Status = updatedItem.Status;
    item.Description = updatedItem.Description;
    item.ProjectId = updatedItem.ProjectId;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.MapDelete("/items/{id}", async (int id, GarageInventoryContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.UseSwagger();
app.Run();
