namespace Samurai.SqlDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addinplaytomatch : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TennisPredictionStats", "MatchID_fk", "dbo.Matches");
            DropIndex("dbo.TennisPredictionStats", new[] { "MatchID_fk" });
            AddColumn("dbo.Matches", "InPlay", c => c.Boolean(nullable: false));
            AddForeignKey("dbo.TennisPredictionStats", "MatchID_fk", "dbo.Matches", "MatchID_pk", cascadeDelete: true);
            CreateIndex("dbo.TennisPredictionStats", "MatchID_fk");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TennisPredictionStats", new[] { "MatchID_fk" });
            DropForeignKey("dbo.TennisPredictionStats", "MatchID_fk", "dbo.Matches");
            DropColumn("dbo.Matches", "InPlay");
            CreateIndex("dbo.TennisPredictionStats", "MatchID_fk");
            AddForeignKey("dbo.TennisPredictionStats", "MatchID_fk", "dbo.Matches", "MatchID_pk");
        }
    }
}
