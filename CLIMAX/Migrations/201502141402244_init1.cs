namespace CLIMAX.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.Reservations", new[] { "EmployeeID" });
            AlterColumn("dbo.Reservations", "EmployeeID", c => c.Int());
            CreateIndex("dbo.Reservations", "EmployeeID");
            AddForeignKey("dbo.Reservations", "EmployeeID", "dbo.Employees", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reservations", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.Reservations", new[] { "EmployeeID" });
            AlterColumn("dbo.Reservations", "EmployeeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "EmployeeID");
            AddForeignKey("dbo.Reservations", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
        }
    }
}
