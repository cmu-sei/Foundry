/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { HttpEventType, HttpResponse } from '@angular/common/http';
import { FileService } from '../../api/file.service';

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
export class FileUploadComponent implements OnInit {

    @Input() orderId: number;
    @Input() id : number = 0;
    @Input() prompt : string = "Browse";
    @Output() onUploaded : EventEmitter<string> = new EventEmitter<string>();

    images : DocImage[] = [];
    queuedFiles : any[] = [];

    constructor(
        private service : FileService
    ) { }

    ngOnInit() {
    }

    fileSelectorChanged(e) {
        this.queuedFiles = [];
        for (let i = 0; i < e.srcElement.files.length; i++) {
            let file = e.srcElement.files[i];
            this.queuedFiles.push({
                key: this.id + "-" + file.name,
                name: file.name,
                file: file,
                progress: -1
            });
        }
    }

    filesQueued() {
        return this.queuedFiles.length > 0;
    }

    dequeueFile(qf) {
        this.queuedFiles.splice(this.queuedFiles.indexOf(qf),1);
    }

    upload() {
        for (let i = 0; i < this.queuedFiles.length; i++) {
            this.uploadFile(this.queuedFiles[i]);
        }
    }

    uploadFile(qf) {
        this.service.upload(this.orderId, qf.file)
        .finally(() => this.queuedFiles.splice(this.queuedFiles.indexOf(qf), 1))
        .subscribe(
            (event) => {
                if (event.type === HttpEventType.UploadProgress) {
                    qf.progress = Math.round(100 * event.loaded / event.total);
                } else if (event instanceof HttpResponse) {
                    this.onUploaded.emit(event.body[0]);
                }
            },
            (err) => {

            }
        );
    }
}

