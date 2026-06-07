using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DunesOfArabia.Models;
using System.Text.Json;

namespace DunesOfArabia.Data
{
    /// <summary>
    /// Main EF Core database context for Dunes of Arabia / Saudi Heritage.
    ///
    /// ADDITIONS vs original:
    ///   1. UserDocuments  — identity verification docs (passport, ID, selfie).
    ///      Required by Profile.razor booking gate and Checkout.razor gate.
    ///   2. TripBuddyPosts, TripBuddyMessages, TripBuddyJoinRequests
    ///      — co-traveler matching feature (TripBuddy.razor).
    ///   3. Booking payment columns added to model config:
    ///      Subtotal, Tax, PaymentMethod, ConfirmationNumber, StripePaymentIntentId,
    ///      NumberOfTravelers — required by BookingService.UpdateAfterPaymentAsync.
    ///   4. Activity.DifficultyLevel, OperatorName etc. configured as optional strings.
    ///   5. All cascade behaviours consistent (no orphaned child rows).
    ///   6. Review.StarRating typed as decimal(3,1); Booking prices as decimal(18,2).
    ///   7. Itinerary.Interests stored as JSON nvarchar(max).
    ///   8. All original 18 destinations + 12 activities seed data preserved unchanged.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ── DbSets ────────────────────────────────────────────────────────────
        public DbSet<Destination> Destinations => Set<Destination>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
        public DbSet<Itinerary> Itineraries => Set<Itinerary>();
        public DbSet<DailyActivity> DailyActivities => Set<DailyActivity>();
        public DbSet<PackingItem> PackingItems => Set<PackingItem>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingDocument> BookingDocuments => Set<BookingDocument>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Complaint> Complaints => Set<Complaint>();

        // ── NEW DbSets ────────────────────────────────────────────────────────
        /// <summary>Identity documents for the profile booking gate (passport, ID, selfie).</summary>
        public DbSet<UserDocument> UserDocuments => Set<UserDocument>();
        /// <summary>Trip Buddy co-traveler posts.</summary>
        public DbSet<TripBuddyPost> TripBuddyPosts => Set<TripBuddyPost>();
        /// <summary>In-app chat messages between trip buddy matched users.</summary>
        public DbSet<TripBuddyChatMessage> TripBuddyMessages => Set<TripBuddyChatMessage>();
        /// <summary>Join requests sent from one user to a TripBuddyPost owner.</summary>
        public DbSet<TripBuddyJoinRequest> TripBuddyJoinRequests => Set<TripBuddyJoinRequest>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ══════════════════════════════════════════════════════════════════
            // ApplicationUser — extra columns
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<ApplicationUser>(e =>
            {
                e.Property(u => u.PhoneNumber).IsRequired(false);
                e.Property(u => u.FullName).HasMaxLength(150).IsRequired(false);
                e.Property(u => u.FirstName).HasMaxLength(75).IsRequired(false);
                e.Property(u => u.LastName).HasMaxLength(75).IsRequired(false);
                e.Property(u => u.AvatarUrl).HasMaxLength(500).IsRequired(false);
                e.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()")
                    .IsRequired();
            });

            // ══════════════════════════════════════════════════════════════════
            // Destination
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<Destination>(e =>
            {
                e.Property(d => d.ImageGallery)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new())
                    .HasColumnType("nvarchar(max)");

                e.Property(d => d.HighlightsJson)
                    .HasColumnType("nvarchar(max)")
                    .HasDefaultValue("[]");

                e.Property(d => d.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                e.Property(d => d.Cost)
                    .HasColumnType("decimal(18,2)");
            });

            // ══════════════════════════════════════════════════════════════════
            // Activity
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<Activity>(e =>
            {
                e.Property(a => a.DurationHours).HasColumnType("decimal(6,2)");
                e.Property(a => a.PriceSAR).HasColumnType("decimal(10,2)");
                e.Property(a => a.DifficultyLevel).HasMaxLength(30).IsRequired(false);
                e.Property(a => a.OperatorName).HasMaxLength(150).IsRequired(false);
                e.Property(a => a.OperatorEmail).HasMaxLength(150).IsRequired(false);
                e.Property(a => a.OperatorPhone).HasMaxLength(30).IsRequired(false);
                e.Property(a => a.CancellationPolicy).HasMaxLength(300).IsRequired(false);
            });

