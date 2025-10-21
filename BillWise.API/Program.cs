using Microsoft.EntityFrameworkCore;
using BillWise.Infrastructure.Data;
using BillWise.Infrastructure.Data.Repositories;
using BillWise.Domain.Interfaces;
using BillWise.Domain.Interfaces.Repositories;
using BillWise.Domain.Interfaces.Services;
using BillWise.Application.Services;
using BillWise.Application.Validators.Company;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ========== DATABASE CONFIGURATION ==========
builder.Services.AddDbContext<BillWiseDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========== REPOSITORY REGISTRATION ==========
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
// TODO: Diğer repository'ler eklenecek
// builder.Services.AddScoped<IUserRepository, UserRepository>();
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

// ========== UNIT OF WORK ==========
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ========== SERVICE REGISTRATION ==========
builder.Services.AddScoped<ICompanyService, CompanyService>();
// TODO: Diğer servisler eklenecek

// ========== VALIDATOR REGISTRATION ==========
builder.Services.AddScoped<CreateCompanyRequestValidator>();
builder.Services.AddScoped<UpdateCompanyRequestValidator>();
// FluentValidation assembly scan (opsiyonel)
builder.Services.AddValidatorsFromAssemblyContaining<CreateCompanyRequestValidator>();

// ========== AUTOMAPPER ==========
builder.Services.AddAutoMapper(typeof(BillWise.Application.Mappings.AutoMapperProfile));

// ========== CONTROLLERS ==========
builder.Services.AddControllers();

// ========== SWAGGER/OPENAPI ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BillWise API", Version = "v1" });
});

// ========== CORS (Development) ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========== MIDDLEWARE PIPELINE ==========

// CORS
app.UseCors("AllowAll");

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BillWise API v1");
    });
}

// HTTPS Redirection
app.UseHttpsRedirection();

// Authentication & Authorization (şimdilik yok)
// app.UseAuthentication();
// app.UseAuthorization();

// Map Controllers
app.MapControllers();

app.Run();
