var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.TaskTrack_WebApi>("tasktrack-api");

var nodeFrontend = builder.AddNpmApp("frontend", "../../frontend/tasktrack-web/", "dev")
    .WithReference(api) // Opcional: Referenciar uma API .NET
    .WithHttpEndpoint(env: "PORT") 
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http")); ;

builder.Build().Run();
