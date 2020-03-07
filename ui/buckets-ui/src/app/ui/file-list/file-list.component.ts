/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MatDialog } from '@angular/material';
import { Subscription, Observable } from 'rxjs';
import { BucketDetail, DataFilter, FileSummary, PagedResult } from '../../model';
import { AuthService } from '../../svc/auth.service';
import { FileService } from '../../svc/file.service';
import { ConfirmDialogComponent, ConfirmDialogSettings } from '../confirm-dialog/confirm-dialog.component';
import { FileUploadComponent } from '../file-upload/file-upload.component';
import { TagBrowserComponent } from '../tag-browser/tag-browser.component';
import { HttpResponse } from '@angular/common/http';
import { FileInfoDialogComponent } from '../file-info-dialog/file-info-dialog.component';

@Component({
    selector: 'file-list',
    templateUrl: './file-list.component.html',
    styleUrls: ['./file-list.component.scss']
})

export class FileListComponent implements OnInit, OnChanges {
    @Input() bucket: BucketDetail;
    public filter: string = '';
    user: any;
    profile$: Subscription;
    pagedResult: PagedResult<FileSummary>;
    public dataFilter: DataFilter = {
        skip: 0,
        take: 25,
        term: '',
        filter: '',
        sort: '-name'
    };
    working: boolean = false;
    constructor(
        public dialog: MatDialog,
        private fileService: FileService,
        private authService: AuthService
    ) { }

    ngOnChanges(changes: SimpleChanges) {
        this.search();
    }

    ngOnInit() {
        this.search();

        this.fileService.filesUpdated$.subscribe(result => {
            if (result === true) {
                this.search();
            }
        });
    }

    download(file: FileSummary) {

        this.fileService.download(file).subscribe((blob: HttpResponse<Blob>) => {
            window.open(blob.url, 'download');
        });
    };

    search() {
        if (!this.working) {
            this.working = true;
            if (this.bucket) {
                this.dataFilter.filter = `bucket=${this.bucket.id}` + '|' + this.filter;
            }
            else {
                this.dataFilter.filter = this.filter;
            }

            this.fileService.list(this.dataFilter).subscribe((result) => {
                this.pagedResult = result;
                this.working = false;
            });
        }
    }

    confirmDelete(file: FileSummary) {
        let settings: ConfirmDialogSettings = {
            title: 'Delete \'' + file.name + '\'',
            message: 'Are you sure you want to delete this file?',
            yesText: 'Yes',
            yesCallback: () => { this.delete(file); },
            noText: 'No',
            parent: this
        }

        const dialogRef = this.dialog.open(ConfirmDialogComponent, { data: settings });
    }

    openFileInfoDialog(file: FileSummary) {
        const dialogRef = this.dialog.open(FileInfoDialogComponent, { data: file });
    }

    reset() {
        this.dataFilter.skip = 0;
        this.search();
    }

    delete(file: FileSummary) {
        this.fileService.delete(file).subscribe((result) => {
            this.search();
        });
    }

    images: string[] = ['jpg', 'jpeg', 'png', 'gif']

    isImage(file: FileSummary): boolean {
        var isImage = false;
        this.images.forEach((value, key) => {
            if (file.name.endsWith(value)) isImage = true;
        });

        return isImage;
    }

    hasAccess(file: FileSummary, access: string) {
        return file.access.indexOf(access) >= 0;
    }

    toFileSize(length: number): string {
        return this.fileService.toFileSize(length);
    }

    openTagInput(file) {
        const dialogRef = this.dialog.open(TagBrowserComponent, {
            width: '800px',
            data: { file: file }
        });

        dialogRef.afterClosed().subscribe(result => {
            this.search();
            console.log('The dialog was closed');
        });
    }

    openUploadDialog() {
        const dialogRef = this.dialog.open(FileUploadComponent, {
            width: '800px'
        });

        dialogRef.afterClosed().subscribe(result => {
            console.log('The dialog was closed');
            this.search();
        });
    }

    sort(sort: string) {
        if (!this.working) {
            var current = this.dataFilter.sort;
            var root = current.replace('-', '');
            var desc = current.indexOf('-') == 0;

            if (root == sort) {
                this.dataFilter.sort = desc ? sort : '-' + sort;
            }
            else {
                this.dataFilter.sort = sort;
            }

            this.reset();
        }
    }

    getSortClass(sort: string): string {
        var current = this.dataFilter.sort;
        if ('-' + sort == current || sort == current)
            return '';

        return 'text-muted';

    }

    getSortIcon(sort: string): string {
        var current = this.dataFilter.sort;
        if ('-' + sort == current)
            return 'arrow_drop_down';

        return 'arrow_drop_up';
    }
}

