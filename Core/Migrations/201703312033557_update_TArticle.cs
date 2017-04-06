namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_TArticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TArticles", "OwnLink", c => c.String());
            AddColumn("dbo.TArticles", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TArticles", "Title");
            DropColumn("dbo.TArticles", "OwnLink");
        }
    }
}
