namespace CMSController.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MAKE_NUMBER_AS_LONG : DbMigration
    {
        public override void Up()
        {
            Sql("alter table EntityInformations alter column EntityNumber BIGINT");
        }
        
        public override void Down()
        {
        }
    }
}
