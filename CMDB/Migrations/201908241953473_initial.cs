namespace CMDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ConfigurationItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Version = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfigurationItemConfigurationItems",
                c => new
                    {
                        ConfigurationItem_Id = c.Int(nullable: false),
                        ConfigurationItem_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ConfigurationItem_Id, t.ConfigurationItem_Id1 })
                .ForeignKey("dbo.ConfigurationItems", t => t.ConfigurationItem_Id)
                .ForeignKey("dbo.ConfigurationItems", t => t.ConfigurationItem_Id1)
                .Index(t => t.ConfigurationItem_Id)
                .Index(t => t.ConfigurationItem_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConfigurationItemConfigurationItems", "ConfigurationItem_Id1", "dbo.ConfigurationItems");
            DropForeignKey("dbo.ConfigurationItemConfigurationItems", "ConfigurationItem_Id", "dbo.ConfigurationItems");
            DropIndex("dbo.ConfigurationItemConfigurationItems", new[] { "ConfigurationItem_Id1" });
            DropIndex("dbo.ConfigurationItemConfigurationItems", new[] { "ConfigurationItem_Id" });
            DropTable("dbo.ConfigurationItemConfigurationItems");
            DropTable("dbo.ConfigurationItems");
        }
    }
}
