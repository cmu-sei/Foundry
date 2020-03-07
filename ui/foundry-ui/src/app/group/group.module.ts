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
import { ContentService } from '../content/content.service';
import { DocumentModule } from '../document/document.module';
import { ImagesModule } from '../images/images.module';
import { ImagesService } from '../images/images.service';
import { PlaylistService } from '../playlist/playlist.service';
import { ConfirmDeleteComponent } from '../shared/components/confirm-delete.component';
import { SharedModule } from '../shared/shared.module';
import { GroupBrowserComponent } from './components/browser/browser.component';
import { GroupDetailComponent } from './components/detail/detail.component';
import { GroupEditComponent } from './components/edit/edit.component';
import { InviteInputComponent } from './components/invite-input/invite-input.component';
import { InviteComponent } from './components/invite/invite.component';
import { MemberRequestsComponent } from './components/member-requests/member-requests.component';
import { GroupMembersComponent } from './components/members/members.component';
import { GroupRoutingModule } from './group-routing.module';
import { GroupComponent } from './group.component';
import { GroupService } from './group.service';

@NgModule({
  declarations: [
    GroupComponent,
    GroupBrowserComponent,
    GroupDetailComponent,
    GroupEditComponent,
    GroupMembersComponent,
    MemberRequestsComponent,
    InviteComponent,
    InviteInputComponent
  ],
  imports: [
    SharedModule,
    DocumentModule,
    GroupRoutingModule,
    ImagesModule
  ],
  exports: [],
  providers: [GroupService, PlaylistService, ContentService, ImagesService],
  entryComponents: [ConfirmDeleteComponent, InviteComponent, InviteInputComponent]
})
export class GroupModule { }

