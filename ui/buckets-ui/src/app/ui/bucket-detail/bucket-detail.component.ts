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
import { ActivatedRoute, Params, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { BucketService } from 'src/app/svc/bucket.service';
import { EditComponent } from '../edit/edit.component';
import { BucketDetail, DataFilter, FileSummary } from '../../model';
import { MessageService } from '../../svc/message.service';

@Component({
    selector: 'app-bucket-detail',
    templateUrl: './bucket-detail.component.html',
    styleUrls: ['./bucket-detail.component.scss']
})
export class BucketDetailComponent implements OnInit {
    public bucket: BucketDetail;

    constructor(
        private route: ActivatedRoute,
        private bucketService: BucketService,
        private messageService: MessageService,
        public dialog: MatDialog,
    ) { }

    ngOnInit() {
        this.route.params.pipe(
            switchMap((params: Params) => this.bucketService.load(params['id'])))
            .subscribe(result => { this.bucket = result; }, error => { console.log(error); });
    }

    load() {
        this.bucketService.load(this.bucket.id).subscribe(result => {
            this.bucket = result;
        }, error => { console.log(error); });
    }

    updateListeners() {
        this.messageService.notify('bucket-update');
    }

    hasAccess(bucket: BucketDetail, access: string) {
        return bucket.access.indexOf(access) >= 0;
    }

    setDefault() {
        this.bucketService.setDefault(this.bucket.id).subscribe(result => {
            this.bucket = result;
            this.updateListeners();
        });
    }

    openEditDialog(bucket) {
        const dialogRef = this.dialog.open(EditComponent, {
            width: '800px',
            data: { bucket: bucket }
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
            this.load();
        });
    }
}

