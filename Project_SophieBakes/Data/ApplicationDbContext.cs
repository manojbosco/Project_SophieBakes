using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Models;

namespace Project_SophieBakes.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; } // Ensure Categories DbSet
        
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        
        public DbSet<DeliveryOrders> DeliveryOrders { get; set; }
        
        
        public DbSet<Order> Orders { get; set; }  // Orders table
        public DbSet<User> Users { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }  // OrderItems table
   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderItem>()
                .Property(o => o.OrderItemId)
                .ValueGeneratedOnAdd();  // Ensures auto-increment is set correctly
            
            // 🔹 OrderItem → Order (Cascade Delete)
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // ✅ Ensure deletion cascades

            // 🔹 OrderItem → Product (Restrict Deletion)
            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Cakes" },
                new Category { CategoryId = 2, Name = "Cupcakes" },
                new Category { CategoryId = 3, Name = "Cookies" },
                new Category { CategoryId = 4, Name = "Others" }
            ); // 🔹 **Fixed semicolon here**

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1, ProductName = "Chocolate Cake", Description = "Delicious chocolate cake ",
                    Price = 399.00m, ImageUrl = "/images/chocolate-cake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 2, ProductName = "Vanilla Cupcake", Description = "Soft delicious vanilla cupcake",
                    Price = 39.00m, ImageUrl = "/images/vanilla-cupcake.jpg", CategoryId = 2
                },
                new Product
                {
                    ProductId = 3, ProductName = "Red Velvet Cake", Description = "Rich and delicious red velvet cake",
                    Price = 699.00m, ImageUrl = "/images/red-velvet-cake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 4, ProductName = "Lemon Tart", Description = "Tangy lemon tart with a buttery crust",
                    Price = 199.00m, ImageUrl = "/images/Lemon Tart.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 5, ProductName = "Carrot Cake",
                    Description = "Moist carrot cake with cream cheese frosting", Price = 499.00m,
                    ImageUrl = "/images/carrot cake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 6, ProductName = "Chocolate Cookies", Description = "Classic chocolate chip cookies",
                    Price = 29.00m, ImageUrl = "/images/chocolate chip cookies.jpg", CategoryId = 2
                },
                new Product
                {
                    ProductId = 7, ProductName = "Strawberry Shortcake",
                    Description = "Layers of sponge cake with fresh strawberries", Price = 399.00m,
                    ImageUrl = "/images/strawberry shortcake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 8, ProductName = "Pineapple Upside Down Cake",
                    Description = "Classic pineapple upside down cake with Nuts", Price = 499.00m,
                    ImageUrl = "/images/pineapple upside down cake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 9, ProductName = "Cheesecake",
                    Description = "Rich and creamy cheesecake with crushed cookies and Strawberry ", Price = 599.00m,
                    ImageUrl = "/images/cheesecake.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 10, ProductName = "Mango Mousse",
                    Description = "Light, soft and creamy, full of tropical taste and delicious.", Price = 299.00m,
                    ImageUrl = "/images/mango mousse.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 11, ProductName = "Chocolate Mousse", Description = "Decadent chocolate mousse",
                    Price = 299.00m, ImageUrl = "/images/chocolate mousse.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 12, ProductName = "Banana Bread", Description = "Moist banana bread with walnuts",
                    Price = 199.00m, ImageUrl = "/images/banana bread.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 13, ProductName = "Oreo Cheesecake", Description = "Cheesecake with Oreo crust",
                    Price = 599.00m, ImageUrl = "/images/oreo cheesecake.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 14, ProductName = "Tiramisu",
                    Description = "The delicate flavor of layers of mascarpone and Italian custard", Price = 499.00m,
                    ImageUrl = "/images/tiramisu.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 15, ProductName = "Cupcake Assortment", Description = "Assorted cupcakes",
                    Price = 199.00m, ImageUrl = "/images/cupcake assortment.jpg", CategoryId = 2
                },
                new Product
                {
                    ProductId = 16, ProductName = "Brownies",
                    Description = "They're rich, chocolatey, buttery fudge with extra love.", Price = 29.00m,
                    ImageUrl = "/images/brownies.jpg", CategoryId = 3
                },
                new Product
                {
                    ProductId = 17, ProductName = "Apple Pie",
                    Description = "Apple pie is served with whipped cream and cheddar cheese.", Price = 399.00m,
                    ImageUrl = "/images/apple pie.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 18, ProductName = "Peach Cobbler",
                    Description = "Warm peach cobbler with vanilla ice cream", Price = 299.00m,
                    ImageUrl = "/images/peach cobbler.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 19, ProductName = "Chocolate Eclair",
                    Description = " Chocolate Eclair Cake is an old-fashioned sheet cake and Cream-filled. ",
                    Price = 49.00m, ImageUrl = "/images/chocolate eclair.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 20, ProductName = "Pistachio Cake",
                    Description = "Delicious pistachio-flavored cake is super moist and flavorful bomb",
                    Price = 499.00m, ImageUrl = "/images/pistachio cake.jpg", CategoryId = 1
                },
                new Product
                {
                    ProductId = 21, ProductName = "Coconut Cream Pie",
                    Description = "Creamy coconut pie with a flaky crust and love", Price = 399.00m,
                    ImageUrl = "/images/coconut cream pie.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 22, ProductName = "Raspberry Tart",
                    Description = "Fresh raspberry tart with a buttery crust and wild raspberry", Price = 299.00m,
                    ImageUrl = "/images/raspberry tart.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 23, ProductName = "Almond Biscotti",
                    Description = "Crispy almond biscotti made in home with love and care", Price = 29.00m,
                    ImageUrl = "/images/Almond Biscotti.jpg", CategoryId = 3
                },
                new Product
                {
                    ProductId = 24, ProductName = "Fruit Cake",
                    Description = "Traditional fruit cake with mixed fruits and love", Price = 599.00m,
                    ImageUrl = "/images/Fruit Cake.jpg", CategoryId = 4
                },
                new Product
                {
                    ProductId = 25, ProductName = "Chocolate Fondant",
                    Description = "Rich chocolate fondant with a gooey center", Price = 399.00m,
                    ImageUrl = "/images/chocolate fondant.jpg", CategoryId = 4
                }
            );
        }
    }
}
