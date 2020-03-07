/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Inject, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Croppie } from 'croppie/croppie';
import { FileDetail, DataFilter } from '../../../core-api-models';
import { ImagesService } from '../../images.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { SettingsService } from '../../../root/settings.service';
import { DocumentService } from '../../../document/document.service';
import { MessageService } from '../../../root/message.service';

@Component({
  selector: 'app-image-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class ImageBrowserComponent extends BaseComponent implements OnInit {
  searchType = 'name';
  images: FileDetail[];
  imagePath: string;
  uploadElement: any;
  croppie: any;
  imageChanged = false;
  fileName: string;
  queuedFiles: any[] = [];
  croppedImage: any;
  hideBtn: boolean;
  logoUrl: string;
  brokenImage: boolean;
  more: boolean;
  total: number;
  // tslint:disable-next-line:no-output-on-prefix
  @Output() onUploaded: EventEmitter<string> = new EventEmitter<string>();
  // tslint:disable-next-line:no-output-on-prefix
  @Output() onSelected: EventEmitter<any> = new EventEmitter<any>();
  public dataFilter: DataFilter = {
    skip: 0,
    take: 20,
    term: '',
    sort: '-recent',
    filter: ''
  };
  constructor(
    private imgSvc: ImagesService,
    private settingsSvc: SettingsService,
    public dialogRef: MatDialogRef<ImageBrowserComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private service: DocumentService,
    public messageSvc: MessageService
  ) {
    super();
  }

  ngOnInit() {
    this.reset();
    this.imagePath = this.settingsSvc.settings.clientSettings.urls.uploadUrl;
  }

  searchByType(term, searchType) {
    this.images = [];
    this.dataFilter.term = '';
    this.dataFilter.filter = ''

    if (searchType === 'name') {
      this.dataFilter.term = term;
    } else if (searchType === 'tag') {
      this.dataFilter.filter = 'tag=' + term;
    }

    this.search();
  }

  search() {

    if (this.dataFilter.filter != '')
      this.dataFilter.filter += '|';

    var extensions = this.data.extensions.join(',');
    this.dataFilter.filter += 'extension=' + extensions;

    this.$.push(this.imgSvc.list(this.dataFilter).subscribe(result => {
      for (let i = 0; i < result.results.length; i++) {
        this.images.push(result.results[i]);
      }

      this.total = result.total;
      this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
    }));
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 20;
    this.dataFilter.term = '';
    this.dataFilter.filter = '';
    this.images = [];
    this.search();
  }

  selectFile(path) {
    this.imgSvc.updateFileSource(path);
    this.dialogRef.close();
  }

  selectFileUrl(path) {
    this.imgSvc.updateFileUrlSource(path);
    this.dialogRef.close();
  }

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
    return this.croppie.result({ type: 'blob', size: 'viewport' })
      .then((result) => {
        if (result) {
          this.uploadFile(result);
          this.croppie.destroy();
          this.messageSvc.addSnackBar('Image uploaded. Click the submit button to save the changes.');
        }
      });
  }
  uploadFile(file: File) {
    this.$.push(this.service.uploadCrop(this.fileName, file).subscribe((result: any) => {
      this.imgSvc.updateFileSource(result[0].file);
      this.hideBtn = true;
      this.dialogRef.close();
    }));
  }

  removeFile() {
    this.queuedFiles = [];
    this.croppie.destroy();
  }

  imgError() {
    this.brokenImage = true;
  }

  resetError() {
    this.brokenImage = false;
  }

  showUrlButton() {
    return !(this.brokenImage || !this.logoUrl);
  }
}

