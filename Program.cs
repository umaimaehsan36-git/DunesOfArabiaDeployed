using Blazored.LocalStorage;
using DunesOfArabia.Components;
using DunesOfArabia.Components.Account;
using DunesOfArabia.Data;
using DunesOfArabia.Models;
using DunesOfArabia.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Resend;
using Stripe;                      // ← NEW: Stripe.net namespace
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL;
// Disambiguate: Stripe.net also has a ReviewService — use our own explicitly
using AppReviewService = DunesOfArabia.Services.ReviewService;

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// =====================================================
// DATABASE
// =====================================================
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? Environment.GetEnvironmentVariable("DATABASE_URL")
              ?? "";

    // Convert postgres:// URL to Npgsql format if needed
    if (connStr.StartsWith("postgresql://") || connStr.StartsWith("postgres://"))
    {
        var uri = new Uri(connStr);
        var userInfo = uri.UserInfo.Split(':');
        connStr = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }

    options.UseNpgsql(connStr);
});

// =====================================================
// IDENTITY
// =====================================================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<RoleManager<IdentityRole>>();

// =====================================================
// COOKIE SETTINGS
// =====================================================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// =====================================================
// JWT AUTHENTICATION
// =====================================================
var jwtKey = builder.Configuration["Jwt:SecretKey"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(
        "JWT SecretKey is missing or empty in appsettings.json. " +
        "Add: \"Jwt\": { \"SecretKey\": \"your-32-char-minimum-secret-key\" }");

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// =====================================================
// AUTHORIZATION POLICIES
// =====================================================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

// =====================================================
// BLAZOR
// =====================================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthenticationStateProvider,
    IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

// =====================================================
// IDENTITY HELPER SERVICES
// =====================================================
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<IdentityUserAccessor>();

// =====================================================
// EMAIL - RESEND
// =====================================================
builder.Services.AddOptions<ResendClientOptions>()
    .Configure(options =>
    {
        options.ApiToken = builder.Configuration["Resend:ApiKey"]
            ?? throw new InvalidOperationException(
                "Resend API key missing. Run: dotnet user-secrets set \"Resend:ApiKey\" \"re_xxx\"");
    });
builder.Services.AddHttpClient<ResendClient>();
builder.Services.AddTransient<IResend, ResendClient>();
builder.Services.AddTransient<IEmailSender<ApplicationUser>, ResendEmailSender>();

// =====================================================
// APPLICATION SERVICES
// =====================================================
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDestinationService, DestinationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IReviewService, AppReviewService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IUserFavoriteService, UserFavoriteService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IDocumentUploadService, DocumentUploadService>();
builder.Services.AddScoped<ITripBuddyService, TripBuddyService>();

// =====================================================
// BLAZORED LOCAL STORAGE
// =====================================================
builder.Services.AddBlazoredLocalStorage();

// =====================================================
// HTTP CONTEXT (needed by services that read current user)
// =====================================================
builder.Services.AddHttpContextAccessor();

// =====================================================
// HTTP CLIENT (used by Checkout.razor to call /api/payments/create-intent
//              and by Planner.razor AI suggestion)
// =====================================================
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["AppBaseUrl"] ?? "https://localhost:5001";
    return new HttpClient { BaseAddress = new Uri(baseUrl) };
});

// =====================================================
// ANTIFORGERY
// =====================================================
// Skip antiforgery validation for /api/* routes — those are called by
// HttpClient from Blazor components and do not send an antiforgery token.
// Blazor's own form endpoints still get antiforgery protection.
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "XSRF-TOKEN";
});
// We register a no-op filter for API controllers via WithMetadata below.

// =====================================================
// CONTROLLERS + SWAGGER
// =====================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dunes of Arabia API",
        Version = "v1",
        Description = "Saudi Heritage Tourism Platform - REST API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }] = new List<string>()
    });
});

// =====================================================
// BUILD
// =====================================================
var app = builder.Build();

// =====================================================
// STRIPE — configure secret key at startup           ← NEW
// =====================================================
StripeConfiguration.ApiKey = app.Configuration["Stripe:SecretKey"]
    ?? throw new InvalidOperationException(
        "Stripe SecretKey missing. Add it to appsettings.json under Stripe:SecretKey.");

