namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeHiddenNotesNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("book.Books", "HiddenNotes", c => c.String(maxLength: 2000));
        }
        
        public override void Down()
        {
            AlterColumn("book.Books", "HiddenNotes", c => c.String(nullable: false, maxLength: 2000));
        }
    }
}
