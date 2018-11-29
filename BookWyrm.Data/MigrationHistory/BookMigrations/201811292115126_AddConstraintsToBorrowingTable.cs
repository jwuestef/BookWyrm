namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddConstraintsToBorrowingTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("book.Borrowings", "UserId", c => c.String(nullable: false));
            AlterColumn("book.Borrowings", "BookId", c => c.String(nullable: false));
            AlterColumn("book.Borrowings", "CheckInDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("book.Borrowings", "CheckInDateTime", c => c.DateTime(nullable: false));
            AlterColumn("book.Borrowings", "BookId", c => c.String());
            AlterColumn("book.Borrowings", "UserId", c => c.String());
        }
    }
}
