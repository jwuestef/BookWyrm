// <auto-generated />
namespace BookWyrm.Data.MigrationHistory.BookMigrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.2.0-61023")]
    public sealed partial class InBorrowingTableConvertIdFromStringToInt : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(InBorrowingTableConvertIdFromStringToInt));
        
        string IMigrationMetadata.Id
        {
            get { return "201811292158114_InBorrowingTableConvertIdFromStringToInt"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}