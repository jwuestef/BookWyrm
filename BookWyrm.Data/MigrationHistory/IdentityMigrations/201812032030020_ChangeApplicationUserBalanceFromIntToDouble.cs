namespace BookWyrm.Data.MigrationHistory.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeApplicationUserBalanceFromIntToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("identity.AspNetUsers", "Balance", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("identity.AspNetUsers", "Balance", c => c.Int(nullable: false));
        }
    }
}
