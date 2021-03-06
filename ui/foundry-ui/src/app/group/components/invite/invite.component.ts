/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { DOCUMENT } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { SettingsService } from 'src/app/root/settings.service';
import { GroupService } from '../../group.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'app-invite',
  templateUrl: './invite.component.html',
  styleUrls: ['./invite.component.scss']
})
export class InviteComponent extends BaseComponent implements OnInit {
  groupInviteCode: string;
  memberInviteCode: string;
  groupUrl: string;

  constructor(
    public dialogRef: MatDialogRef<InviteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupSvc: GroupService,
    private router: Router,
    private settingsSvc: SettingsService,
    @Inject(DOCUMENT) private document: Document
  ) {
    super();
  }

  ngOnInit() {
    this.groupUrl = this.document.location.href;
    this.$.push(this.groupSvc.getGroupInviteCodeById(this.data.group.id).subscribe(result => {
      if (result) {
        this.groupInviteCode = result;
      }
    }));
    this.$.push(this.groupSvc.getMemberInviteCodeById(this.data.group.id).subscribe(result => {
      if (result) {
        this.memberInviteCode = result;
      }
    }));
  }

  generateMemberCode() {
    this.$.push(this.groupSvc.updateMemberInviteCodeById(this.data.group.id).subscribe(result => {
      if (result) {
        this.memberInviteCode = result;
      }
    }));
  }

  generateGroupCode() {
    this.$.push(this.groupSvc.updateGroupInviteCodeById(this.data.group.id).subscribe(result => {
      if (result) {
        this.groupInviteCode = result;
      }
    }));
  }

  removeMemberCode() {
    this.$.push(this.groupSvc.deleteMemberInviteCodeById(this.data.group.id).subscribe(() => {
      this.memberInviteCode = '';
    }));
  }

  removeGroupCode() {
    this.$.push(this.groupSvc.deleteGroupInviteCodeById(this.data.group.id).subscribe(result => {
      this.groupInviteCode = '';
    }));
  }

  copyToClipboard(code) {
    document.addEventListener('copy', (e: ClipboardEvent) => {
      e.clipboardData.setData('text/plain', (code));
      e.preventDefault();
      document.removeEventListener('copy', null);
    });
    document.execCommand('copy');
  }

}

