namespace BookWyrm.Data.MigrationHistory.TransactionMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRandomKeyColumnToChronJobTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("transaction.ChronJobs", "RandomKey", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("transaction.ChronJobs", "RandomKey");
        }
    }
}
