using Azure.Storage.Blobs;
using BusinessLayer;
using BusinessLayer.Enumerations;
using BusinessLayer.Helpers;
using BusinessLayer.Interfaces;
using BusinessLayer.Interfaces.Authors;
using BusinessLayer.Interfaces.BookItems;
using BusinessLayer.Interfaces.BookLoans;
using BusinessLayer.Interfaces.BookRequests;
using BusinessLayer.Interfaces.Books;
using BusinessLayer.Interfaces.Comments;
using BusinessLayer.Interfaces.Email;
using BusinessLayer.Interfaces.Notifications;
using BusinessLayer.Interfaces.Publishers;
using BusinessLayer.Interfaces.UserBooks;
using BusinessLayer.Interfaces.Users;
using BusinessLayer.Services;
using DataAccess;
using DataAccess.Repositories;
using DataAccess.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.WithOrigins(
                    "http://localhost:4200",
                    "http://schoolofdotnet2022-vitosha.azurewebsites.net")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddScoped<IBooksService, BooksService>();
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddScoped<IAuthorsService, AuthorsService>();
builder.Services.AddScoped<IAuthorsRepository, AuthorsRepository>();
builder.Services.AddScoped<IPublishersService, PublisherService>();
builder.Services.AddScoped<IPublishersRepository, PublishersRepository>();
builder.Services.AddScoped<IBookItemsService, BookItemsService>();
builder.Services.AddScoped<IBookItemsRepository, BookItemsRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IBookRequestsService, BookRequestsService>();
builder.Services.AddScoped<IBookRequestsRepository, BookRequestsRepository>();
builder.Services.AddScoped<IBookLoansService, BookLoansService>();
builder.Services.AddScoped<IBookLoansRepository, BookLoansRepository>();
builder.Services.AddScoped<INotificationsService, NotificationService>();
builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IUserBooksRepository, UserBooksRepository>();

builder.Services.AddScoped<IBlobRepository, BlobRepository>();
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("BlobConnectionString")));
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSecret").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.RequireAdministratorRole,
        policy => policy.RequireRole(Roles.Administrator));

    options.AddPolicy(Policies.RequireLibrarianRole,
        policy => policy.RequireRole(Roles.Librarian));

    options.AddPolicy(Policies.RequireCustomerRole,
        policy => policy.RequireRole(Roles.Customer));

    options.AddPolicy(Policies.RequireAdministratorOrLibrarianRole,
        policy => policy.RequireRole(Roles.Administrator, Roles.Librarian));
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "The Curious Readers API",
        Description = "The Curious Readers backend API",
        Contact = new OpenApiContact
        {
            Name = "Team Vitosha"
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseCors();

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var dataContext = scope.ServiceProvider.GetService<DataContext>();
await dataContext!.Database.MigrateAsync();
await RoleSeeder.SeedRolesAsync(dataContext);
await UserSeeder.SeedUsersAsync(dataContext);

app.Run();