using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DunesOfArabia.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    LastName = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: true),
                    AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AdminResponse = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Climate = table.Column<string>(type: "text", nullable: false),
                    VisaInfo = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    BestSeason = table.Column<string>(type: "text", nullable: false),
                    Temperature = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageGalleryJson = table.Column<string>(type: "text", nullable: false),
                    ImageGallery = table.Column<string>(type: "text", nullable: false),
                    HighlightsJson = table.Column<string>(type: "text", nullable: false, defaultValue: "[]")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TripBuddyPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false),
                    DestinationName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TripType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    BudgetRange = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Bio = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    TotalSpots = table.Column<int>(type: "integer", nullable: false),
                    SpotsLeft = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripBuddyPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValue: "Other"),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UploadedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DestinationId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    DurationHours = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    PriceSAR = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MaxParticipants = table.Column<int>(type: "integer", nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    IncludedServices = table.Column<string>(type: "text", nullable: false),
                    CancellationPolicy = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OperatorName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    OperatorEmail = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    OperatorPhone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DestinationId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Destinations_DestinationId1",
                        column: x => x.DestinationId1,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConfirmationNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NumberOfTravelers = table.Column<int>(type: "integer", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DestinationId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Destinations_DestinationId1",
                        column: x => x.DestinationId1,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Itineraries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false),
                    Travelers = table.Column<int>(type: "integer", nullable: false),
                    TripType = table.Column<string>(type: "text", nullable: false),
                    Interests = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Itineraries_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    DestinationId = table.Column<int>(type: "integer", nullable: true),
                    ActivityId = table.Column<int>(type: "integer", nullable: true),
                    StarRating = table.Column<decimal>(type: "numeric(3,1)", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DestinationId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Destinations_DestinationId1",
                        column: x => x.DestinationId1,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DestinationId = table.Column<int>(type: "integer", nullable: false),
                    DestinationId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Destinations_DestinationId1",
                        column: x => x.DestinationId1,
                        principalTable: "Destinations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TripBuddyJoinRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    RequesterId = table.Column<string>(type: "text", nullable: false),
                    RequesterName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripBuddyJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripBuddyJoinRequests_TripBuddyPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "TripBuddyPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripBuddyMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<string>(type: "text", nullable: false),
                    SenderName = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    RecipientId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripBuddyMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripBuddyMessages_TripBuddyPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "TripBuddyPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FileUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingDocuments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItineraryId = table.Column<int>(type: "integer", nullable: false),
                    DayNumber = table.Column<int>(type: "integer", nullable: false),
                    ActivityName = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyActivities_Itineraries_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itineraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PackingItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItineraryId = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsPacked = table.Column<bool>(type: "boolean", nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackingItems_Itineraries_ItineraryId",
                        column: x => x.ItineraryId,
                        principalTable: "Itineraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "BestSeason", "Category", "Climate", "Cost", "CreatedAt", "CreatedDate", "Description", "HighlightsJson", "ImageGallery", "ImageGalleryJson", "ImageUrl", "Latitude", "Longitude", "Name", "Province", "Rating", "Temperature", "VisaInfo" },
                values: new object[,]
                {
                    { 1, "November to February", "Urban", "Hot, Arid", 800m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The modern capital blending innovation with rich cultural heritage and historic landmarks.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1580418827493-f2b22c0a76cb?w=900", 24.6877, 46.721899999999998, "Riyadh", "Central Region", 4.7999999999999998, "20°C – 45°C", "Tourist Visa Available" },
                    { 2, "October to April", "Coastal", "Hot, Humid", 700m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Historic port city with beautiful coastline, vibrant culture, and world-class diving.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1578895101408-1a36b834405b?w=900", 21.485800000000001, 39.192500000000003, "Jeddah", "Red Sea Coast", 4.7000000000000002, "22°C – 40°C", "Tourist Visa Available" },
                    { 3, "October to March", "Historical", "Hot, Dry", 1200m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ancient rock formations and UNESCO World Heritage sites in a stunning desert landscape.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1631217073612-123ed4ea4eed?w=900", 26.609999999999999, 37.920000000000002, "AlUla", "Al Madinah Region", 4.9000000000000004, "10°C – 38°C", "Tourist Visa Available" },
                    { 4, "November to February", "Historical", "Hot, Arid", 500m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The birthplace of the Kingdom with beautifully preserved mud-brick architecture.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1591604466107-ec97de577aff?w=900", 24.734400000000001, 46.575400000000002, "Diriyah", "Riyadh Province", 4.5999999999999996, "18°C – 44°C", "Tourist Visa Available" },
                    { 5, "October to March", "Historical", "Hot, Dry", 950m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Saudi Arabia's first UNESCO World Heritage Site with breathtaking Nabataean tombs carved into sandstone.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1614255548580-fc3cae2faae2?w=900", 26.791399999999999, 37.9529, "Hegra (Mada'in Saleh)", "Al Madinah Region", 4.7999999999999998, "10°C – 38°C", "Tourist Visa Available" },
                    { 6, "October to March", "Historical", "Hot, Dry", 600m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A labyrinth of mud-brick houses dating back 2,000 years, abandoned and eerily preserved in the desert.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1526392060635-9d6019884377?w=900", 26.587, 37.916800000000002, "Al Ula Old Town", "Al Madinah Region", 4.5, "10°C – 38°C", "Tourist Visa Available" },
                    { 7, "November to February", "Desert", "Extremely Hot, Arid", 1100m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The world's largest continuous sand desert offering unparalleled adventure experiences.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1509316785289-025f5b846b35?w=900", 20.0, 50.0, "Empty Quarter", "Southern Saudi Arabia", 4.5, "15°C – 50°C", "Tourist Visa Available" },
                    { 8, "October to April", "Desert", "Hot, Dry", 900m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dramatic red-sand valleys and towering sandstone pillars stretching to the horizon.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=900", 29.575500000000002, 35.423699999999997, "Wadi Rum", "Tabuk Region", 4.7000000000000002, "10°C – 38°C", "Tourist Visa Available" },
                    { 9, "November to March", "Desert", "Hot, Arid", 750m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vast crescent-shaped dunes with striking reddish-orange sands unique to northern Arabia.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1547234935-80c7145ec969?w=900", 28.0, 41.0, "Al Nafud Desert", "Northern Region", 4.2999999999999998, "8°C – 42°C", "Tourist Visa Available" },
                    { 10, "April to October", "Mountain", "Mild, Temperate", 650m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lush green mountains with a cooler climate, terraced farms, and breathtaking natural landscapes.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=900", 18.2164, 42.505299999999998, "Asir Mountains", "Southwestern Saudi Arabia", 4.7000000000000002, "12°C – 30°C", "Tourist Visa Available" },
                    { 11, "March to October", "Mountain", "Mild", 400m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mountain resort city famous for its rose gardens, cool summer retreats, and pleasant weather year-round.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1548263594-a71ea65a8598?w=900", 21.270299999999999, 40.415799999999997, "Taif", "Makkah Province", 4.4000000000000004, "15°C – 35°C", "Tourist Visa Available" },
                    { 12, "October to April", "Nature", "Hot, Humid", 850m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pristine coral reefs, crystal-clear waters, and rare wildlife in a protected Red Sea marine reserve.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1560275619-4662e36fa65c?w=900", 16.699999999999999, 41.966700000000003, "Farasan Islands", "Jizan Region", 4.5999999999999996, "24°C – 38°C", "Tourist Visa Available" },
                    { 13, "November to February", "Nature", "Hot, Arid", 350m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The world's largest oasis with sprawling date palm gardens and natural artesian springs.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1501854140801-50d01698950b?w=900", 25.381399999999999, 49.586399999999998, "Al-Ahsa Oasis", "Eastern Province", 4.2999999999999998, "12°C – 45°C", "Tourist Visa Available" },
                    { 14, "October to April", "Coastal", "Hot, Humid", 2200m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pristine islands and turquoise waters home to a new world-class luxury eco-tourism destination.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=900", 28.0, 35.149999999999999, "Red Sea Project", "Western Coast", 4.7999999999999998, "22°C – 38°C", "Tourist Visa Available" },
                    { 15, "October to April", "Coastal", "Hot, Humid", 500m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A laid-back Red Sea city with beautiful coral reefs, clear waters, and a charming historic old town.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1505118380757-91f5f5632de0?w=900", 24.089300000000001, 38.061799999999998, "Yanbu", "Al Madinah Region", 4.2000000000000002, "20°C – 40°C", "Tourist Visa Available" },
                    { 16, "November to March", "Coastal", "Hot, Humid", 300m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Vibrant waterfront promenade with fresh seafood, mangrove walks, and island day trips.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=900", 16.889199999999999, 42.551099999999998, "Jizan Corniche", "Jizan Region", 4.0999999999999996, "22°C – 38°C", "Tourist Visa Available" },
                    { 17, "November to March", "Urban", "Hot, Humid", 550m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A modern city on the Arabian Gulf known for its waterfront promenade and cosmopolitan dining.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1519999482648-25049ddd37b1?w=900", 26.217199999999998, 50.197099999999999, "Al Khobar", "Eastern Province", 4.2000000000000002, "18°C – 42°C", "Tourist Visa Available" },
                    { 18, "October to April", "Urban", "Hot, Arid", 1800m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The future city of Saudi Arabia — a futuristic mega-project amidst dramatic desert and coastal scenery.", "[]", "[]", "[]", "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=900", 28.033899999999999, 35.513599999999997, "NEOM & Tabuk Region", "Tabuk Region", 4.5999999999999996, "15°C – 40°C", "Tourist Visa Available" }
                });

            migrationBuilder.InsertData(
                table: "Activities",
                columns: new[] { "Id", "CancellationPolicy", "Category", "Description", "DestinationId", "DestinationId1", "DifficultyLevel", "DurationHours", "ImageUrl", "IncludedServices", "MaxParticipants", "MinAge", "Name", "OperatorEmail", "OperatorName", "OperatorPhone", "PriceSAR", "Rating" },
                values: new object[,]
                {
                    { 1, "", "Adventure", "Experience the best of AlUla with a combination of archaeological tours, desert adventures, and cultural immersion.", 3, null, "Moderate", 8m, "https://images.unsplash.com/photo-1616236197457-53e96373d0b0?w=900", "", 20, 0, "AlUla Heritage & Adventure Combo", "", "", "", 350m, 0.0 },
                    { 2, "", "Adventure", "Thrilling off-road desert adventure through vast golden dunes with expert guides and traditional refreshments.", 7, null, "Moderate", 5m, "https://images.unsplash.com/photo-1516912481808-3406841bd33c?w=900", "", 20, 0, "Desert Safari", "", "", "", 150m, 0.0 },
                    { 3, "", "Adventure", "Scale spectacular sandstone formations and canyon walls with certified climbing instructors.", 8, null, "Challenging", 3m, "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=900", "", 20, 0, "Rock Climbing", "", "", "", 120m, 0.0 },
                    { 4, "", "Adventure", "Heart-pumping 4x4 ride across towering dunes in the vast Empty Quarter desert.", 7, null, "Moderate", 2m, "https://images.unsplash.com/photo-1542401886-65d6c61db217?w=900", "", 20, 0, "Dune Bashing", "", "", "", 100m, 0.0 },
                    { 5, "", "Cultural", "Guided walk through Diriyah's ancient mud-brick At-Turaif district with a local historian.", 4, null, "Easy", 3m, "https://images.unsplash.com/photo-1539667284076-a4d98d9ac42b?w=900", "", 20, 0, "Heritage Walking Tour", "", "", "", 90m, 0.0 },
                    { 6, "", "Cultural", "Explore labyrinthine souqs, taste local spices, and shop handcrafted Saudi treasures.", 2, null, "Easy", 2m, "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=900", "", 20, 0, "Traditional Souq Experience", "", "", "", 70m, 0.0 },
                    { 7, "", "Cultural", "Walk among Nabataean tombs and ancient inscriptions at AlUla's UNESCO World Heritage sites.", 3, null, "Easy", 4m, "https://images.unsplash.com/photo-1569949380136-1b9e90c860b2?w=900", "", 20, 0, "Archaeological Site Visit", "", "", "", 110m, 0.0 },
                    { 8, "", "Water", "Dive into pristine Red Sea coral reefs teeming with vibrant marine life and stunning underwater formations.", 12, null, "Moderate", 3m, "https://images.unsplash.com/photo-1564769611905-cd27ee64e59b?w=900", "", 20, 0, "Scuba Diving", "", "", "", 200m, 0.0 },
                    { 9, "", "Water", "Snorkel through crystal-clear waters above spectacular coral gardens and tropical fish.", 12, null, "Easy", 3m, "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=900", "", 20, 0, "Snorkeling Adventure", "", "", "", 90m, 0.0 },
                    { 10, "", "Desert", "Ride through golden sands atop a camel as the desert sun paints the dunes a brilliant crimson.", 7, null, "Easy", 2m, "https://images.unsplash.com/photo-1549880338-65ddcdfd017b?w=900", "", 20, 0, "Camel Trekking", "", "", "", 100m, 0.0 },
                    { 11, "", "Desert", "Witness a breathtaking canopy of stars far from city lights, deep in the Arabian desert.", 9, null, "Easy", 2m, "https://images.unsplash.com/photo-1446941303997-2843d7b4d20f?w=900", "", 20, 0, "Stargazing Experience", "", "", "", 80m, 0.0 },
                    { 12, "", "Desert", "Spend an evening in a traditional Bedouin camp with dinner, cultural music, and desert tales.", 9, null, "Easy", 8m, "https://images.unsplash.com/photo-1519671282429-b44b0de7773e?w=900", "", 20, 0, "Bedouin Camp Experience", "", "", "", 180m, 0.0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DestinationId",
                table: "Activities",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_DestinationId1",
                table: "Activities",
                column: "DestinationId1");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingDocuments_BookingId",
                table: "BookingDocuments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DestinationId",
                table: "Bookings",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DestinationId1",
                table: "Bookings",
                column: "DestinationId1");

            migrationBuilder.CreateIndex(
                name: "IX_DailyActivities_ItineraryId",
                table: "DailyActivities",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_DestinationId",
                table: "Itineraries",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PackingItems_ItineraryId",
                table: "PackingItems",
                column: "ItineraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DestinationId",
                table: "Reviews",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DestinationId1",
                table: "Reviews",
                column: "DestinationId1");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripBuddyJoinRequests_PostId",
                table: "TripBuddyJoinRequests",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_TripBuddyMessages_PostId",
                table: "TripBuddyMessages",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_DestinationId",
                table: "UserFavorites",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_DestinationId1",
                table: "UserFavorites",
                column: "DestinationId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookingDocuments");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "DailyActivities");

            migrationBuilder.DropTable(
                name: "PackingItems");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "TripBuddyJoinRequests");

            migrationBuilder.DropTable(
                name: "TripBuddyMessages");

            migrationBuilder.DropTable(
                name: "UserDocuments");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Itineraries");

            migrationBuilder.DropTable(
                name: "TripBuddyPosts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Destinations");
        }
    }
}
