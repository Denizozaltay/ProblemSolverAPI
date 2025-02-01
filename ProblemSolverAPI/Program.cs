using ProblemSolverAPI.Controllers;
using ProblemSolverAPI.Services;

namespace ProblemSolverAPI
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi();
            builder.Services.AddHttpClient<OpenRouterService>();

            var app = builder.Build();
            app.MapControllers();
        
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
        
            app.Run();
        }
    }
}