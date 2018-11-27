using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookWyrm.Data.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookWyrm.Data.DataContexts
{
    public class IdentityDb : IdentityDbContext<ApplicationUser>
    {
        public IdentityDb()
            : base("BookWyrmConnection", throwIfV1Schema: false)
        {
        }


        // PUT TABLES HERE
        //public virtual DbSet<Author> Authors { get; set; }
        //public virtual DbSet<Course> Courses { get; set; }
        //public virtual DbSet<Tag> Tags { get; set; }





        public static IdentityDb Create()
        {
            return new IdentityDb();
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // You can partition your DB into different schemas, supposedly helps with adminstrative tasks later for a DBA
            modelBuilder.HasDefaultSchema("identity");

            // Entity configurations go here
            //modelBuilder.Entity<Course>()
            //    .Property(c => c.Name)
            //    .IsRequired()
            //    .HasMaxLength(255);


            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.BirthDate)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Address)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Barcode)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Balance)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.HiddenNotes)
                .IsOptional()
                .HasMaxLength(2000);






            // In Mosh's EF tutorial he commented this out when he moved the entity configurations to a different file
            // When I did it here, it complained heavily and I had to UNCOMMENT this
            base.OnModelCreating(modelBuilder);

            // Move the entity configurations to a different file
            //modelBuilder.Configurations.Add(new ApplicationUserConfiguration());
            //modelBuilder.Configurations.Add(new BookConfiguration());
            //modelBuilder.Configurations.Add(new BorrowingConfiguration());
            //modelBuilder.Configurations.Add(new TransactionConfiguration());

        }



    }
}
