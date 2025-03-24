namespace StudentManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdNumberToStudent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "IdNumber", c => c.String(nullable: false, maxLength: 13));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "IdNumber");
        }
    }
}
