namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fullNewsV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TSources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Link = c.String(),
                        Type = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TArticles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(),
                        Version = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        TSourceId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TSources", t => t.TSourceId, cascadeDelete: true)
                .Index(t => t.TSourceId);
            
            CreateTable(
                "dbo.TLogins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TPreferableWords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.String(),
                        IsMust = c.Boolean(nullable: false),
                        TLoginId = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Modified = c.DateTime(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TLogins", t => t.TLoginId, cascadeDelete: true)
                .Index(t => t.TLoginId);
            
            CreateTable(
                "dbo.TLoginTSources",
                c => new
                    {
                        TLogin_Id = c.Int(nullable: false),
                        TSource_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TLogin_Id, t.TSource_Id })
                .ForeignKey("dbo.TLogins", t => t.TLogin_Id, cascadeDelete: true)
                .ForeignKey("dbo.TSources", t => t.TSource_Id, cascadeDelete: true)
                .Index(t => t.TLogin_Id)
                .Index(t => t.TSource_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TLoginTSources", "TSource_Id", "dbo.TSources");
            DropForeignKey("dbo.TLoginTSources", "TLogin_Id", "dbo.TLogins");
            DropForeignKey("dbo.TPreferableWords", "TLoginId", "dbo.TLogins");
            DropForeignKey("dbo.TArticles", "TSourceId", "dbo.TSources");
            DropIndex("dbo.TLoginTSources", new[] { "TSource_Id" });
            DropIndex("dbo.TLoginTSources", new[] { "TLogin_Id" });
            DropIndex("dbo.TPreferableWords", new[] { "TLoginId" });
            DropIndex("dbo.TArticles", new[] { "TSourceId" });
            DropTable("dbo.TLoginTSources");
            DropTable("dbo.TPreferableWords");
            DropTable("dbo.TLogins");
            DropTable("dbo.TArticles");
            DropTable("dbo.TSources");
        }
    }
}
