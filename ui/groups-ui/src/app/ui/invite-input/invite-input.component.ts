/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { GroupService } from 'src/app/svc/group.service';
import { MessageService } from 'src/app/svc/message.service';

@Component({
  selector: 'app-invite-input',
  templateUrl: './invite-input.component.html',
  styleUrls: ['./invite-input.component.scss']
})
export class InviteInputComponent implements OnInit {
  inviteCode: string;
  inviteType: string;
  errorMsg: string;
  constructor(
    public dialogRef: MatDialogRef<InviteInputComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupSvc: GroupService,
    private msgSvc: MessageService,
    private router: Router
  ) { }

  ngOnInit() {
  }

  sendCode() {
    if (this.inviteType === 'member') {
      this.groupSvc.acceptMemberInvite(this.inviteCode).subscribe(result => {
        if (result) {
          this.msgSvc.addSnackBar('Membership Added');
          this.msgSvc.notify('update-status');
          this.router.navigate(['/detail', result.groupId, result.groupSlug]);
        }
      }, error => {
        this.errorMsg = error.error.message;
      });
    } else {
      this.groupSvc.acceptGroupInvite(this.inviteCode, this.data.group.id).subscribe(result => {
        if (result) {
          this.msgSvc.addSnackBar('Group Added');
          this.msgSvc.notify('update-status');
          this.router.navigate(['/detail', result.groupId, result.groupSlug]);
        }
      }, error => {
        this.errorMsg = error.error.message;
      });
    }
  }

  onCancel() {
    this.dialogRef.close();
  }

}

