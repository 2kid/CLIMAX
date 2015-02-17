namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "treatment_TreatmentsID", "dbo.Treatments");
            DropIndex("dbo.Reservations", new[] { "treatment_TreatmentsID" });
            DropColumn("dbo.Reservations", "TreatmentID");
            RenameColumn(table: "dbo.Reservations", name: "treatment_TreatmentsID", newName: "TreatmentID");
            AlterColumn("dbo.Reservations", "TreatmentID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "TreatmentID");
            AddForeignKey("dbo.Reservations", "TreatmentID", "dbo.Treatments", "TreatmentsID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservations", "TreatmentID", "dbo.Treatments");
            DropIndex("dbo.Reservations", new[] { "TreatmentID" });
            AlterColumn("dbo.Reservations", "TreatmentID", c => c.Int());
            RenameColumn(table: "dbo.Reservations", name: "TreatmentID", newName: "treatment_TreatmentsID");
            AddColumn("dbo.Reservations", "TreatmentID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "treatment_TreatmentsID");
            AddForeignKey("dbo.Reservations", "treatment_TreatmentsID", "dbo.Treatments", "TreatmentsID");
        }
    }
}
