/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { EditComponent } from '../edit/edit.component';
import { FileUploadComponent } from '../file-upload/file-upload.component';
import { BucketService } from '../../svc/bucket.service';
import { FileService } from '../../svc/file.service';
import { MessageService } from '../../svc/message.service';
import { AccountService } from '../../svc/account.service';
import { BucketSummary, AccountDetail } from '../../model';

@Component({
    selector: 'app-sidenav',
    templateUrl: './sidenav.component.html',
    styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
    activeBucketId: number = 0;
    buckets: BucketSummary[];
    panelOpen = true;
    account: AccountDetail;
    public isAdministrator: boolean = false;

    constructor(
        public dialog: MatDialog,
        private bucketService: BucketService,
        private fileService: FileService,
        private messageService: MessageService,
        private accountService: AccountService

    ) { }

    initAccount(account: AccountDetail) {
        if (account) {
            this.account = account;
            this.isAdministrator = account.isAdministrator;

            this.bucketService.bucketsUpdated$.subscribe(result => {
                if (result === true) {
                    this.loadBuckets();
                }
            });

            this.loadBuckets();
        }
    }

    ngOnInit() {

        this.accountService.account$.subscribe(account => { this.initAccount(account); });
        this.initAccount(this.accountService.account);

        this.messageService.listen().subscribe((m: any) => {
            if (m === 'bucket-update') {
                this.loadBuckets();
            }
        });
    }

    setBucket(id: number) {
        this.activeBucketId = id;
    }

    loadBuckets() {
        this.bucketService.list(null).subscribe((result) => {
            this.buckets = result.results;
        });
    }

    openUploadDialog() {
        const dialogRef = this.dialog.open(FileUploadComponent, {
            width: '800px', data: { bucketId: this.activeBucketId }
        });

        dialogRef.afterClosed().subscribe(result => {
            this.fileService.updateFiles();
            console.log('The dialog was closed');
        });
    }

    openBucketDialog() {
        const dialogRef = this.dialog.open(EditComponent, {
            width: '800px'
        });

        dialogRef.afterClosed().subscribe(result => {
            this.bucketService.updateBuckets();
            console.log('The dialog was closed');
        });
    }
}

