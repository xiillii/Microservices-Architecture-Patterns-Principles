var builder = DistributedApplication.CreateBuilder(args);

// 1. Add the password parameter from configuration/secrets
// recuerda usar el comando en el proyecto: dotnet user-secrets init
// dotnet user-secrets set "postgres-password" "password"
var postgresPassword = builder.AddParameter("postgres-password", secret: true);

// 2. Pass it to the postgres builder
var postgres = builder
        .AddPostgres("postgres", password: postgresPassword) 
        .WithPgAdmin(pgAdmin => pgAdmin.WithUrlForEndpoint("http", url => url.DisplayText = "PostgreDB Browser"))
        .WithDataVolume()
        .WithLifetime(ContainerLifetime.Persistent);

var eshopDb = postgres.AddDatabase("eshopdb");

builder
    .AddProject<Projects.WebApp>("webapp")
    .WithUrlForEndpoint("https", url => url.DisplayText = "EShop WebApp (HTTPS)")
    .WithUrlForEndpoint("http", url => url.DisplayText = "EShop WebApp (HTTP)")
    .WithReference(eshopDb)
    .WaitFor(eshopDb);

builder.Build().Run();
