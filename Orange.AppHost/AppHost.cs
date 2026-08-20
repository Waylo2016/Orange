var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Orange_Api>("orange-api");

builder.Build().Run();
