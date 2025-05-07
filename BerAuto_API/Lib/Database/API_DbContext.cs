namespace BerAuto.Lib.Database
{
	public class API_DbContext : DbContext
	{
        public API_DbContext(DbContextOptions<API_DbContext> options)
        : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
		public DbSet<CarRent> CarRents { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Log> Logs { get; set; }
		public DbSet<Rent> Rents { get; set; }
		public DbSet<User> Users { get; set; }

		/*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=DESKTOP-HD8T179\SQLEXPRESS;Database=BerAutoDb;Trusted_Connection=true;TrustServerCertificate=true;");
		}
		*/

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasKey(x => x.ID);
			modelBuilder.Entity<Car>().HasKey(x => x.ID);

			modelBuilder.Entity<CarRent>().HasKey(x => x.ID);

			modelBuilder.Entity<Rent>().HasKey(x => x.ID);

			modelBuilder.Entity<Log>().HasKey(x => x.ID);
			modelBuilder.Entity<User>().HasKey(x => x.ID);

			modelBuilder.Entity<Car>().ToTable("Cars");
			modelBuilder.Entity<CarRent>().ToTable("CarRents");
			modelBuilder.Entity<Category>().ToTable("Categories");
			modelBuilder.Entity<Log>().ToTable("Logs");
			modelBuilder.Entity<Rent>().ToTable("Rents");
			modelBuilder.Entity<User>().ToTable("Users");
		}
	}
}
