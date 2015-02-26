namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inventories", "BranchID", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetRoles", "Description", c => c.String());
            AddColumn("dbo.AspNetRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Inventories", "BranchID");
            AddForeignKey("dbo.Inventories", "BranchID", "dbo.Branches", "BranchID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Inventories", "BranchID", "dbo.Branches");
            DropIndex("dbo.Inventories", new[] { "BranchID" });
            DropColumn("dbo.AspNetRoles", "Discriminator");
            DropColumn("dbo.AspNetRoles", "Description");
            DropColumn("dbo.Inventories", "BranchID");
        }
    }
}
