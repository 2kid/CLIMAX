namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init6 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChargeSlips", "GiftCertificateAmt", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChargeSlips", "GiftCertificateAmt", c => c.Double(nullable: false));
        }
    }
}
