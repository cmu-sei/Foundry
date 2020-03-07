/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MatDialog, PageEvent } from '@angular/material';
import { DataFilter, MemberRequestStatus, MemberRequestUpdate } from 'src/app/models';
import { MemberRequestService } from 'src/app/svc/member-request.service';
import { MessageService } from 'src/app/svc/message.service';
import { SettingsService } from 'src/app/svc/settings.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-group-requests',
  templateUrl: './group-requests.component.html',
  styleUrls: ['./group-requests.component.scss']
})
export class GroupRequestsComponent implements OnChanges, OnInit {
  @Input()
  groupId: string;
  memberRequests: any [];
  spin: boolean;
  displayedColumns: string[] = ['name', 'date', 'status', 'actions'];
  errorMsg: string;
  portalLink: string;
  public memberDataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: 'recent',
    filter: ''
  };
  total: number;
  pageEvent: PageEvent;

  constructor(
    private memberRequestSvc: MemberRequestService,
    private msgSvc: MessageService,
    private settingsSvc: SettingsService,
    public dialog: MatDialog
  ) { }
  ngOnChanges(changes: SimpleChanges) {
    this.loadMemberRequests(changes.groupId.currentValue);
  }

  ngOnInit() {
    this.portalLink = this.settingsSvc.settings.urls.portalUrl;
    this.loadMemberRequests(this.groupId);
    this.msgSvc.listen().subscribe((m: any) => {
      if (m === 'update-requests') {
        this.loadMemberRequests(this.groupId);
      }
      if (m === 'update-members') {
        this.loadMemberRequests(this.groupId);
      }
    });
  }

  getPageEvent(e) {
    this.memberDataFilter.take = e.pageSize;
    if (e.previousPageIndex < e.pageIndex) {
      if ((this.memberDataFilter.skip + this.memberDataFilter.take) < this.total) {
        this.memberDataFilter.skip += this.memberDataFilter.take;
      }
    }
    if (e.previousPageIndex > e.pageIndex) {
      this.memberDataFilter.skip -= this.memberDataFilter.take;
    }
    this.loadMemberRequests(this.groupId);
  }

  loadMemberRequests(id) {
    this.spin = true;
    this.memberRequestSvc.listRequestsByGroup(id, this.memberDataFilter).subscribe(m => {
      this.memberRequests = m.results;
      this.total = m.total;
      this.spin = false;
    }, error => {
      this.errorMsg = error.error.message;
      this.spin = false;
    });
  }
  sort(value) {
    this.memberDataFilter.sort = value;
    this.reset();
  }

  reset() {
    this.memberDataFilter.skip = 0;
    this.memberDataFilter.take = 20;
    this.memberRequests = [];
    this.loadMemberRequests(this.groupId);
    }

  updateMemberRequest(member, sendStatus) {
    const model: MemberRequestUpdate = {
      accountId: member.accountId,
      groupId: this.groupId,
      status: sendStatus
    };

    this.memberRequestSvc.updateRequest(member.groupId, member.accountId, model).subscribe(result => {
      if (result) {
        if (sendStatus === MemberRequestStatus.Approved) {
          this.memberRequestSvc.removeMemberRequest(member.groupId, member.accountId).subscribe();
        }
      }
      this.msgSvc.addSnackBar('Member Request Updated');
      this.msgSvc.notify('update-requests');
    }, error => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  deleteMemberRequest(member) {
    this.memberRequestSvc.removeMemberRequest(member.groupId, member.accountId).subscribe(result => {
      this.msgSvc.addSnackBar('Member Request Removed. User can request access again.');
      this.msgSvc.notify('update-requests');
    }, error => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  openDelete(member): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete ' + member.accountName + ' ?',
        yesText: 'Delete',
        yesCallback: () => {
          this.deleteMemberRequest(member);
        },
        noText: 'Cancel',
        noCallback: () => { dialogRef.close(); },
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }


  openProfile(member) {
    const memberSlug = this.memberRequestSvc.slugifyName(member.accountName);
    window.location.href = this.portalLink + '/profile/' + member.accountId + '/' + memberSlug;
  }

}

