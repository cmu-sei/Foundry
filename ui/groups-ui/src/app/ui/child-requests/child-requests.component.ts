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
import { GroupRequestUpdate } from 'src/app/models';
import { GroupRequestService } from 'src/app/svc/group-request.service';
import { MessageService } from 'src/app/svc/message.service';

@Component({
  selector: 'app-child-requests',
  templateUrl: './child-requests.component.html',
  styleUrls: ['./child-requests.component.scss']
})
export class ChildRequestsComponent implements OnInit {
  @Input()
  groupId: string;
  @Input()
  groupRequests: any [];
  displayedColumns: string[] = ['name', 'date', 'status', 'actions'];
  constructor(
    private groupRequestSvc: GroupRequestService,
    private msgSvc: MessageService
  ) { }

  ngOnInit() {
  }

  updateGroupRequest(group, sendStatus) {
    const model: GroupRequestUpdate = {
      parentGroupId: group.parentGroupId,
      childGroupId: group.childGroupId,
      status: sendStatus
    };

    this.groupRequestSvc.updateRequest(group.parentGroupId, group.childGroupId, model).subscribe(result => {
      if (result) {
          this.groupRequestSvc.removeGroupRequest(group.parentGroupId, group.childGroupId).subscribe();
      }
      this.msgSvc.addSnackBar('Group Request Updated');
      this.msgSvc.notify('update-child-requests');
    }, error => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }
}

