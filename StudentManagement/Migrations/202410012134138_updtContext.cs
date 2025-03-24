namespace StudentManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updtContext : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Streams",
                c => new
                    {
                        StreamId = c.Int(nullable: false, identity: true),
                        StreamName = c.String(),
                    })
                .PrimaryKey(t => t.StreamId);
            
            AddColumn("dbo.Courses", "Stream_StreamId", c => c.Int());
            CreateIndex("dbo.Courses", "Stream_StreamId");
            AddForeignKey("dbo.Courses", "Stream_StreamId", "dbo.Streams", "StreamId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Courses", "Stream_StreamId", "dbo.Streams");
            DropIndex("dbo.Courses", new[] { "Stream_StreamId" });
            DropColumn("dbo.Courses", "Stream_StreamId");
            DropTable("dbo.Streams");
        }
    }
}
