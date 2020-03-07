/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { forkJoin } from 'rxjs';
import { BucketSummary } from '../../model';
import { AuthService } from '../../svc/auth.service';
import { BucketService } from '../../svc/bucket.service';
import { FileService } from '../../svc/file.service';
import { MessageService } from '../../svc/message.service';

@Component({
    selector: 'app-file-upload',
    templateUrl: './file-upload.component.html',
    styleUrls: ['./file-upload.component.scss']
})
export class FileUploadComponent implements OnInit {

    @ViewChild('file') file;
    public files: Set<File> = new Set();
    buckets: BucketSummary[];
    bucketId: number;
    progress;
    showCancelButton = true;
    showActionButtons = false;
    uploading = false;
    uploadSuccessful = false;
    public errors: Array<string> = [];

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        public dialogRef: MatDialogRef<FileUploadComponent>,
        public fileService: FileService,
        private msgService: MessageService,
        private bucketSvc: BucketService
    ) {
    }

    ngOnInit() {
        this.loadBuckets();
        this.bucketId = this.data.bucketId;
    }

    onFilesAdded() {
        const files: { [key: string]: File } = this.file.nativeElement.files;
        for (let key in files) {
            if (!isNaN(parseInt(key))) {
                this.files.add(files[key]);
            }
        }
        this.showActionButtons = true;
    }

    addFiles() {
        this.file.nativeElement.click();
    }

    onError(this$: FileUploadComponent, file: File) {
        this$.errors.push("'" + file.name + "' failed to upload.");
        this$.dialogRef.disableClose = false;
        this$.uploading = false;
    }

    upload() {
        this.errors = [];
        this.dialogRef.disableClose = true;

        this.uploading = true;

        if (this.bucketId) {
            this.progress = this.bucketSvc.uploadToBucket(this.files, this.bucketId, this, this.onError);
        } else {
            this.progress = this.fileService.uploadToDefault(this.files, this, this.onError);
        }

        for (const key in this.progress) {
            this.progress[key].progress.subscribe(val => console.log(val));
        }

        let allProgressObservables = [];
        for (let key in this.progress) {
            allProgressObservables.push(this.progress[key].progress);
        }

        forkJoin(allProgressObservables).subscribe(end => {
            this.dialogRef.disableClose = false;
            this.uploadSuccessful = true;
            this.uploading = false;

            this.msgService.addSnackBar('Files Uploaded');
        });
    }

    loadBuckets() {
        this.bucketSvc.list(null).subscribe((result) => {
            this.buckets = result.results;

            if (!this.bucketId) {
                this.bucketId = this.buckets.find(e => e.isDefault === true).id;
            }

        });
    }
}

