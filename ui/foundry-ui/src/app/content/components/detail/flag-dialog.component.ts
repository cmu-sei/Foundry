/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MessageService } from '../../../root/message.service';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'flag-dialog',
  templateUrl: 'flag-dialog.component.html',
  styleUrls: ['./detail.component.scss']
})
export class FlagDialog extends BaseComponent {
  comment: string;
  inputVisible: boolean;
  otherComment: string;
  updateContent = new EventEmitter();

  constructor(
    public dialogRef: MatDialogRef<FlagDialog>,
    private service: ContentService,
    private msgService: MessageService,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    super();
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  submitFlag(id, comment): void {
    if (comment) {
      if (comment === 'Other') {
        comment = this.otherComment;
      }
      this.$.push(this.service.addFlag(id, comment).subscribe(() => { }));
      this.dialogRef.close();
      this.msgService.addSnackBar('Content Flagged');
      this.updateContent.emit(comment);
    }
  }
}

