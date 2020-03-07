/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnInit } from '@angular/core';
import { ProfileInfo, ProfileSummary } from '../../../core-api-models';
import { ProfileService } from '../../profile.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'profile-tile',
  templateUrl: './tile.component.html',
  styleUrls: ['./tile.component.scss']
})
export class ProfileTileComponent extends BaseComponent implements OnInit {
  @Input() public profile: ProfileSummary;
  @Input() public index: number;
  private _viewMode: string;
  canManagePermissions: boolean = false;
  profileInfo: ProfileInfo;

  constructor(
    private profileService: ProfileService
  ) {
    super();
  }

  ngOnInit() {

    if (this.profileService.profile != null && this.profileService.profile.id !== this.profile.id) {
      this.canManagePermissions = this.profileService.profile.isAdministrator;
    }

    // fetch identity profile
    this.$.push(this.profileService.getProfileInfo(this.profile.globalId).subscribe((info: ProfileInfo) => {
      this.profileInfo = info;
    }
    ));

  }

  defaultImage() {
    return this.profileService.defaultImageUri;
  }

  get viewMode(): string {
    return this._viewMode;
  }

  @Input()
  set viewMode(viewMode: string) {
    this._viewMode = viewMode;
  }
}

