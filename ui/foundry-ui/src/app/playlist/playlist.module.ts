/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { NgModule } from '@angular/core';
import { CommentEditDialog } from '../content/components/comments/comments.component';
import { FlagDialog } from '../content/components/detail/flag-dialog.component';
import { ContentService } from '../content/content.service';
import { DiscussionService } from '../discussion/discussion.service';
import { DocumentModule } from '../document/document.module';
import { GroupService } from '../group/group.service';
import { ImagesModule } from '../images/images.module';
import { ImagesService } from '../images/images.service';
import { SharedModule } from '../shared/shared.module';
import { PlaylistBrowserComponent } from './components/browser/browser.component';
import { PlaylistDetailComponent } from './components/detail/detail.component';
import { PlaylistEditComponent } from './components/edit/edit.component';
import { ExportPlaylistDialog } from './components/manage/export.playlist.dialog';
import { PlaylistManageComponent } from './components/manage/manage.component';
import { PlaylistRatingsComponent } from './components/rating/ratings.component';
import { PlaylistRoutingModule } from './playlist-routing.module';
import { PlaylistComponent } from './playlist.component';
import { PlaylistService } from './playlist.service';
import { GameBoardComponent } from './components/game-board/game-board.component';

@NgModule({
    imports: [
        SharedModule,
        DocumentModule,
        PlaylistRoutingModule,
        ImagesModule
    ],
    declarations: [
        PlaylistComponent,
        PlaylistBrowserComponent,
        PlaylistDetailComponent,
        PlaylistManageComponent,
        PlaylistEditComponent,
        ExportPlaylistDialog,
        PlaylistRatingsComponent,
        GameBoardComponent
    ],
    providers: [ PlaylistService, DiscussionService, GroupService, ContentService, ImagesService ],
    exports: [
        ExportPlaylistDialog,
        PlaylistManageComponent,
    ],
    entryComponents: [ExportPlaylistDialog, CommentEditDialog, PlaylistRatingsComponent, FlagDialog]

})
export class PlaylistModule {}

