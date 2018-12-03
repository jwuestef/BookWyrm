using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookWyrm.Data.Models;

namespace BookWyrm.Data.DataContexts
{
    public class TransactionDb : DbContext
    {
        public TransactionDb()
            : base("BookWyrmConnection")
        {
        }



        // PUT MODELS HERE - EACH WILL GET IT'S OWN TABLE
        //public virtual DbSet<Author> Authors { get; set; }
        //public virtual DbSet<Course> Courses { get; set; }
        //public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<ChronJob> ChronJobs { get; set; }





        public static TransactionDb Create()
        {
            return new TransactionDb();
        }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // You can partition your DB into different schemas, supposedly helps with adminstrative tasks later for a DBA
            modelBuilder.HasDefaultSchema("transaction");

            // Entity configurations go here
            //modelBuilder.Entity<Course>()
            //    .Property(c => c.Name)
            //    .IsRequired()
            //    .HasMaxLength(255);



            modelBuilder.Entity<Transaction>()
                .Property(t => t.PersonId)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.DateApplied)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .IsRequired();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.BookId)
                .IsOptional();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Notes)
                .HasMaxLength(2000)
                .IsOptional();





            modelBuilder.Entity<ChronJob>()
                .Property(cj => cj.ChronJobId)
                .IsRequired();

            modelBuilder.Entity<ChronJob>()
                .Property(cj => cj.DateRan)
                .IsRequired();

            modelBuilder.Entity<ChronJob>()
                .Property(cj => cj.RandomKey)
                .IsRequired();

            





            // In Mosh's EF tutorial he commented this out when he moved the entity configurations to a different file
            // When I did it here, it complained heavily and I had to UNCOMMENT this
            base.OnModelCreating(modelBuilder);

            // Move the entity configurations to a different file
            //modelBuilder.Configurations.Add(new BookConfiguration());

        }

    }
}
