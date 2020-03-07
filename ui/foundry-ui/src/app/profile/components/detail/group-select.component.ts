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
import { GroupMemberDetail, GroupSummary } from '../../../core-api-models';
import { GroupService } from '../../../group/group.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'group-profile-select',
  // tslint:disable-next-line:max-line-length
  template: '<mat-checkbox (change)="updateGroup(group.id)" [checked]="groupExists" [disabled]="groupExists" [value]="group.id">{{ group.name }}</mat-checkbox>',
  styleUrls: ['./detail.component.scss'],
})
export class GroupProfileSelectComponent extends BaseComponent implements OnInit {
  @Input()
  public group: GroupSummary;
  @Input()
  public profileId: any;
  groupExists: boolean;
  playlists: any;


  constructor(
    private groupService: GroupService,
    private msgService: MessageService
  ) {
    super();
  }

  ngOnInit() {
    this.$.push(this.groupService.members(this.group.id, '').subscribe(result => {
      const members = result.results as GroupMemberDetail[];
      members.forEach(element => {
        if (element.accountId.toString() === this.profileId) {
          this.groupExists = true;
        }
      });
    }));
  }

  updateGroup(id) {
    // this.groupService.addMember(id, this.profileId).subscribe(
    //     result => {
    //         this.groupExists = true;
    //         this.msgService.addSnackBar('Member added to group');
    //     },
    //     error => {
    //         this.msgService.addSnackBar(error.error.message);
    //     }
    // );
  }
}

