using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using QuickShop.Models;

namespace QuickShop.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Person> Persons {get; set;}
        public DbSet<Product> Products {get; set;}
        public DbSet<ProductImage> ProductImages {get; set;}
        public DbSet<ProductPrice> ProductPrices {get; set;}
        public DbSet<ProductRating> ProductRatings {get; set;}
        public DbSet<ProductTransaction> ProductTransactions {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<DeliveryType> DeliveryTypes {get; set;}
        public DbSet<DeliveryTypePrice> DeliveryTypePrices {get; set;}
        public DbSet<Chat> Chats {get; set;}
        public DbSet<ChatEntry> ChatEntries {get; set;}

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
            .HasOne(product => product.Person)
            .WithMany(person => person.Products)
            .HasForeignKey(product => product.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>()
            .HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductImage>()
            .HasOne(productImage => productImage.Product)
            .WithMany(product => product.ProductImages)
            .HasForeignKey(productImage => productImage.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductPrice>()
            .HasOne(productPrice => productPrice.Product)
            .WithMany(product => product.ProductPrices)
            .HasForeignKey(productPrice => productPrice.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductRating>()
            .HasOne(productRating => productRating.Product)
            .WithMany(product => product.ProductRatings)
            .HasForeignKey(productRating => productRating.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductRating>()
            .HasOne(productRating => productRating.Person)
            .WithMany(person => person.ProductRatings)
            .HasForeignKey(productRating => productRating.PersonId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.Product)
            .WithMany(product => product.ProductTransactions)
            .HasForeignKey(productTransaction => productTransaction.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.Person)
            .WithMany(person => person.ProductTransactions)
            .HasForeignKey(productTransaction => productTransaction.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.ProductPrice)
            .WithMany(productPrice => productPrice.ProductTransactions)
            .HasForeignKey(productTransaction => productTransaction.ProductPriceId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.DeliveryType)
            .WithMany(deliveryType => deliveryType.ProductTransactions)
            .HasForeignKey(productTransaction => productTransaction.DeliveryTypeId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.DeliveryTypePrice)
            .WithMany(deliveryTypePrice => deliveryTypePrice.ProductTransactions)
            .HasForeignKey(productTransaction => productTransaction.DeliveryTypePriceId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<ProductTransaction>()
            .HasOne(productTransaction => productTransaction.Chat)
            .WithOne(chat => chat.ProductTransaction)
            .HasForeignKey<ProductTransaction>(productTransaction => productTransaction.ChatId);

            builder.Entity<Chat>()
            .HasOne(chat => chat.ProductTransaction)
            .WithOne(productTransaction => productTransaction.Chat)
            .HasForeignKey<Chat>(chat => chat.ProductTransactionId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatEntry>()
            .HasOne(chatEntry => chatEntry.Chat)
            .WithMany(chat => chat.ChatEntries)
            .HasForeignKey(chatEntry => chatEntry.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChatEntry>()
            .HasOne(chatEntry => chatEntry.Person)
            .WithMany(person => person.ChatEntries)
            .HasForeignKey(chatEntry => chatEntry.PersonId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<DeliveryTypePrice>()
            .HasOne(deliveryTypePrice => deliveryTypePrice.DeliveryType)
            .WithMany(deliveryType => deliveryType.DeliveryTypePrices)
            .HasForeignKey(deliveryTypePrice => deliveryTypePrice.DeliveryTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}