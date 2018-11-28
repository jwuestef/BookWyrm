using System.Data.Entity.ModelConfiguration;
using BookWyrm.Data.Models;

namespace BookWyrm.Data.DataContexts
{
    internal class BookConfiguration : EntityTypeConfiguration<Book>
    {
        public BookConfiguration()
        {

            // Table overrides at the very beginning

            //ToTable("tbl_Course");



            // Then override primary keys

            //HasKey(c => c.Id);



            // Next property configurations, sorted alphabetically

            //Property(c => c.Description)
            //.IsRequired()
            //.HasMaxLength(2000);

            //Property(c => c.Name)
            //.IsRequired()
            //.HasMaxLength(255);

            //public string Title { get; set; }
            //public string Author { get; set; }
            //public int YearPublished { get; set; }
            //public string Genre { get; set; }
            //public string Keywords { get; set; }
            //public string Description { get; set; }
            //public string ISBN { get; set; }
            //public string Barcode { get; set; }
            //public int MinAgeReq { get; set; }
            //public string HiddenNotes { get; set; }

            Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(255);

            Property(b => b.Author)
                .IsRequired()
                .HasMaxLength(255);

            Property(b => b.YearPublished)
                .IsRequired();

            Property(b => b.Genre)
                .IsRequired()
                .HasMaxLength(255);

            Property(b => b.Keywords)
                .IsRequired()
                .HasMaxLength(2000);

            Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(2000);

            Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(255);

            Property(b => b.Barcode)
                .IsRequired()
                .HasMaxLength(255);

            Property(b => b.MinAgeReq)
                .IsRequired();

            Property(b => b.HiddenNotes)
                .HasMaxLength(2000);




            // After properties come relationships, again alphabetically
            //HasRequired(c => c.Author)
            //.WithMany(a => a.Courses)
            //.HasForeignKey(c => c.AuthorId)
            //.WillCascadeOnDelete(false);

            //HasRequired(c => c.Cover)
            //.WithRequiredPrincipal(c => c.Course);

            //HasMany(c => c.Tags)
            //.WithMany(t => t.Courses)
            //.Map(m =>
            //{
            //    m.ToTable("CourseTags");
            //    m.MapLeftKey("CourseId");
            //    m.MapRightKey("TagId");
            //});



        }
    }
}