/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { NgModule, Optional, SkipSelf } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MomentModule } from 'angular2-moment';
import { ContentService } from '../content/content.service';
import { DocumentModule } from '../document/document.module';
import { GroupService } from '../group/group.service';
import { ImagesModule } from '../images/images.module';
import { ImagesService } from '../images/images.service';
import { PlaylistService } from '../playlist/playlist.service';
import { AdvancedSelectDialogComponent } from '../shared/components/advanced-select-dialog/advanced-select-dialog.component';
import { SharedModule } from '../shared/shared.module';
import { ProfileBrowserComponent } from './components/browser/browser.component';
import { ProfileDetailComponent } from './components/detail/detail.component';
import { ProfileManageComponent } from './components/manage/manage.component';
import { ProfileTileComponent } from './components/tile/tile.component';
import { ProfileRoutingModule } from './profile-routing.module';
import { EntityCache } from './profile.cache';
import { ProfileComponent } from './profile.component';
import { ProfileService } from './profile.service';

@NgModule({
  declarations: [
    ProfileComponent,
    ProfileBrowserComponent,
    ProfileDetailComponent,
    ProfileTileComponent,
    ProfileManageComponent
  ],
  imports: [
    SharedModule,
    DocumentModule,
    ImagesModule,
    ProfileRoutingModule,
    MomentModule,
    MatProgressSpinnerModule

  ],
  providers: [ProfileService, EntityCache, GroupService, PlaylistService, ContentService, ImagesService],
  entryComponents: [AdvancedSelectDialogComponent],
  exports: [
    ProfileTileComponent,
    ProfileManageComponent
  ]
})
export class ProfileModule {
  constructor(@Optional() @SkipSelf() parentModule: ProfileModule) {
    if (parentModule) {
      throw new Error(
        'ProfileModule is already loaded. Import it in the AppModule only');
    }
  }
}

