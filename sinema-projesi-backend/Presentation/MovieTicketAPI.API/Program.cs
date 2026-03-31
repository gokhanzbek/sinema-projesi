using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieTicketAPI.Application;
using MovieTicketAPI.Infrastructure; 
using MovieTicketAPI.Persistence;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using MovieTicketAPI.Domain.Entities.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ETicaret API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'�n�z� 'Bearer {token}' format�nda girin."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// --- KATMANLI MİMARİ SERVİSLERİ ---
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();
// Eğer Infrastructure katmanın varsa (Token servisleri vb. için) bu satırı da eklemelisin:
builder.Services.AddInfrastructureServices();

// CurrentUser gibi işlemlerde HttpContext'e erişebilmek için:
builder.Services.AddHttpContextAccessor();

// --- 1. CORS POLİTİKASINI TANIMLA ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

// --- JWT AYARLARI ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Token:Audience"],
        ValidIssuer = builder.Configuration["Token:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]!))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 2. CORS KULLANIMI (Sıralama önemli!) ---
app.UseCors("AllowAll");

// --- 3. KİMLİK VE YETKİ KONTROLÜ (Sıralama önemli!) ---
app.UseAuthentication(); // Önce kimlik doğrulanır (Sen kimsin?)
app.UseAuthorization();  // Sonra yetki kontrol edilir (Bunu yapmaya iznin var mı?)

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    // 1. Sistemde "Admin" rozeti (rolü) yoksa, hemen matbaada bas
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new AppRole { Name = "Admin", NormalizedName = "ADMIN" });
    }

    // 2. "admin" adında bir kullanıcı yoksa, hemen oluştur
    if (await userManager.FindByNameAsync("admin") == null)
    {
        var adminUser = new AppUser
        {
            UserName = "admin",
            Email = "admin@sinemaprojesi.com",
            FirstName = "Admin",
            LastName = "Admin",
            // Eğer AppUser içinde FirstName, LastName gibi zorunlu alanların varsa buraya onları da yaz.
        };

        // Şifreyi veriyoruz (Identity varsayılan olarak büyük harf, sayı ve özel karakter ister)
        var result = await userManager.CreateAsync(adminUser, "Admin123*"); 

        // 3. Hesap başarıyla açıldıysa, "Admin" rozetini (rolünü) yakasına tak!
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();