using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<web_api>("web-api");

builder.AddJavaScriptApp("frontend", "../../frontend")
    .WithNpm()
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT") 
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"));

builder.Build().Run();
