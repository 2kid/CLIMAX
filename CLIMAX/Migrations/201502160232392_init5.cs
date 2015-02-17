namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RoleTypes", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.Branches", "BranchName", c => c.String(nullable: false));
            AlterColumn("dbo.ChargeSlips", "ModeOfPayment", c => c.String(nullable: false));
            AlterColumn("dbo.Treatments", "TreatmentName", c => c.String(nullable: false));
            AlterColumn("dbo.Materials", "MaterialName", c => c.String(nullable: false));
            AlterColumn("dbo.UnitTypes", "Type", c => c.String(nullable: false));
            AlterColumn("dbo.Procedures", "ProcedureName", c => c.String(nullable: false));
            AlterColumn("dbo.ReportTypes", "Type", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReportTypes", "Type", c => c.String());
            AlterColumn("dbo.Procedures", "ProcedureName", c => c.String());
            AlterColumn("dbo.UnitTypes", "Type", c => c.String());
            AlterColumn("dbo.Materials", "MaterialName", c => c.String());
            AlterColumn("dbo.Treatments", "TreatmentName", c => c.String());
            AlterColumn("dbo.ChargeSlips", "ModeOfPayment", c => c.String());
            AlterColumn("dbo.Branches", "BranchName", c => c.String());
            AlterColumn("dbo.RoleTypes", "Type", c => c.String());
        }
    }
}
