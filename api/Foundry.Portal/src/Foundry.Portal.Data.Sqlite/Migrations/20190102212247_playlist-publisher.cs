/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

using Microsoft.EntityFrameworkCore.Migrations;

namespace Foundry.Portal.Data.SqliteMigrations
{
    public partial class playlistpublisher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=\"0\"", true);
            migrationBuilder.Sql(@"CREATE TABLE new_Playlists(
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                    Copyright TEXT,  
                    Created TEXT NOT NULL, 
                    CreatedBy TEXT, 
                    Description TEXT, 
                    FeaturedOrder INTEGER NOT NULL DEFAULT 0, 
                    GlobalId TEXT NOT NULL, 
                    Imported TEXT, 
                    ImportedBy TEXT, 
                    IsDefault INTEGER NOT NULL, 
                    IsDisabled INTEGER NOT NULL DEFAULT 0, 
                    IsFeatured INTEGER NOT NULL DEFAULT 0, 
                    IsPublic INTEGER NOT NULL, 
                    IsRecommended INTEGER NOT NULL DEFAULT 0, 
                    LogoUrl TEXT, 
                    Name TEXT NOT NULL, 
                    ProfileId INTEGER, 
                    RatingAverage REAL NOT NULL, 
                    RatingMedian INTEGER NOT NULL, 
                    RatingTotal INTEGER NOT NULL, 
                    Slug TEXT DEFAULT '_', 
                    Tags TEXT, 
                    Updated TEXT, 
                    UpdatedBy TEXT,
                    TrailerUrl TEXT,
                    Summary TEXT,
                    PublisherId INTEGER, FOREIGN KEY (PublisherId) REFERENCES Groups (Id) ON DELETE RESTRICT)");
            migrationBuilder.Sql("INSERT INTO new_Playlists SELECT Id, Copyright, Created, CreatedBy, Description, FeaturedOrder, GlobalId, Imported, ImportedBy, IsDefault, IsDisabled, IsFeatured, IsPublic, IsRecommended, LogoUrl, Name, ProfileId, RatingAverage, RatingMedian, RatingTotal, Slug, Tags, Updated, UpdatedBy, TrailerUrl, Summary, 'NULL' FROM Playlists");
            migrationBuilder.Sql("DROP TABLE Playlists");
            migrationBuilder.Sql("ALTER TABLE new_Playlists RENAME TO Playlists");
            migrationBuilder.Sql("CREATE INDEX IX_Playlists_ProfileId ON Playlists (ProfileId)");
            migrationBuilder.Sql("PRAGMA foreign_key_check");
            migrationBuilder.Sql("PRAGMA foreign_keys=\"1\"", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=\"0\"", true);
            migrationBuilder.Sql(@"CREATE TABLE new_Playlists(
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                    Copyright TEXT,  
                    Created TEXT NOT NULL, 
                    CreatedBy TEXT, 
                    Description TEXT, 
                    FeaturedOrder INTEGER NOT NULL DEFAULT 0, 
                    GlobalId TEXT, 
                    Imported TEXT, 
                    ImportedBy TEXT, 
                    IsDefault INTEGER NOT NULL, 
                    IsDisabled INTEGER NOT NULL DEFAULT 0, 
                    IsFeatured INTEGER NOT NULL DEFAULT 0, 
                    IsPublic INTEGER NOT NULL, 
                    IsRecommended INTEGER NOT NULL DEFAULT 0, 
                    LogoUrl TEXT, 
                    Name TEXT, 
                    ProfileId INTEGER, 
                    RatingAverage REAL NOT NULL, 
                    RatingMedian INTEGER NOT NULL, 
                    RatingTotal INTEGER NOT NULL, 
                    Slug TEXT DEFAULT '_', 
                    Tags TEXT, 
                    Updated TEXT, 
                    UpdatedBy TEXT,
                    TrailerUrl TEXT,
                    Summary TEXT)");
            migrationBuilder.Sql("INSERT INTO new_Playlists SELECT Id, Copyright, Created, CreatedBy, Description, FeaturedOrder, GlobalId, Imported, ImportedBy, IsDefault, IsDisabled, IsFeatured, IsPublic, IsRecommended, LogoUrl, Name, ProfileId, RatingAverage, RatingMedian, RatingTotal, Slug, Tags, Updated, UpdatedBy, TrailerUrl, Summary FROM Playlists");
            migrationBuilder.Sql("DROP TABLE Playlists");
            migrationBuilder.Sql("ALTER TABLE new_Playlists RENAME TO Playlists");
            migrationBuilder.Sql("CREATE INDEX IX_Playlists_ProfileId ON Playlists (ProfileId)");
            migrationBuilder.Sql("PRAGMA foreign_key_check");
            migrationBuilder.Sql("PRAGMA foreign_keys=\"1\"", true);
        }
    }
}

