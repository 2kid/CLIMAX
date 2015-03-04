namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
           
            CreateTable(
                "dbo.Procedures",
                c => new
                    {
                        ProcedureID = c.Int(nullable: false, identity: true),
                        ProcedureName = c.String(nullable: false),
                        TreatmentID = c.Int(nullable: false),
                        StepNo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProcedureID)
                .ForeignKey("dbo.Treatments", t => t.TreatmentID, cascadeDelete: true)
                .Index(t => t.TreatmentID);
            
          
            
        }
        
        public override void Down()
        {
          
        }
    }
}
