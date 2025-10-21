using ContentNegotiation.Data;
using ContentNegotiation.Formatters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // Enable content negotiation based on Accept header
    options.OutputFormatters.Add(new PlainTextTableOutputFormatter());
}).AddXmlSerializerFormatters(); // Add XML formatter support

builder.Services.AddSingleton<ProductRepository>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();
