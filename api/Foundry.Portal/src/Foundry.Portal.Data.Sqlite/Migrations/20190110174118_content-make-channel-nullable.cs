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
    public partial class contentmakechannelnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=\"0\"", true);
            migrationBuilder.Sql(@"CREATE TABLE new_Contents(
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                    AuthorId INTEGER,
                    ChannelId INTEGER,
                    Created TEXT NOT NULL,
                    CreatedBy TEXT,
                    Description TEXT,
                    DifficultyAverage REAL NOT NULL,
                    DifficultyMedian INTEGER NOT NULL,
                    DifficultyTotal INTEGER NOT NULL,
                    GlobalId TEXT,
                    HoverUrl TEXT,
                    IsDisabled INTEGER NOT NULL,
                    IsRecommended INTEGER NOT NULL,
                    LogoUrl TEXT,
                    Name TEXT NOT NULL, 
                    ""Order"" INTEGER NOT NULL,
                    PublisherId INTEGER,
                    RatingAverage REAL NOT NULL,
                    RatingMedian INTEGER NOT NULL,
                    RatingTotal INTEGER NOT NULL,
                    Settings TEXT,
                    Tags TEXT,
                    ThumbnailUrl TEXT,
                    Type INTEGER NOT NULL,
                    Updated TEXT,
                    UpdatedBy TEXT,
                    Url TEXT,
                    IsFeatured INTEGER NOT NULL DEFAULT 0,
                    FlagCount INTEGER NOT NULL DEFAULT 0,
                    End TEXT,
                    Start TEXT,
                    Slug TEXT DEFAULT '_',
                    Imported TEXT,
                    ImportedBy TEXT,
                    FeaturedOrder INTEGER NOT NULL DEFAULT 0,
                    Copyright TEXT,
                    Summary TEXT,
                    TrailerUrl TEXT)");
            migrationBuilder.Sql("INSERT INTO new_Contents SELECT Id, AuthorId, ChannelId, Created, CreatedBy, Description, DifficultyAverage, DifficultyMedian, DifficultyTotal, GlobalId, HoverUrl, IsDisabled, IsRecommended, LogoUrl, Name, \"Order\", PublisherId, RatingAverage, RatingMedian, RatingTotal, Settings, Tags, ThumbnailUrl, Type, Updated, UpdatedBy, Url, IsFeatured, FlagCount, End, Start, Slug, Imported, ImportedBy, FeaturedOrder, Copyright, Summary, TrailerUrl FROM Contents");
            migrationBuilder.Sql("DROP TABLE Contents");
            migrationBuilder.Sql("ALTER TABLE new_Contents RENAME TO Contents");
            migrationBuilder.Sql("PRAGMA foreign_key_check");
            migrationBuilder.Sql("PRAGMA foreign_keys=\"1\"", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys=\"0\"", true);
            migrationBuilder.Sql(@"CREATE TABLE new_Contents(
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                    AuthorId INTEGER,
                    ChannelId INTEGER NOT NULL,
                    Created TEXT NOT NULL,
                    CreatedBy TEXT,
                    Description TEXT,
                    DifficultyAverage REAL NOT NULL,
                    DifficultyMedian INTEGER NOT NULL,
                    DifficultyTotal INTEGER NOT NULL,
                    GlobalId TEXT,
                    HoverUrl TEXT,
                    IsDisabled INTEGER NOT NULL,
                    IsRecommended INTEGER NOT NULL,
                    LogoUrl TEXT,
                    Name TEXT NOT NULL,
                    ""Order"" INTEGER NOT NULL,
                    PublisherId INTEGER,
                    RatingAverage REAL NOT NULL,
                    RatingMedian INTEGER NOT NULL,
                    RatingTotal INTEGER NOT NULL,
                    Settings TEXT,
                    Tags TEXT,
                    ThumbnailUrl TEXT,
                    Type INTEGER NOT NULL,
                    Updated TEXT,
                    UpdatedBy TEXT,
                    Url TEXT,
                    IsFeatured INTEGER NOT NULL DEFAULT 0,
                    FlagCount INTEGER NOT NULL DEFAULT 0,
                    End TEXT,
                    Start TEXT,
                    Slug TEXT DEFAULT '_',
                    Imported TEXT,
                    ImportedBy TEXT,
                    FeaturedOrder INTEGER NOT NULL DEFAULT 0,
                    Copyright TEXT,
                    Summary TEXT,
                    TrailerUrl TEXT,
                    FOREIGN KEY(ChannelId) REFERENCES Channels(Id))");
            migrationBuilder.Sql("INSERT INTO new_Contents SELECT Id, AuthorId, ChannelId, Created, CreatedBy, Description, DifficultyAverage, DifficultyMedian, DifficultyTotal, GlobalId, HoverUrl, IsDisabled, IsRecommended, LogoUrl, Name, \"Order\", PublisherId, RatingAverage, RatingMedian, RatingTotal, Settings, Tags, ThumbnailUrl, Type, Updated, UpdatedBy, Url, IsFeatured, FlagCount, End, Start, Slug, Imported, ImportedBy, FeaturedOrder, Copyright, Summary, TrailerUrl FROM Contents");
            migrationBuilder.Sql("DROP TABLE Contents");
            migrationBuilder.Sql("ALTER TABLE new_Contents RENAME TO Contents");
            migrationBuilder.Sql("PRAGMA foreign_key_check");
            migrationBuilder.Sql("PRAGMA foreign_keys=\"1\"", true);
        }
    }
}

