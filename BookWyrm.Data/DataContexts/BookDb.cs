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
    public class BookDb : DbContext
    {
        public BookDb()
            : base("BookWyrmConnection")
        {
        }



        // PUT MODELS HERE - EACH WILL GET IT'S OWN TABLE
        //public virtual DbSet<Author> Authors { get; set; }
        //public virtual DbSet<Course> Courses { get; set; }
        //public virtual DbSet<Tag> Tags { get; set; }

        
        
        
        
        public static BookDb Create()
        {
            return new BookDb();
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // You can partition your DB into different schemas, supposedly helps with adminstrative tasks later for a DBA
            modelBuilder.HasDefaultSchema("book");

            // Entity configurations go here
            //modelBuilder.Entity<Course>()
            //    .Property(c => c.Name)
            //    .IsRequired()
            //    .HasMaxLength(255);












            // In Mosh's EF tutorial he commented this out when he moved the entity configurations to a different file
            // When I did it here, it complained heavily and I had to UNCOMMENT this
            //base.OnModelCreating(modelBuilder);

            // Move the entity configurations to a different file
            modelBuilder.Configurations.Add(new BookConfiguration());

        }



    }
}
