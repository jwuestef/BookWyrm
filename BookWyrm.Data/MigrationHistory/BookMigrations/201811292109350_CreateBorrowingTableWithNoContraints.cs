namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBorrowingTableWithNoContraints : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "book.Borrowings",
                c => new
                    {
                        BorrowingId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(),
                        BookId = c.String(),
                        CheckOutDateTime = c.DateTime(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        CheckInDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BorrowingId);
            
        }
        
        public override void Down()
        {
            DropTable("book.Borrowings");
        }
    }
}
