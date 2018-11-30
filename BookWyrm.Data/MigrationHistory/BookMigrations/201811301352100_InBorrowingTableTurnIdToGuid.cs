namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InBorrowingTableTurnIdToGuid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("book.Borrowings");

            DropColumn("book.Borrowings", "BorrowingId");
            AddColumn("book.Borrowings", "BorrowingId", c => c.Guid(nullable: false, identity: true));

            AddPrimaryKey("book.Borrowings", "BorrowingId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("book.Borrowings");

            DropColumn("book.Borrowings", "BorrowingId");
            AddColumn("book.Borrowings", "BorrowingId", c => c.Int(nullable: false, identity: true));

            AddPrimaryKey("book.Borrowings", "BorrowingId");
        }
    }
}
