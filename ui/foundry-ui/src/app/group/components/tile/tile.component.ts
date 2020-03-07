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
import { MemberRequestCreate, MemberRequestStatus } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { GroupService } from '../../group.service';
import { AuthService } from '../../../auth/auth.service';

@Component({
  selector: 'group-tile',
  templateUrl: './tile.component.html',
  styleUrls: ['./tile.component.scss'],
})

export class GroupTileComponent extends BaseComponent implements OnInit {
  @Input() public group: any;
  @Input() public index: number;
  private _viewMode: string;
  public joinedGroup: any;
  public pendingFlag: boolean;
  public forumUrl: string;
  btnDisabled: boolean = false
  encodedLogoUrl: string;
  accountId: string;
  accountName: string;
  status: MemberRequestStatus;
  memberRequests: any[];

  constructor(
    private service: GroupService,
    private settingService: SettingsService,
    private msgService: MessageService,
    private authSvc: AuthService
  ) {
    super();
  }

  get viewMode(): string {
    return this._viewMode;
  }

  @Input()
  set viewMode(viewMode: string) {
    this._viewMode = viewMode;
  }

  ngOnInit() {
    this.accountId = this.authSvc.currentUser.profile.sub;
    this.accountName = this.authSvc.currentUser.profile.accountName;
    this.encodedLogoUrl = encodeURI(this.group.logoUrl);
    this.forumUrl = this.settingService.settings.clientSettings.urls.forumUrl;
  }

  role(): string {
    if (this.group.access.edit) {
      return 'Manager';
    }
    return '';
  }

  join() {
    this.btnDisabled = true;
    const model: MemberRequestCreate = {
      accountId: this.accountId,
      groupId: this.group.id,
      accountName: this.accountName
    };
    this.$.push(this.service.addRequest(this.group.id, model).subscribe(result => {
      if (result) {
        this.status = MemberRequestStatus.Pending;
        this.msgService.addSnackBar('Membership Request Submitted');
        this.btnDisabled = false;
      }
    }, error => {
      this.msgService.addSnackBar(error.message);
      this.btnDisabled = false;
    }));
  }

  leave() {
    this.btnDisabled = true;
    this.$.push(this.service.removeMember(this.group.id, this.accountId).subscribe(result => {
      if (result) {
        this.status = null;
        this.group.actions.leave = false;
        this.group.actions.join = true;
        this.btnDisabled = false;
        this.msgService.addSnackBar('Membership Removed');
      }
    }, error => {
      this.btnDisabled = false;
      this.msgService.addSnackBar(error.message);
    }));
  }
}

