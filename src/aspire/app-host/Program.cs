using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<web_api>("web-api");

builder.AddNpmApp("frontend", "../../frontend/", "dev")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT") 
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"));

builder.Build().Run();
