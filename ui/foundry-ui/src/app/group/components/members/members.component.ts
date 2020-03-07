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
import { MatDialog, PageEvent } from '@angular/material';
import { DataFilter, GroupMemberCreate, GroupMemberSummary } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { SlugifyPipe } from '../../../shared/app.pipes';
import { BaseComponent } from '../../../shared/components/base.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { GroupService } from '../../group.service';

@Component({
  selector: 'group-members',
  templateUrl: 'members.component.html',
  styleUrls: ['./members.component.scss'],
  providers: [SlugifyPipe]
})
export class GroupMembersComponent extends BaseComponent implements OnInit {
  @Input()
  groupId: string;
  @Input()
  canManage: boolean;
  spin: boolean;
  members: GroupMemberSummary[];
  displayedColumns: string[] = ['name', 'date', 'access', 'actions'];
  showDelete = false;
  errorMsg: string;
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
    private service: GroupService,
    private messageService: MessageService,
    private slugifyPipe: SlugifyPipe,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit() {
    this.loadMembers(this.groupId);
    this.$.push(this.messageService.listen().subscribe((m: any) => {
      if (m === 'update-requests') {
        this.loadMembers(this.groupId);
      }
      if (m === 'update-members') {
        this.loadMembers(this.groupId);
      }
    }));
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
    this.$.push(this.service.members(id, this.memberDataFilter).subscribe(m => {
      this.members = m.results;
      this.total = m.total;
      this.spin = false;
    }, error => {
      this.errorMsg = error.error.message;
      this.spin = false;
    }));
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
    const model: GroupMemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: true
    };
    this.$.push(this.service.updateMember(member.groupId, member.accountId, model).subscribe(() => {
      member.isManager = true;
      member.isOwner = false;
      this.messageService.addSnackBar('Member Promoted');
    }, (error) => this.messageService.addSnackBar(error.error.message)));
  }

  demoteFromManager(member) {
    const model: GroupMemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: false
    };
    this.$.push(this.service.updateMember(member.groupId, member.accountId, model).subscribe(() => {
      member.isManager = false;
      member.isOwner = false;
      this.messageService.addSnackBar('Manager Access Removed');
    }, (error) => this.messageService.addSnackBar(error.error.message)));
  }

  promoteToOwner(member) {
    const model: GroupMemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: true,
      isManager: false
    };
    this.$.push(this.service.updateMember(member.groupId, member.accountId, model).subscribe(() => {
      member.isOwner = true;
      member.isManager = false;
      this.messageService.addSnackBar('Member Promoted');
    }, (error) => this.messageService.addSnackBar(error.error.message)));
  }

  demoteFromOwner(member) {
    const model: GroupMemberCreate = {
      accountId: member.accountId,
      groupId: member.groupId,
      isOwner: false,
      isManager: true
    };
    this.$.push(this.service.updateMember(member.groupId, member.accountId, model).subscribe(() => {
      member.isManager = true;
      member.isOwner = false;
      this.messageService.addSnackBar('Owner Access Removed');
    }, (error) => this.messageService.addSnackBar(error.error.message)));
  }

  removeMember(member) {
    this.$.push(this.service.removeMember(member.groupId, member.accountId).subscribe(result => {
      if (result) {
        this.messageService.notify('update-members');
        this.messageService.addSnackBar('Member Removed');
      }
    }, error => {
      this.messageService.addSnackBar(error.error.message);
    }));
  }

  openDelete(member): void {
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

  slugify(name: string) {
    return this.slugifyPipe.transform(name);
  }
}

