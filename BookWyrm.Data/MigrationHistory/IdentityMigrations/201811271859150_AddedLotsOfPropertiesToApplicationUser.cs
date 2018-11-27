namespace BookWyrm.Data.MigrationHistory.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLotsOfPropertiesToApplicationUser : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "identity.AspNetUsers", newName: "ApplicationUsers");
            DropForeignKey("identity.AspNetUserClaims", "UserId", "identity.AspNetUsers");
            DropForeignKey("identity.AspNetUserLogins", "UserId", "identity.AspNetUsers");
            DropForeignKey("identity.AspNetUserRoles", "UserId", "identity.AspNetUsers");
            DropIndex("identity.AspNetUserRoles", new[] { "UserId" });
            DropIndex("identity.ApplicationUsers", "UserNameIndex");
            DropIndex("identity.AspNetUserClaims", new[] { "UserId" });
            DropIndex("identity.AspNetUserLogins", new[] { "UserId" });
            AddColumn("identity.AspNetUserRoles", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("identity.ApplicationUsers", "FirstName", c => c.String());
            AddColumn("identity.ApplicationUsers", "LastName", c => c.String());
            AddColumn("identity.ApplicationUsers", "BirthDate", c => c.DateTime(nullable: false));
            AddColumn("identity.ApplicationUsers", "Address", c => c.String());
            AddColumn("identity.ApplicationUsers", "Barcode", c => c.String());
            AddColumn("identity.ApplicationUsers", "Balance", c => c.Int(nullable: false));
            AddColumn("identity.ApplicationUsers", "HiddenNotes", c => c.String());
            AddColumn("identity.AspNetUserClaims", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("identity.AspNetUserLogins", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AlterColumn("identity.ApplicationUsers", "Email", c => c.String());
            AlterColumn("identity.ApplicationUsers", "UserName", c => c.String());
            AlterColumn("identity.AspNetUserClaims", "UserId", c => c.String());
            CreateIndex("identity.AspNetUserRoles", "ApplicationUser_Id");
            CreateIndex("identity.AspNetUserClaims", "ApplicationUser_Id");
            CreateIndex("identity.AspNetUserLogins", "ApplicationUser_Id");
            AddForeignKey("identity.AspNetUserClaims", "ApplicationUser_Id", "identity.ApplicationUsers", "Id");
            AddForeignKey("identity.AspNetUserLogins", "ApplicationUser_Id", "identity.ApplicationUsers", "Id");
            AddForeignKey("identity.AspNetUserRoles", "ApplicationUser_Id", "identity.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("identity.AspNetUserRoles", "ApplicationUser_Id", "identity.ApplicationUsers");
            DropForeignKey("identity.AspNetUserLogins", "ApplicationUser_Id", "identity.ApplicationUsers");
            DropForeignKey("identity.AspNetUserClaims", "ApplicationUser_Id", "identity.ApplicationUsers");
            DropIndex("identity.AspNetUserLogins", new[] { "ApplicationUser_Id" });
            DropIndex("identity.AspNetUserClaims", new[] { "ApplicationUser_Id" });
            DropIndex("identity.AspNetUserRoles", new[] { "ApplicationUser_Id" });
            AlterColumn("identity.AspNetUserClaims", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("identity.ApplicationUsers", "UserName", c => c.String(nullable: false, maxLength: 256));
            AlterColumn("identity.ApplicationUsers", "Email", c => c.String(maxLength: 256));
            DropColumn("identity.AspNetUserLogins", "ApplicationUser_Id");
            DropColumn("identity.AspNetUserClaims", "ApplicationUser_Id");
            DropColumn("identity.ApplicationUsers", "HiddenNotes");
            DropColumn("identity.ApplicationUsers", "Balance");
            DropColumn("identity.ApplicationUsers", "Barcode");
            DropColumn("identity.ApplicationUsers", "Address");
            DropColumn("identity.ApplicationUsers", "BirthDate");
            DropColumn("identity.ApplicationUsers", "LastName");
            DropColumn("identity.ApplicationUsers", "FirstName");
            DropColumn("identity.AspNetUserRoles", "ApplicationUser_Id");
            CreateIndex("identity.AspNetUserLogins", "UserId");
            CreateIndex("identity.AspNetUserClaims", "UserId");
            CreateIndex("identity.ApplicationUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("identity.AspNetUserRoles", "UserId");
            AddForeignKey("identity.AspNetUserRoles", "UserId", "identity.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("identity.AspNetUserLogins", "UserId", "identity.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("identity.AspNetUserClaims", "UserId", "identity.AspNetUsers", "Id", cascadeDelete: true);
            RenameTable(name: "identity.ApplicationUsers", newName: "AspNetUsers");
        }
    }
}
