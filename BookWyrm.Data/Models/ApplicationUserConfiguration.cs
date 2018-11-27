using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWyrm.Data.Models
{
    class ApplicationUserConfiguration : EntityTypeConfiguration<ApplicationUser>
    {
        public ApplicationUserConfiguration()
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


            //Property(u => u.FirstName)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //Property(u => u.LastName)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //Property(u => u.BirthDate)
            //    .IsRequired();

            //Property(u => u.Address)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //Property(u => u.Barcode)
            //    .IsRequired()
            //    .HasMaxLength(255);

            //Property(u => u.Balance)
            //    .IsRequired();

            //Property(u => u.HiddenNotes)
            //    .IsOptional()
            //    .HasMaxLength(2000);




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
