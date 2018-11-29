namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InBorrowingTableConvertBookIdFromStringToInt : DbMigration
    {
        public override void Up()
        {
            AlterColumn("book.Borrowings", "BookId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("book.Borrowings", "BookId", c => c.String(nullable: false));
        }
    }
}
