/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, Input } from '@angular/core';
import { MatDialog } from '@angular/material';
import { AuthService } from '../../svc/auth.service';
import { FileService } from '../../svc/file.service';
import { ImportFileSummary, BucketSummary, ImportFileUpdate, PagedResult, DataFilter } from '../../model';
import { BucketService } from '../../svc/bucket.service';
import { MessageService } from '../../svc/message.service';

@Component({
    selector: 'file-import',
    templateUrl: './import.component.html',
    styleUrls: ['./import.component.scss']
})

export class ImportComponent implements OnInit {
    public pagedResult: PagedResult<ImportFileSummary>;
    public selectedBucketId: number = 0;
    public buckets: Array<BucketSummary> = [];
    public files: Array<ImportFileSummary> = [];
    public working: boolean = false;
    public all: boolean = false;
    public selected: Array<ImportFileSummary> = [];
    public dataFilter: DataFilter = {
        skip: 0,
        take: 25,
        term: '',
        filter: '',
        sort: '-name'
    };

    constructor(
        public dialog: MatDialog,
        private fileService: FileService,
        private authService: AuthService,
        private bucketService: BucketService,
        private messageService: MessageService
    ) { }

    ngOnInit() {
        this.loadBuckets();
    }

    search() {
        if (!this.working) {
            this.working = true;
            this.fileService.listForImport(this.dataFilter).subscribe((result) => {
                this.pagedResult = result;
                this.files = result.results;
                this.working = false;
            });
        }
    }

    reset() {
        this.dataFilter.skip = 0;
        this.search();
    }

    loadBuckets() {
        this.bucketService.list(null).subscribe((result) => {
            this.buckets = result.results;

            this.search();
        });
    }

    import(file: ImportFileSummary) {
        if (!this.working) {
            this.working = true;
            const model: ImportFileUpdate = { bucketId: file.bucketId, path: file.path, name: file.name, globalId: file.globalId };
            this.fileService.import(model).subscribe((result) => {
                file.isImported = result[0].isSuccess;
                this.messageService.addSnackBar('File ' + file.name + ' imported');
                this.working = false;
            });
        }
    }

    importAll() {
        if (this.selected.length > 0 && !this.working) {
            this.working = true;
            let processed = 0;
            for (let i = 0; i < this.selected.length; i++) {
                var file = this.selected[i];
                if (!file.isImported && file.bucketId > 0) {
                    const model: ImportFileUpdate = { bucketId: file.bucketId, path: file.path, name: file.name, globalId: file.globalId };
                    this.fileService.import(model).subscribe((results) => {
                        var result = results[0];
                        var f = this.files.find(x => x.name === result.fileName);
                        if (f) {
                            f.isImported = result.isSuccess;
                        }
                        processed++;
                        this.check(processed);
                    });
                }
                else {
                    processed++;
                    this.check(processed);
                }
            }
        }
    }

    check(processed: number) {
        if (processed == this.selected.length) {
            this.messageService.addSnackBar(processed + ' files were processed. Reloading...');
            this.working = false;
            this.selected = [];
            this.all = false;
            this.search();
        }
    }

    onChange(file, event) {
        this.all = false;
        if (event.target.checked) {
            this.selected.push(file);
        } else {
            for (let i = 0; i < this.selected.length; i++) {
                if (this.selected[i].path === file.path) {
                    this.selected.splice(i, 1);
                }
            }
        }
    }

    onAllChange(value, event) {
        this.selected = [];
        if (event.target.checked) {
            this.all = true;
            for (let i = 0; i < this.files.length; i++) {
                this.selected.push(this.files[i]);
            }
        }
    }
}

