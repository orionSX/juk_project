namespace AkinatorApi

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open AkinatorApi.Services
open AkinatorApi.Models
open Microsoft.EntityFrameworkCore

type Startup(configuration: IConfiguration) =
    member _.Configuration = configuration

    
    member _.ConfigureServices(services: IServiceCollection) =
       
        services.AddControllers() |> ignore
        services.AddSwaggerGen() |> ignore

        
        services.AddCors(fun options ->
            options.AddDefaultPolicy(fun builder ->
                builder
                    .WithOrigins("http://localhost:5173", "https://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    |> ignore
            )
        ) |> ignore

     
        services.AddDbContext<AkinatorDbContext>(fun options ->
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            |> ignore
        ) |> ignore

        
        services.AddSingleton<PrologService>() |> ignore
        services.AddScoped<DatabaseService>() |> ignore
        services.AddSingleton<AuthService>() |> ignore

    
    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        
        app.UseCors() |> ignore

        app.UseRouting() |> ignore
        app.UseHttpsRedirection() |> ignore
        app.UseAuthorization() |> ignore

        app.UseEndpoints(fun endpoints ->
            endpoints.MapControllers() |> ignore
        ) |> ignore
