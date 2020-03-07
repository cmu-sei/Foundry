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
import { ContentService } from '../../content.service';
import { ContentManageComponent } from './manage.component';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'export.dialog',
  templateUrl: './export.dialog.html',
})
export class ExportDialog extends BaseComponent {
  fileName: string = 'export-' + new Date().getTime();
  isPackage: boolean = false;
  private ids: number[] = [];
  public working: boolean = false;
  public parent: ContentManageComponent;

  constructor(
    private contentService: ContentService,
    private dialogRef: MatDialogRef<ExportDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    super();
    this.ids = data.ids;
    this.parent = data.parent;
  }

  cancel(): void {
    this.dialogRef.close();
    this.data.cancelCallback();
  }

  continue() {
    this.parent.working = this.parent.exporting = this.working = true;
    var type = this.isPackage ? 'zip' : 'csv';

    var model = {
      ids: this.ids,
      type: type,
      fileName: this.fileName
    };

    this.$.push(this.contentService.export(model).subscribe(response => {
      var name = this.fileName + '.' + model.type;
      var type = model.type == 'csv' ? 'text/csv' : 'application/zip';
      var blob = new Blob([response.blob()], { type: type });

      if (response.text() != null && navigator.msSaveBlob)
        return navigator.msSaveBlob(blob, name);

      var url = window.URL.createObjectURL(blob);
      var a = document.createElement("a");
      a.style.display = 'none';
      a.href = url;
      a.download = name;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove();

      this.dialogRef.close();
      this.data.continueCallback(this.data.parent);
    },
      error => { },
      () => {
        this.parent.working = this.parent.exporting = this.working = false;
      }));
  }
}

