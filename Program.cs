using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiDemo.Models;
using System.Reflection.Metadata.Ecma335;
namespace MinimalApiDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddDbContext<MovieDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("connString")));


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

           // app.UseAuthorization();




            // retriev all rows
            app.MapGet("/movie", async (MovieDbContext context) =>
                                  await context.Movies.ToListAsync()
                    );


            // retrieve single row

            app.MapGet("/movie/{ID}", async (MovieDbContext context, int ID) =>
            {
                var row = await context.Movies.FindAsync(ID);
                if (row is null)
                {
                    return Results.NotFound("Sorry, you chose an invalid ID");
                }
                else
                {
                    return Results.Ok(row);
                }
            }
            );



            app.MapPost("/movie", async(MovieDbContext context, Movie product) =>
                {
                    if (product == null)
                    {
                        return Results.BadRequest("Fill in the required data");
                    }
                    await context.Movies.AddAsync(product);
                    await context.SaveChangesAsync();
                    return Results.Created($"/product/{product.Id}", product);
                }
                );




            app.MapPatch("/movie/{ID}", async (MovieDbContext context, Movie updated, int ID) =>
            {
                var product = await context.Movies.FindAsync(ID);

                if (product == null)
                {
                    return Results.NotFound($"{ID} is not a valid ID");
                }
                product.Budget = updated.Budget;
                product.Title = updated.Title;
                await context.SaveChangesAsync();
                return Results.Ok(product);
            }
    );


            app.MapDelete("/movie/{ID}", async (MovieDbContext context, int ID) =>
            {
                var movie = await context.Movies.FindAsync(ID);
                if (movie is null)
                {
                    return Results.NotFound($"Sorry,movie with ID {ID} is not found");
                }


                context.Movies.Remove(movie);
                await context.SaveChangesAsync();

                return Results.NoContent();

            }
    );


            app.Run();
        }
    }
}
