/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DiscussionModule } from '../discussion/discussion.module';
import { DiscussionService } from '../discussion/discussion.service';
import { DocumentModule } from '../document/document.module';
import { GroupService } from '../group/group.service';
import { ImagesModule } from '../images/images.module';
import { ImagesService } from '../images/images.service';
import { PlaylistService } from '../playlist/playlist.service';
import { AdvancedSelectDialogComponent } from '../shared/components/advanced-select-dialog/advanced-select-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { ContentBrowserComponent } from './components/browser/browser.component';
import { CommentEditDialog } from './components/comments/comments.component';
import { FlagDialog } from './components/detail/flag-dialog.component';
import { ContentEditComponent } from './components/edit/edit.component';
import { ExportDialog } from './components/manage/export.dialog';
import { ContentQuickEditComponent } from './components/quick-edit/quick-edit.component';
import { ContentRoutingModule } from './content-routing.module';
import { ContentComponent } from './content.component';
import { ContentService } from './content.service';

@NgModule({
    declarations: [
        ContentBrowserComponent,
        ContentQuickEditComponent,
        ContentEditComponent,
        ContentComponent
    ],
    imports: [
        SharedModule,
        ContentRoutingModule,
        DiscussionModule,
        DocumentModule,
        CommonModule,
        ImagesModule,
    ],
    exports: [
        ContentBrowserComponent,
        ContentQuickEditComponent,
        ContentEditComponent,
        ContentComponent
    ],
    entryComponents: [ExportDialog, FlagDialog, CommentEditDialog, AdvancedSelectDialogComponent,  ContentQuickEditComponent],
    providers: [ContentService, DiscussionService, ImagesService, PlaylistService, GroupService]
})
export class ContentModule {}

