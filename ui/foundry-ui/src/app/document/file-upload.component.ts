/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { DocumentService } from './document.service';
import { BaseComponent } from '../shared/components/base.component';

export interface DocImage {
  filename: string;
}

@Component({
  selector: 'file-uploader',
  templateUrl: 'file-upload.component.html',
  styles: [`
        .upload-ui {
            display: inline-block;
        }
    `]
})
export class FileUploadComponent extends BaseComponent {

  @Input() id: string;
  @Input() prompt = 'Browse';
  // tslint:disable-next-line:no-output-on-prefix
  @Output() onUploaded: EventEmitter<string> = new EventEmitter<string>();

  images: DocImage[] = [];
  queuedFiles: any[] = [];

  submitSpin = false;

  constructor(private service: DocumentService) {
    super();
  }

  fileSelectorChanged(e) {
    this.queuedFiles = (e.target || e.srcElement).files;
  }

  filesQueued() {
    return this.queuedFiles.length > 0;
  }

  upload() {
    for (let i = 0; i < this.queuedFiles.length; i++) {
      this.uploadFile(this.queuedFiles[i]);
    }
    this.queuedFiles = [];
  }

  uploadFile(file) {
    this.submitSpin = true;
    this.$.push(this.service.upload(file).subscribe((result: any) => {
      this.onUploaded.emit(result[0]);
      this.submitSpin = false;
    }));
  }
}

