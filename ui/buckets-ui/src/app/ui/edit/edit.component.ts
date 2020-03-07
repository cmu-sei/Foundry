/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { BucketService } from '../../svc/bucket.service';
import { MessageService } from '../../svc/message.service';
import { BucketCreate, BucketUpdate, BucketDetail, BucketSharingType } from '../../model';

@Component({
    selector: 'app-edit',
    templateUrl: './edit.component.html',
    styleUrls: ['./edit.component.scss']
})
export class EditComponent implements OnInit {
    @Input()
    bucket: BucketDetail;
    isPublic: boolean;
    isUpdate = false;
    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        private bucketService: BucketService,
        private messageService: MessageService,
        public dialogRef: MatDialogRef<EditComponent>,
    ) { }

    ngOnInit() {
        if (this.data) {
            this.bucket = this.data.bucket;
            this.isPublic = this.data.bucket.bucketSharingType == BucketSharingType.Public;
            this.isUpdate = true;
        } else {
            this.bucket = {};
        }
    }

    updateListeners() {
        this.messageService.notify('bucket-update');
    }

    onSubmit() {
        if (this.isUpdate) {
            const model: BucketUpdate = {
                id: this.bucket.id,
                name: this.bucket.name,
                bucketSharingType: this.isPublic ? BucketSharingType.Public : BucketSharingType.Private
            };

            this.bucketService.update(model)
                .subscribe((result) => {
                    this.bucket = result;
                    this.messageService.addSnackBar('Bucket Updated');
                    this.updateListeners();
                    this.dialogRef.close();
                }, error => { console.log(error); });
        } else {
            const model: BucketCreate = {
                name: this.bucket.name,
                globalId: this.bucket.globalId,
                bucketSharingType: this.isPublic ? BucketSharingType.Public : BucketSharingType.Private
            };
            this.bucketService.add(model)
                .subscribe((result) => {
                    this.bucket = result;
                    this.messageService.addSnackBar('Bucket Created');
                    this.updateListeners();
                    this.dialogRef.close();
                }, error => { console.log(error); });
        }

    }
}

