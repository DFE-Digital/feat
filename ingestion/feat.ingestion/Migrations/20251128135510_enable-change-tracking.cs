using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace feat.ingestion.Migrations
{
    /// <inheritdoc />
    public partial class enablechangetracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Turn on change tracking for the DB if it's not already on
            migrationBuilder.Sql(@"
Declare @DBNAME SYSNAME = DB_NAME(), @Sql    NVARCHAR(MAX);
IF NOT EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID(@DBNAME))
Begin
    SET @Sql = N'ALTER DATABASE '+  QUOTENAME(@DBNAME) + N' SET CHANGE_TRACKING = ON 
(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)'
    Exec sp_executesql  @Sql
End");
            
            // Enable change tracking for the AiSearchEntries table
            migrationBuilder.Sql(
                @"IF NOT EXISTS(SELECT 1 FROM sys.change_tracking_tables WHERE object_id=OBJECT_ID('AiSearchEntries'))
Begin
    ALTER TABLE [dbo].[AiSearchEntries] ENABLE CHANGE_TRACKING  WITH (TRACK_COLUMNS_UPDATED = ON)
End");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Disable change tracking for the AiSearchEntries table
            migrationBuilder.Sql(
                @"IF EXISTS(SELECT 1 FROM sys.change_tracking_tables WHERE object_id=OBJECT_ID('AiSearchEntries'))
Begin
    ALTER TABLE [dbo].[AiSearchEntries] DISABLE CHANGE_TRACKING
End");
            
            // Turn off change tracking for the DB if it's on
            migrationBuilder.Sql(@"
Declare @DBNAME SYSNAME = DB_NAME(), @Sql NVARCHAR(MAX);
IF EXISTS (SELECT 1 FROM sys.change_tracking_databases WHERE database_id = DB_ID(@DBNAME))
Begin
    SET @Sql = N'ALTER DATABASE '+  QUOTENAME(@DBNAME) + N' SET CHANGE_TRACKING = OFF'
    Exec sp_executesql  @Sql
End");
        }
    }
}
