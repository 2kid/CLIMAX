namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Medicine_ChargeSlip", "MaterialID_MaterialID", "dbo.Materials");
            DropIndex("dbo.Medicine_ChargeSlip", new[] { "MaterialID_MaterialID" });
            RenameColumn(table: "dbo.Medicine_ChargeSlip", name: "MaterialID_MaterialID", newName: "MaterialID");
            AlterColumn("dbo.Medicine_ChargeSlip", "MaterialID", c => c.Int(nullable: false));
            CreateIndex("dbo.Medicine_ChargeSlip", "MaterialID");
            AddForeignKey("dbo.Medicine_ChargeSlip", "MaterialID", "dbo.Materials", "MaterialID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Medicine_ChargeSlip", "MaterialID", "dbo.Materials");
            DropIndex("dbo.Medicine_ChargeSlip", new[] { "MaterialID" });
            AlterColumn("dbo.Medicine_ChargeSlip", "MaterialID", c => c.Int());
            RenameColumn(table: "dbo.Medicine_ChargeSlip", name: "MaterialID", newName: "MaterialID_MaterialID");
            CreateIndex("dbo.Medicine_ChargeSlip", "MaterialID_MaterialID");
            AddForeignKey("dbo.Medicine_ChargeSlip", "MaterialID_MaterialID", "dbo.Materials", "MaterialID");
        }
    }
}
