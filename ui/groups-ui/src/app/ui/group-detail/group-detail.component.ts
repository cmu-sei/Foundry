/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { GroupDetail, MemberRequestCreate } from 'src/app/models';
import { AuthService } from 'src/app/svc/auth.service';
import { GroupRequestService } from 'src/app/svc/group-request.service';
import { GroupService } from 'src/app/svc/group.service';
import { MemberRequestService } from 'src/app/svc/member-request.service';
import { MemberService } from 'src/app/svc/member.service';
import { MessageService } from 'src/app/svc/message.service';
import { InviteComponent } from '../invite/invite.component';
import { TreeDialogComponent } from '../tree-dialog/tree-dialog.component';

@Component({
  selector: 'app-group-detail',
  templateUrl: './group-detail.component.html',
  styleUrls: ['./group-detail.component.scss']
})
export class GroupDetailComponent implements OnInit {
  group: GroupDetail;
  error: any;
  groupRequests: any[];
  accountId: string;

  constructor(
    private groupSvc: GroupService,
    private route: ActivatedRoute,
    private authSvc: AuthService,
    private memberSvc: MemberService,
    private memberRequestSvc: MemberRequestService,
    private groupRequestSvc: GroupRequestService,
    private msgSvc: MessageService,
    public dialog: MatDialog,
    public inviteDialog: MatDialog
  ) { }

  ngOnInit() {
    this.accountId = this.authSvc.currentUser.profile.sub;

    this.route.params.pipe(
      switchMap((params: Params) => this.groupSvc.load(params.id)))
      .subscribe(result => {
        this.group = result;
        this.loadGroupRequests(this.group.id);
      });

    this.msgSvc.listen().subscribe((m: any) => {
      if (m === 'update-status') {
        this.group.roles.member = true;
      }
      if (m === 'update-child-requests') {
        this.loadGroupRequests(this.group.id);
      }
    });
  }

  loadGroupRequests(id) {
    this.groupRequestSvc.getAllByParentID(id, '').subscribe(m => {
      this.groupRequests = m.results;
    });
  }

  sendRequest() {
    const model: MemberRequestCreate = {
      accountId: this.accountId,
      groupId: this.group.id
    };
    this.memberRequestSvc.addRequest(this.group.id, model).subscribe(result => {
      if (result) {
        this.group.actions.join = false;
        this.msgSvc.addSnackBar('Membership Request Submitted');
      }
    }, error => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  leave() {
    this.memberSvc.removeMember(this.group.id, this.accountId).subscribe(result => {
      if (result) {
        this.group.actions.leave = true;
        this.msgSvc.addSnackBar('Membership Removed');
      }
    }, error => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  checkForRequest() {
    if (!this.group.actions.join && !this.group.roles.member) {
      return true;
    } else {
      return false;
    }
  }

  openTreeDialog() {
    const dialogRef = this.dialog.open(TreeDialogComponent, {
      width: '350px',
      data: {
        group: this.group
       }
    });
  }

  openInviteDialog() {
    const dialogRef = this.inviteDialog.open(InviteComponent, {
      width: '1500px',
      data: {
        group: this.group
       }
    });
  }
}

