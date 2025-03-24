namespace StudentManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class streamtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "SelectedStream", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "SelectedStream");
        }
    }
}
