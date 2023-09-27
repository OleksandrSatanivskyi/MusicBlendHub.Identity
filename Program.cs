
using MusicBlendHub.Identity.Middlewares;

namespace MusicBlendHub.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            ConfigureApp(app);


            app.Run();

        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.AddPersistence(configuration);
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
        }

        private static void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("AllowAnyPolicy"); //change in future

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}