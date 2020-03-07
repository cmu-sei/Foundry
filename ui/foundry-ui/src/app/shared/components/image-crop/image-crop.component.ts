/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Croppie } from 'croppie/croppie';
import { DocumentService } from '../../../document/document.service';
import { MessageService } from '../../../root/message.service';

@Component({
  selector: 'image-crop',
  templateUrl: './image-crop.component.html',
  styleUrls: ['./image-crop.component.scss']
})
export class ImageCropComponent {
  uploadElement: any;
  croppie: any;
  imageChanged = false;
  @Input() id: string;
  @Input() prompt = 'Browse';
  // tslint:disable-next-line:no-output-on-prefix
  @Output() onUploaded: EventEmitter<string> = new EventEmitter<string>();
  fileName: string;
  queuedFiles: any[] = [];
  croppedImage: any;
  hideBtn: boolean;

  constructor(
    private service: DocumentService,
    public messageSvc: MessageService
  ) { }

  initCroppie() {
    this.hideBtn = false;
    this.initFileUpload();
    const cropElement = document.getElementById('imageUploadItem');
    this.croppie = new Croppie(cropElement, {
      viewport: { width: 570, height: 300 },
      boundary: { width: 670, height: 400 },
      customClass: 'image-crop'
    });

    this.croppie.bind({
      url: 'assets/images/croppie-placeholder.png',
    });
  }

  initFileUpload() {
    this.uploadElement = document.getElementById('fileUploadInput');
    this.uploadElement.addEventListener('change', this.onFileUpload.bind(this));
  }

  onFileUpload() {
    if (this.uploadElement.files && this.uploadElement.files.length > 0) {
      this.imageChanged = true;
      const file = this.uploadElement.files[0];
      const reader = new FileReader();
      reader.readAsDataURL(file);
      const fileNameWithExtenstion = file.name;
      // remove the extension and save filename with .png ext
      const name = fileNameWithExtenstion.split('.');
      this.fileName = name[0] + '.png';
      reader.onload = ((event: any) => {
        this.croppie.bind({
          url: event.target.result
        });
      });
    }
  }

  fileSelectorChanged(e) {
    this.queuedFiles = (e.target || e.srcElement).files;
}

  filesQueued() {
    return this.queuedFiles.length > 0;
  }

  upload(): Promise<any> {
    return this.croppie.result({type: 'blob', size: 'viewport'})
      .then((result) => {
        if (result) {
          this.uploadFile(result);
          this.croppie.destroy();
          this.messageSvc.addSnackBar('Image uploaded. Click the submit button to save the changes.');
        }
      });
  }

  uploadFile(file: File) {
    this.service.uploadCrop(this.fileName, file).subscribe((result: any) => {
        this.onUploaded.emit(result[0]);
        this.hideBtn = true;
    });
  }
}

