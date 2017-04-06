namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rename : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TLoginTSources", newName: "TSourceTLogins");
            DropPrimaryKey("dbo.TSourceTLogins");
            AddPrimaryKey("dbo.TSourceTLogins", new[] { "TSource_Id", "TLogin_Id" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TSourceTLogins");
            AddPrimaryKey("dbo.TSourceTLogins", new[] { "TLogin_Id", "TSource_Id" });
            RenameTable(name: "dbo.TSourceTLogins", newName: "TLoginTSources");
        }
    }
}
