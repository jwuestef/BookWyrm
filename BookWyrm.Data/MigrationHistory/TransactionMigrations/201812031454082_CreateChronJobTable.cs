namespace BookWyrm.Data.MigrationHistory.TransactionMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateChronJobTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "transaction.ChronJobs",
                c => new
                    {
                        ChronJobId = c.Int(nullable: false, identity: true),
                        DateRan = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChronJobId);
            
        }
        
        public override void Down()
        {
            DropTable("transaction.ChronJobs");
        }
    }
}
