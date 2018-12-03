namespace BookWyrm.Data.MigrationHistory.TransactionMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "transaction.Transactions",
                c => new
                    {
                        TransactionId = c.Guid(nullable: false),
                        PersonId = c.String(nullable: false, maxLength: 255),
                        DateApplied = c.DateTime(nullable: false),
                        Amount = c.Double(nullable: false),
                        BookId = c.Int(),
                        Notes = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.TransactionId);
            
        }
        
        public override void Down()
        {
            DropTable("transaction.Transactions");
        }
    }
}
