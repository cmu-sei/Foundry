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
import { DataFilter, MemberCreate, MemberSummary } from 'src/app/models';
import { MemberService } from 'src/app/svc/member.service';
import { MessageService } from 'src/app/svc/message.service';
import { SettingsService } from 'src/app/svc/settings.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-group-members',
  templateUrl: './group-members.component.html',
  styleUrls: ['./group-members.component.scss']
})
export class GroupMembersComponent implements OnChanges, OnInit {
  @Input()
  groupId: string;
  members: MemberSummary [];
  @Input()
  canManage: boolean;
  displayedColumns: string[] = ['name', 'date', 'access', 'actions'];
  showDelete = false;
  spin: boolean;
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
    private memberSvc: MemberService,
    private msgSvc: MessageService,
    private settingsSvc: SettingsService,
    public dialog: MatDialog
  ) { }

  ngOnChanges(changes: SimpleChanges) {
    this.loadMembers(changes.groupId.currentValue);
  }

  ngOnInit() {
    this.portalLink = this.settingsSvc.settings.urls.portalUrl;
    this.loadMembers(this.groupId);
    this.msgSvc.listen().subscribe((m: any) => {
      if (m === 'update-requests') {
        this.loadMembers(this.groupId);
      }
      if (m === 'update-members') {
        this.loadMembers(this.groupId);
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
    this.loadMembers(this.groupId);
  }

  loadMembers(id) {
    this.spin = true;
    this.memberSvc.members(id, this.memberDataFilter).subscribe(m => {
      this.members = m.results;
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
    this.members = [];
    this.loadMembers(this.groupId);
  }

  promoteToManager(member) {
    const model: MemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: true
    };
    this.memberSvc.updateMember(member.groupId, member.accountId, model).subscribe(result => {
      member.isManager = true;
      member.isOwner = false;
      this.msgSvc.addSnackBar('Member Promoted');
    }, (error) => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  demoteFromManager(member) {
    const model: MemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: false
    };
    this.memberSvc.updateMember(member.groupId, member.accountId, model).subscribe(result => {
      member.isManager = false;
      member.isOwner = false;
      this.msgSvc.addSnackBar('Manager Access Removed');
    }, (error) => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  promoteToOwner(member) {
    const model: MemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: true,
      isManager: false
    };
    this.memberSvc.updateMember(member.groupId, member.accountId, model).subscribe(result => {
      member.isOwner = true;
      member.isManager = false;
      this.msgSvc.addSnackBar('Member Promoted to Owner');
    }, (error) => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  demoteFromOwner(member) {
    const model: MemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: true
    };
    this.memberSvc.updateMember(member.groupId, member.accountId, model).subscribe(result => {
      member.isManager = true;
      member.isOwner = false;
      this.msgSvc.addSnackBar('Owner Access Removed');
    }, (error) => {
      this.msgSvc.addSnackBar(error.error.message);
    });
  }

  removeMember(member) {
    this.memberSvc.removeMember(member.groupId, member.accountId).subscribe(result => {
      if (result) {
        this.msgSvc.notify('update-members');
        this.msgSvc.addSnackBar('Member Removed');
      }
      }, error => {
        this.msgSvc.addSnackBar(error.error.message);
    });
  }

  openDelete(member): void {
    console.log(member);
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete ' + member.accountName + ' ?',
        yesText: 'Delete',
        yesCallback: () => {
          this.removeMember(member);
        },
        noText: 'Cancel',
        noCallback: () => { dialogRef.close(); },
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }

  openProfile(member) {
    const memberSlug = this.memberSvc.slugifyName(member.accountName);
    window.location.href = this.portalLink + '/profile/' + member.accountId + '/' + memberSlug;
  }
}

