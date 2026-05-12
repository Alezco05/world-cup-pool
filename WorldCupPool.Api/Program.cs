using WorldCupPool.Api.Infrastructure;
using WorldCupPool.Api.Extensions; // <-- Importante para que reconozca tus extensiones

var builder = WebApplication.CreateBuilder(args);

// 1. Registrar Controladores básicos
builder.Services.AddControllers();

// 2. Configurar Políticas de CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Llama a tus métodos de extensión (La magia limpia)
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCustomJwtAuthentication(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// ==========================================
// PIPELINE HTTP & MIDDLEWARES
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); 
    app.UseSwaggerUi(); 
}

// Inicializar la base de datos y ejecutar la semilla (Seeder)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Seed(context);
}

app.UseHttpsRedirection();
app.UseCors("AngularClient");

// Middlewares de seguridad
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
