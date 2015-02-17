namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChargeSlips", "AmtDue", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChargeSlips", "AmtDue");
        }
    }
}
