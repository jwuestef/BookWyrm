using System.Data.Entity.ModelConfiguration;
using BookWyrm.Data.Models;

namespace BookWyrm.Data.DataContexts
{
    internal class BorrowingConfiguration : EntityTypeConfiguration<Borrowing>
    {
        public BorrowingConfiguration()
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

            Property(b => b.UserId)
                .IsRequired();

            Property(b => b.BookId)
                .IsRequired();

            Property(b => b.CheckOutDateTime)
                .IsRequired();

            Property(b => b.DueDate)
                .IsRequired();

            Property(b => b.CheckInDateTime)
                .IsOptional();






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