namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InBorrowingTableTroubleshootingPrimaryKey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("book.Borrowings");
            AlterColumn("book.Borrowings", "BorrowingId", c => c.Int(nullable: false));
            AddPrimaryKey("book.Borrowings", "BorrowingId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("book.Borrowings");
            AlterColumn("book.Borrowings", "BorrowingId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("book.Borrowings", "BorrowingId");
        }
    }
}
