namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChargeSlips", "DiscountRate", c => c.Int());
            AddColumn("dbo.ChargeSlips", "AmtDiscount", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChargeSlips", "AmtDiscount");
            DropColumn("dbo.ChargeSlips", "DiscountRate");
        }
    }
}