// =====================================================
// SEED ROLES + DEFAULT ADMIN ON STARTUP
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    const string adminEmail = "admin@dunesofarabia.com";
    const string adminPassword = "Admin@12345";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Site Administrator",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

    if (!Directory.Exists(webRoot))
        Directory.CreateDirectory(webRoot);

    var uploadsFolder = Path.Combine(webRoot, "uploads");
    if (!Directory.Exists(uploadsFolder))
        Directory.CreateDirectory(uploadsFolder);

    // ── AUTO-SEED ACTIVITIES ───────────────────────────────────────────────
    // HasData() seed only runs inside a migration. If the Activities table
    // already existed before seed data was added, it stays empty forever.
    // This block fills it on every startup if it finds the table empty — safe
    // to leave in permanently (the Any() check makes it a no-op once seeded).
    if (!db.Activities.Any())
    {
        db.Activities.AddRange(
            new DunesOfArabia.Models.Activity { Id = 1, Name = "AlUla Heritage & Adventure Combo", Category = "Adventure", DurationHours = 8M, PriceSAR = 350, DestinationId = 3, Description = "Experience the best of AlUla with a combination of archaeological tours, desert adventures, and cultural immersion.", ImageUrl = "https://images.unsplash.com/photo-1616236197457-53e96373d0b0?w=900", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 2, Name = "Desert Safari", Category = "Adventure", DurationHours = 5M, PriceSAR = 150, DestinationId = 7, Description = "Thrilling off-road desert adventure through vast golden dunes with expert guides and traditional refreshments.", ImageUrl = "https://images.unsplash.com/photo-1509316785289-025f5b846b35?w=700", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 3, Name = "Rock Climbing", Category = "Adventure", DurationHours = 3M, PriceSAR = 120, DestinationId = 8, Description = "Scale spectacular sandstone formations and canyon walls with certified climbing instructors.", ImageUrl = "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=700", DifficultyLevel = "Challenging", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 4, Name = "Dune Bashing", Category = "Adventure", DurationHours = 2M, PriceSAR = 100, DestinationId = 7, Description = "Heart-pumping 4x4 ride across towering dunes in the vast Empty Quarter desert.", ImageUrl = "https://images.unsplash.com/photo-1542401886-65d6c61db217?w=700", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 5, Name = "Heritage Walking Tour", Category = "Cultural", DurationHours = 3M, PriceSAR = 90, DestinationId = 4, Description = "Guided walk through Diriyah's ancient mud-brick At-Turaif district with a local historian.", ImageUrl = "https://images.unsplash.com/photo-1539667284076-a4d98d9ac42b?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 6, Name = "Traditional Souq Experience", Category = "Cultural", DurationHours = 2M, PriceSAR = 70, DestinationId = 2, Description = "Explore labyrinthine souqs, taste local spices, and shop handcrafted Saudi treasures.", ImageUrl = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 7, Name = "Archaeological Site Visit", Category = "Cultural", DurationHours = 4M, PriceSAR = 110, DestinationId = 3, Description = "Walk among Nabataean tombs and ancient inscriptions at AlUla's UNESCO World Heritage sites.", ImageUrl = "https://images.unsplash.com/photo-1591604466107-ec97de577aff?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 8, Name = "Scuba Diving", Category = "Water", DurationHours = 3M, PriceSAR = 200, DestinationId = 12, Description = "Dive into pristine Red Sea coral reefs teeming with vibrant marine life and stunning underwater formations.", ImageUrl = "https://images.unsplash.com/photo-1564769611905-cd27ee64e59b?w=700", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 9, Name = "Snorkeling Adventure", Category = "Water", DurationHours = 3M, PriceSAR = 90, DestinationId = 12, Description = "Snorkel through crystal-clear waters above spectacular coral gardens and tropical fish.", ImageUrl = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 10, Name = "Camel Trekking", Category = "Desert", DurationHours = 2M, PriceSAR = 100, DestinationId = 7, Description = "Ride through golden sands atop a camel as the desert sun paints the dunes a brilliant crimson.", ImageUrl = "https://images.unsplash.com/photo-1549880338-65ddcdfd017b?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 11, Name = "Stargazing Experience", Category = "Desert", DurationHours = 2M, PriceSAR = 80, DestinationId = 9, Description = "Witness a breathtaking canopy of stars far from city lights, deep in the Arabian desert.", ImageUrl = "https://images.unsplash.com/photo-1446941303997-2843d7b4d20f?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
            new DunesOfArabia.Models.Activity { Id = 12, Name = "Bedouin Camp Experience", Category = "Desert", DurationHours = 8M, PriceSAR = 180, DestinationId = 9, Description = "Spend an evening in a traditional Bedouin camp with dinner, cultural music, and desert tales.", ImageUrl = "https://images.unsplash.com/photo-1519671282429-b44b0de7773e?w=700", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" }
        );
        await db.SaveChangesAsync();
        Console.WriteLine("[Seed] 12 activities inserted.");
    }
    // ── END AUTO-SEED ─────────────────────────────────────────────────────
}

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Dunes of Arabia API v1");
        options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// =====================================================
// ROUTE MAPPING
// =====================================================
// RequireAuthorization is NOT applied globally — individual controllers
// use [Authorize] / [AllowAnonymous] as needed.
// IgnoreAntiforgeryToken is applied to all API controller endpoints so
// that HttpClient POSTs from Blazor components aren't rejected.
app.MapControllers().WithMetadata(new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute());

// ===== STARTUP DIAGNOSTIC =====
Console.WriteLine("Starting component scan diagnostic...");
try
{
    var asm = typeof(App).Assembly;
    foreach (var type in asm.GetTypes())
    {
        if (!typeof(Microsoft.AspNetCore.Components.IComponent).IsAssignableFrom(type)) continue;
        var attrs = type.GetCustomAttributes(true);
        bool hasPage = attrs.Any(a => a is Microsoft.AspNetCore.Components.RouteAttribute);
        bool hasLayout = attrs.Any(a => a is Microsoft.AspNetCore.Components.LayoutAttribute);
        foreach (var attr in attrs)
        {
            if (attr is Microsoft.AspNetCore.Components.RouteAttribute ra && string.IsNullOrEmpty(ra.Template))
                Console.WriteLine($"NULL ROUTE: {type.FullName}");
            if (attr is Microsoft.AspNetCore.Components.LayoutAttribute la && la.LayoutType == null)
                Console.WriteLine($"NULL LAYOUT: {type.FullName}");
        }
        if (hasLayout && !hasPage)
            Console.WriteLine($"LAYOUT WITHOUT PAGE: {type.FullName}");
    }
    Console.WriteLine("Component scan passed.");
}
catch (Exception ex) { Console.WriteLine($"Scan error: {ex.Message}"); }
// ===== END DIAGNOSTIC =====

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// =====================================================
// LOGOUT ENDPOINT
// =====================================================
app.MapPost("/Account/Logout", async (
    SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).WithOrder(-1);

app.MapAdditionalIdentityEndpoints();

app.Run();