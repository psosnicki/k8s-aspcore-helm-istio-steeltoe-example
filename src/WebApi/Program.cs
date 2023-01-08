using Steeltoe.Extensions.Configuration.Kubernetes;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Kubernetes;
using Steeltoe.Management.Kubernetes;
using Steeltoe.Management.Endpoint;

var builder = WebApplication.CreateBuilder(args)
    .AddServiceDiscovery(x => x.UseKubernetes())
    .AddKubernetesConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddKubernetesActuators(builder.Configuration);
builder.Services.AddAllActuators(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/test", () =>
{
    return Results.Ok(new
    {
        pod = $"{Environment.GetEnvironmentVariable("POD_NAME") ?? "Unknown"}",
        node = $"{Environment.GetEnvironmentVariable("NODE_NAME") ?? "Unknown"}",
        config = app.Configuration["config-map-test"]
    });
})
.WithName("Test")
.WithOpenApi();
app.MapAllActuators(MediaTypeVersion.V2);
app.Run();