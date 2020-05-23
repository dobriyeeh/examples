namespace CMSController.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CLASSIFICATION : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EntityInformations", "EntityClassification", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EntityInformations", "EntityClassification");
        }
    }
}
