/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatChipInputEvent, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { AccountService } from '../../svc/account.service';
import { MessageService } from '../../svc/message.service';
import { AccountDetail, AccountUpdate, AccountCreate, BucketUpdate, AccountUpdateBucket, AccountDetailBucket, BucketSummary, BucketAccountCreate, BucketAccessType } from '../../model';
import { BucketService } from '../../svc/bucket.service';

@Component({
    selector: 'account-edit',
    templateUrl: './account-edit.component.html',
    styleUrls: ['./account-edit.component.scss']
})

export class AccountEditComponent implements OnInit {

    public globalId: string;
    public name: string;
    public isAdministrator: boolean;
    public isApplication: boolean;
    public isUploadOwner: boolean;
    public buckets: Array<AccountDetailBucket>;
    public availableBuckets: Array<BucketSummary>;
    public selectedBucket: BucketSummary = null;

    public isNew: boolean;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        private accountService: AccountService,
        private messageService: MessageService,
        private bucketService: BucketService,
        public dialogRef: MatDialogRef<AccountEditComponent>,
    ) { }

    ngOnInit() {
        if (this.data.account) {
            this.isNew = false;
            this.load();
        }
        else {
            this.isNew = true;
        }
    }

    loadAvailableBuckets() {
        this.bucketService.list({ filter: 'available' }).subscribe((result) => {
            this.availableBuckets = result.results;
        });
    }

    addBucket() {
        let model: BucketAccountCreate = {
            bucketId: this.selectedBucket.id,
            accountId: this.data.account.globalId,
            bucketAccessType: BucketAccessType.Member,
            isDefault: false
        };

        this.bucketService.addAccount(this.selectedBucket.id, model).subscribe((result) => {
            this.loadAvailableBuckets();
            this.load();
            this.selectedBucket = null;
        });
    }

    load() {
        this.loadAvailableBuckets();

        this.accountService.load(this.data.account.globalId).subscribe((result) => {
            this.globalId = result.globalId;
            this.name = result.name;
            this.isAdministrator = result.isAdministrator;
            this.isApplication = result.isApplication;
            this.isUploadOwner = result.isUploadOwner;
            this.buckets = result.buckets;
        });
    }

    submit() {
        if (this.isNew) {
            let model: AccountCreate = {
                globalId: this.globalId,
                name: this.name,
                isAdministrator: this.isAdministrator,
                isApplication: this.isApplication,
                isUploadOwner: this.isUploadOwner
            };

            this.accountService.add(model).subscribe(result => {
                this.dialogRef.close();
                this.messageService.addSnackBar('Account Created');
            }, error => { console.log(error); });
        }
        else {
            let model: AccountUpdate = {
                globalId: this.globalId,
                name: this.name,
                isAdministrator: this.isAdministrator,
                isApplication: this.isApplication,
                isUploadOwner: this.isUploadOwner
            };

            model.buckets = [];

            this.buckets.forEach(bucket => {
                model.buckets.push({ id: bucket.id, name: bucket.name, bucketAccessType: bucket.bucketAccessType, isDefault: bucket.isDefault });
            });

            this.accountService.update(model).subscribe(result => {
                this.dialogRef.close();
                this.messageService.addSnackBar('Account Updated');
            }, error => { console.log(error); });
        }
    }

    onDefaultChange(value, bucket: AccountDetailBucket) {
        bucket.isDefault = value;
        if (value) {
            this.buckets.forEach(b => {
                if (bucket != b) {
                    b.isDefault = false;
                }
            });
        }
    }
}

