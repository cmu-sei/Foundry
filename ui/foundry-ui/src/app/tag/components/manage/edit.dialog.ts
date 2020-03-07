/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Tag, TagUpdate } from '../../../core-api-models';
import { BaseComponent } from '../../../shared/components/base.component';
import { TagService } from '../../tag.service';
import { TagManageComponent } from './manage.component';

@Component({
  selector: 'edit.dialog',
  templateUrl: './edit.dialog.html',
  styleUrls: ['./edit.dialog.scss']
})
// tslint:disable-next-line:component-class-suffix
export class EditTagDialog extends BaseComponent {
  name = '';
  description = '';
  tagType = '';
  tagSubType = '';
  private tag: Tag;
  public working = false;
  public parent: TagManageComponent;

  constructor(
    private tagService: TagService,
    private dialogRef: MatDialogRef<EditTagDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    super();
    this.tag = data.tag;
    this.parent = data.parent;
    this.name = this.tag.name;
    this.description = this.tag.description;
    this.tagType = this.tag.tagType;
    this.tagSubType = this.tag.tagSubType;
  }

  cancel(): void {
    this.dialogRef.close();
    this.data.cancelCallback();
  }

  continue() {
    const model: TagUpdate = {
      id: this.tag.id,
      name: this.name,
      description: this.description,
      tagType: this.tagType,
      tagSubType: this.tagSubType
    };

    this.parent.working = this.working = true;

    this.$.push(this.tagService.update(this.tag.id, model).subscribe(() => {
      this.dialogRef.close();
      this.data.continueCallback(this.data.parent);
    },
      () => {
      },
      () => {
        this.parent.working = this.working = false;
      }));
  }
}

