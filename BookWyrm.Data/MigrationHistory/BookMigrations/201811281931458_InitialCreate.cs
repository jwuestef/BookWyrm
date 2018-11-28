namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "book.Books",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Author = c.String(nullable: false, maxLength: 255),
                        YearPublished = c.Int(nullable: false),
                        Genre = c.String(nullable: false, maxLength: 255),
                        Keywords = c.String(nullable: false, maxLength: 2000),
                        Description = c.String(nullable: false, maxLength: 2000),
                        ISBN = c.String(nullable: false, maxLength: 255),
                        Barcode = c.String(nullable: false, maxLength: 255),
                        MinAgeReq = c.Int(nullable: false),
                        HiddenNotes = c.String(nullable: false, maxLength: 2000),
                    })
                .PrimaryKey(t => t.BookId);
            
        }
        
        public override void Down()
        {
            DropTable("book.Books");
        }
    }
}