            // ══════════════════════════════════════════════════════════════════
            // Booking — decimal precision + payment columns
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<Booking>(e =>
            {
                e.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)");
                e.Property(b => b.Subtotal).HasColumnType("decimal(18,2)");
                e.Property(b => b.Tax).HasColumnType("decimal(18,2)");
                e.Property(b => b.PaymentMethod).HasMaxLength(30).IsRequired(false);
                e.Property(b => b.ConfirmationNumber).HasMaxLength(30).IsRequired(false);
                e.Property(b => b.StripePaymentIntentId).HasMaxLength(100).IsRequired(false);
                e.Property(b => b.Status).HasMaxLength(30);
            });

            // ══════════════════════════════════════════════════════════════════
            // Review — decimal precision
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<Review>(e =>
            {
                e.Property(r => r.StarRating).HasColumnType("decimal(3,1)");
                e.Property(r => r.Comment).IsRequired(false);
            });

            // ══════════════════════════════════════════════════════════════════
            // Itinerary — JSON interests list
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<Itinerary>(e =>
            {
                e.Property(i => i.Interests)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new())
                    .HasColumnType("nvarchar(max)");

                e.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ══════════════════════════════════════════════════════════════════
            // UserDocument — profile booking gate
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<UserDocument>(e =>
            {
                e.Property(d => d.Category).HasMaxLength(30).HasDefaultValue("Other");
                e.Property(d => d.FileUrl).IsRequired(false);
                e.Property(d => d.UploadedOn).HasDefaultValueSql("GETUTCDATE()");
            });

            // ══════════════════════════════════════════════════════════════════
            // TripBuddyPost
            // ══════════════════════════════════════════════════════════════════

            modelBuilder.Entity<TripBuddyPost>(e =>
            {
                e.Property(p => p.Bio).HasMaxLength(400).IsRequired(false);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ══════════════════════════════════════════════════════════════════
            // Relationships
            // ══════════════════════════════════════════════════════════════════

            // Booking → Destination
            modelBuilder.Entity<Booking>()
                .HasOne<Destination>()
                .WithMany()
                .HasForeignKey(b => b.DestinationId)
                .OnDelete(DeleteBehavior.Restrict);   // don't cascade-delete bookings when dest removed

            // BookingDocument → Booking
            modelBuilder.Entity<BookingDocument>()
                .HasOne(d => d.Booking)
                .WithMany(b => b.Documents)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // DailyActivity → Itinerary
            modelBuilder.Entity<DailyActivity>()
                .HasOne(a => a.Itinerary)
                .WithMany(i => i.Activities)
                .HasForeignKey(a => a.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // PackingItem → Itinerary (Cascade — not NoAction — avoids orphan error on delete)
            modelBuilder.Entity<PackingItem>()
                .HasOne(p => p.Itinerary)
                .WithMany(i => i.PackingItems)
                .HasForeignKey(p => p.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Activity → Destination
            modelBuilder.Entity<Activity>()
                .HasOne<Destination>()
                .WithMany()
                .HasForeignKey(a => a.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review → Destination
            modelBuilder.Entity<Review>()
                .HasOne<Destination>()
                .WithMany()
                .HasForeignKey(r => r.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review → ApplicationUser (no cascade — preserve reviews when user deleted)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // UserFavorite → Destination
            modelBuilder.Entity<UserFavorite>()
                .HasOne<Destination>()
                .WithMany()
                .HasForeignKey(uf => uf.DestinationId)
                .OnDelete(DeleteBehavior.Cascade);

            // TripBuddyChatMessage → TripBuddyPost
            modelBuilder.Entity<TripBuddyChatMessage>()
                .HasOne<TripBuddyPost>()
                .WithMany()
                .HasForeignKey(m => m.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // TripBuddyJoinRequest → TripBuddyPost
            modelBuilder.Entity<TripBuddyJoinRequest>()
                .HasOne<TripBuddyPost>()
                .WithMany()
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // ══════════════════════════════════════════════════════════════════
            // SEED DATA — all 18 destinations + 12 activities (unchanged)
            // ══════════════════════════════════════════════════════════════════

            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<Destination>().HasData(
                new Destination { Id = 1, Name = "Riyadh", Province = "Central Region", Category = "Urban", Rating = 4.8, Description = "The modern capital blending innovation with rich cultural heritage and historic landmarks.", ImageUrl = "https://images.unsplash.com/photo-1580418827493-f2b22c0a76cb?w=900", Latitude = 24.6877, Longitude = 46.7219, Cost = 800, Climate = "Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "November to February", Temperature = "20°C – 45°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 2, Name = "Jeddah", Province = "Red Sea Coast", Category = "Coastal", Rating = 4.7, Description = "Historic port city with beautiful coastline, vibrant culture, and world-class diving.", ImageUrl = "https://images.unsplash.com/photo-1578895101408-1a36b834405b?w=900", Latitude = 21.4858, Longitude = 39.1925, Cost = 700, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "22°C – 40°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 3, Name = "AlUla", Province = "Al Madinah Region", Category = "Historical", Rating = 4.9, Description = "Ancient rock formations and UNESCO World Heritage sites in a stunning desert landscape.", ImageUrl = "https://images.unsplash.com/photo-1631217073612-123ed4ea4eed?w=900", Latitude = 26.6100, Longitude = 37.9200, Cost = 1200, Climate = "Hot, Dry", VisaInfo = "Tourist Visa Available", BestSeason = "October to March", Temperature = "10°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 4, Name = "Diriyah", Province = "Riyadh Province", Category = "Historical", Rating = 4.6, Description = "The birthplace of the Kingdom with beautifully preserved mud-brick architecture.", ImageUrl = "https://images.unsplash.com/photo-1591604466107-ec97de577aff?w=900", Latitude = 24.7344, Longitude = 46.5754, Cost = 500, Climate = "Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "November to February", Temperature = "18°C – 44°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 5, Name = "Hegra (Mada'in Saleh)", Province = "Al Madinah Region", Category = "Historical", Rating = 4.8, Description = "Saudi Arabia's first UNESCO World Heritage Site with breathtaking Nabataean tombs carved into sandstone.", ImageUrl = "https://images.unsplash.com/photo-1614255548580-fc3cae2faae2?w=900", Latitude = 26.7914, Longitude = 37.9529, Cost = 950, Climate = "Hot, Dry", VisaInfo = "Tourist Visa Available", BestSeason = "October to March", Temperature = "10°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 6, Name = "Al Ula Old Town", Province = "Al Madinah Region", Category = "Historical", Rating = 4.5, Description = "A labyrinth of mud-brick houses dating back 2,000 years, abandoned and eerily preserved in the desert.", ImageUrl = "https://images.unsplash.com/photo-1526392060635-9d6019884377?w=900", Latitude = 26.5870, Longitude = 37.9168, Cost = 600, Climate = "Hot, Dry", VisaInfo = "Tourist Visa Available", BestSeason = "October to March", Temperature = "10°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 7, Name = "Empty Quarter", Province = "Southern Saudi Arabia", Category = "Desert", Rating = 4.5, Description = "The world's largest continuous sand desert offering unparalleled adventure experiences.", ImageUrl = "https://images.unsplash.com/photo-1509316785289-025f5b846b35?w=900", Latitude = 20.0000, Longitude = 50.0000, Cost = 1100, Climate = "Extremely Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "November to February", Temperature = "15°C – 50°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 8, Name = "Wadi Rum", Province = "Tabuk Region", Category = "Desert", Rating = 4.7, Description = "Dramatic red-sand valleys and towering sandstone pillars stretching to the horizon.", ImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=900", Latitude = 29.5755, Longitude = 35.4237, Cost = 900, Climate = "Hot, Dry", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "10°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 9, Name = "Al Nafud Desert", Province = "Northern Region", Category = "Desert", Rating = 4.3, Description = "Vast crescent-shaped dunes with striking reddish-orange sands unique to northern Arabia.", ImageUrl = "https://images.unsplash.com/photo-1547234935-80c7145ec969?w=900", Latitude = 28.0000, Longitude = 41.0000, Cost = 750, Climate = "Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "November to March", Temperature = "8°C – 42°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 10, Name = "Asir Mountains", Province = "Southwestern Saudi Arabia", Category = "Mountain", Rating = 4.7, Description = "Lush green mountains with a cooler climate, terraced farms, and breathtaking natural landscapes.", ImageUrl = "https://images.unsplash.com/photo-1464822759023-fed622ff2c3b?w=900", Latitude = 18.2164, Longitude = 42.5053, Cost = 650, Climate = "Mild, Temperate", VisaInfo = "Tourist Visa Available", BestSeason = "April to October", Temperature = "12°C – 30°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 11, Name = "Taif", Province = "Makkah Province", Category = "Mountain", Rating = 4.4, Description = "Mountain resort city famous for its rose gardens, cool summer retreats, and pleasant weather year-round.", ImageUrl = "https://images.unsplash.com/photo-1548263594-a71ea65a8598?w=900", Latitude = 21.2703, Longitude = 40.4158, Cost = 400, Climate = "Mild", VisaInfo = "Tourist Visa Available", BestSeason = "March to October", Temperature = "15°C – 35°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 12, Name = "Farasan Islands", Province = "Jizan Region", Category = "Nature", Rating = 4.6, Description = "Pristine coral reefs, crystal-clear waters, and rare wildlife in a protected Red Sea marine reserve.", ImageUrl = "https://images.unsplash.com/photo-1560275619-4662e36fa65c?w=900", Latitude = 16.7000, Longitude = 41.9667, Cost = 850, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "24°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 13, Name = "Al-Ahsa Oasis", Province = "Eastern Province", Category = "Nature", Rating = 4.3, Description = "The world's largest oasis with sprawling date palm gardens and natural artesian springs.", ImageUrl = "https://images.unsplash.com/photo-1501854140801-50d01698950b?w=900", Latitude = 25.3814, Longitude = 49.5864, Cost = 350, Climate = "Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "November to February", Temperature = "12°C – 45°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 14, Name = "Red Sea Project", Province = "Western Coast", Category = "Coastal", Rating = 4.8, Description = "Pristine islands and turquoise waters home to a new world-class luxury eco-tourism destination.", ImageUrl = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=900", Latitude = 28.0000, Longitude = 35.1500, Cost = 2200, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "22°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 15, Name = "Yanbu", Province = "Al Madinah Region", Category = "Coastal", Rating = 4.2, Description = "A laid-back Red Sea city with beautiful coral reefs, clear waters, and a charming historic old town.", ImageUrl = "https://images.unsplash.com/photo-1505118380757-91f5f5632de0?w=900", Latitude = 24.0893, Longitude = 38.0618, Cost = 500, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "20°C – 40°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 16, Name = "Jizan Corniche", Province = "Jizan Region", Category = "Coastal", Rating = 4.1, Description = "Vibrant waterfront promenade with fresh seafood, mangrove walks, and island day trips.", ImageUrl = "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?w=900", Latitude = 16.8892, Longitude = 42.5511, Cost = 300, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "November to March", Temperature = "22°C – 38°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 17, Name = "Al Khobar", Province = "Eastern Province", Category = "Urban", Rating = 4.2, Description = "A modern city on the Arabian Gulf known for its waterfront promenade and cosmopolitan dining.", ImageUrl = "https://images.unsplash.com/photo-1519999482648-25049ddd37b1?w=900", Latitude = 26.2172, Longitude = 50.1971, Cost = 550, Climate = "Hot, Humid", VisaInfo = "Tourist Visa Available", BestSeason = "November to March", Temperature = "18°C – 42°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate },
                new Destination { Id = 18, Name = "NEOM & Tabuk Region", Province = "Tabuk Region", Category = "Urban", Rating = 4.6, Description = "The future city of Saudi Arabia — a futuristic mega-project amidst dramatic desert and coastal scenery.", ImageUrl = "https://images.unsplash.com/photo-1573843981267-be1999ff37cd?w=900", Latitude = 28.0339, Longitude = 35.5136, Cost = 1800, Climate = "Hot, Arid", VisaInfo = "Tourist Visa Available", BestSeason = "October to April", Temperature = "15°C – 40°C", HighlightsJson = "[]", ImageGallery = new(), CreatedDate = seedDate }
            );

            modelBuilder.Entity<Activity>().HasData(
                new Activity { Id = 1, Name = "AlUla Heritage & Adventure Combo", Category = "Adventure", DurationHours = 8M, PriceSAR = 350, DestinationId = 3, Description = "Experience the best of AlUla with a combination of archaeological tours, desert adventures, and cultural immersion.", ImageUrl = "https://images.unsplash.com/photo-1616236197457-53e96373d0b0?w=900", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 2, Name = "Desert Safari", Category = "Adventure", DurationHours = 5M, PriceSAR = 150, DestinationId = 7, Description = "Thrilling off-road desert adventure through vast golden dunes with expert guides and traditional refreshments.", ImageUrl = "https://images.unsplash.com/photo-1516912481808-3406841bd33c?w=900", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 3, Name = "Rock Climbing", Category = "Adventure", DurationHours = 3M, PriceSAR = 120, DestinationId = 8, Description = "Scale spectacular sandstone formations and canyon walls with certified climbing instructors.", ImageUrl = "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=900", DifficultyLevel = "Challenging", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 4, Name = "Dune Bashing", Category = "Adventure", DurationHours = 2M, PriceSAR = 100, DestinationId = 7, Description = "Heart-pumping 4x4 ride across towering dunes in the vast Empty Quarter desert.", ImageUrl = "https://images.unsplash.com/photo-1542401886-65d6c61db217?w=900", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 5, Name = "Heritage Walking Tour", Category = "Cultural", DurationHours = 3M, PriceSAR = 90, DestinationId = 4, Description = "Guided walk through Diriyah's ancient mud-brick At-Turaif district with a local historian.", ImageUrl = "https://images.unsplash.com/photo-1539667284076-a4d98d9ac42b?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 6, Name = "Traditional Souq Experience", Category = "Cultural", DurationHours = 2M, PriceSAR = 70, DestinationId = 2, Description = "Explore labyrinthine souqs, taste local spices, and shop handcrafted Saudi treasures.", ImageUrl = "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 7, Name = "Archaeological Site Visit", Category = "Cultural", DurationHours = 4M, PriceSAR = 110, DestinationId = 3, Description = "Walk among Nabataean tombs and ancient inscriptions at AlUla's UNESCO World Heritage sites.", ImageUrl = "https://images.unsplash.com/photo-1569949380136-1b9e90c860b2?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 8, Name = "Scuba Diving", Category = "Water", DurationHours = 3M, PriceSAR = 200, DestinationId = 12, Description = "Dive into pristine Red Sea coral reefs teeming with vibrant marine life and stunning underwater formations.", ImageUrl = "https://images.unsplash.com/photo-1564769611905-cd27ee64e59b?w=900", DifficultyLevel = "Moderate", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 9, Name = "Snorkeling Adventure", Category = "Water", DurationHours = 3M, PriceSAR = 90, DestinationId = 12, Description = "Snorkel through crystal-clear waters above spectacular coral gardens and tropical fish.", ImageUrl = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 10, Name = "Camel Trekking", Category = "Desert", DurationHours = 2M, PriceSAR = 100, DestinationId = 7, Description = "Ride through golden sands atop a camel as the desert sun paints the dunes a brilliant crimson.", ImageUrl = "https://images.unsplash.com/photo-1549880338-65ddcdfd017b?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 11, Name = "Stargazing Experience", Category = "Desert", DurationHours = 2M, PriceSAR = 80, DestinationId = 9, Description = "Witness a breathtaking canopy of stars far from city lights, deep in the Arabian desert.", ImageUrl = "https://images.unsplash.com/photo-1446941303997-2843d7b4d20f?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" },
                new Activity { Id = 12, Name = "Bedouin Camp Experience", Category = "Desert", DurationHours = 8M, PriceSAR = 180, DestinationId = 9, Description = "Spend an evening in a traditional Bedouin camp with dinner, cultural music, and desert tales.", ImageUrl = "https://images.unsplash.com/photo-1519671282429-b44b0de7773e?w=900", DifficultyLevel = "Easy", OperatorName = "", OperatorEmail = "", OperatorPhone = "", CancellationPolicy = "" }
            );
        }
    }
}