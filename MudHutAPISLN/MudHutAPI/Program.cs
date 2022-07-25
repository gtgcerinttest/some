using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using MudHutAPI;
using MudHutAPI.DAL;
using MudHutAPI.Helpers;
using MudHutAPI.MiddleWare;
using MudHutAPI.Services.Implementations;
using MudHutAPI.Services.Interfaces;
using System.Net;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddDbContext<ApiDataContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

})
    .AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();

var apiSettings = new ApiSettings();
builder.Configuration.Bind(nameof(apiSettings), apiSettings);
builder.Services.AddSingleton(apiSettings);

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MudHut API", Version = "v1" });
    c.EnableAnnotations();
    c.OperationFilter<SwaggerFileOperationFilter>();
    c.SchemaFilter<SwaggerExcludePropertySchemaFilter>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                    {
                          new OpenApiSecurityScheme
                          {
                              Reference = new OpenApiReference
                              {
                                  Type = ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                              }
                          },
                         new string[] {}
                    }
                        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();


app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        //TODO: Remove the false and add an ip check or something to show that the request is from the admin site. 
        if (!ctx.Context.User.Identity.IsAuthenticated && false)
        {
            // respond HTTP 401 Unauthorized.
            ctx.Context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            // Append following 2 lines to drop body from static files middleware!
            ctx.Context.Response.ContentLength = 0;
            ctx.Context.Response.Headers.Add("Cache-Control", "no-store");
            ctx.Context.Response.Body = Stream.Null;
        }
        else
        {
            //TODO: Check for access to the folder by userID. THe folder has the ID of the user tha uploaded the file. If the user is allowed to access the file do nothing. else do the same as above. 
        }



    },
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
    RequestPath = new PathString("/StaticFiles")
});

app.MapControllers();

app.Run();
