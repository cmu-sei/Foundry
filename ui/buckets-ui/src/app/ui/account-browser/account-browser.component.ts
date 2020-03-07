/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material';
import { AuthService } from '../../svc/auth.service';
import { FileUploadComponent } from '../file-upload/file-upload.component';
import { SidenavComponent } from '../sidenav/sidenav.component';
import { TagBrowserComponent } from '../tag-browser/tag-browser.component';
import { AccountSummary, PagedResult, DataFilter } from '../../model';
import { FileService } from '../../svc/file.service';
import { AccountService } from '../../svc/account.service';
import { MessageService } from '../../svc/message.service';
import { AccountEditComponent } from '../account-edit/account-edit.component';

@Component({
    selector: 'account-browser',
    templateUrl: './account-browser.component.html',
    styleUrls: ['./account-browser.component.scss']
})

export class AccountBrowserComponent implements OnInit, OnChanges {
    public filter: string = '';
    user: any;
    profile$: Subscription;
    pagedResult: PagedResult<AccountSummary>;
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
        private accountService: AccountService,
        private messageService: MessageService,
        private authService: AuthService
    ) { }

    ngOnChanges(changes: SimpleChanges) {
        this.search();
    }

    ngOnInit() {
        this.search();
    }

    search() {
        if (!this.working) {
            this.working = true;
            this.dataFilter.filter = this.filter;

            this.accountService.list(this.dataFilter).subscribe((result) => {
                this.pagedResult = result;
                this.working = false;
            });
        }
    }

    reset() {
        this.dataFilter.skip = 0;
        this.search();
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

    openAccountUpdate(account: AccountSummary) {
        const dialogRef = this.dialog.open(AccountEditComponent, {
            width: '820px',
            data: { account: account }
        });

        dialogRef.afterClosed().subscribe(result => {
            this.updateListeners();
            this.search();
            console.log('The dialog was closed');
        });
    }

    openAccountAdd() {
        const dialogRef = this.dialog.open(AccountEditComponent, {
            width: '820px',
            data: { account: null }
        });

        dialogRef.afterClosed().subscribe(result => {
            this.updateListeners();
            this.search();
            console.log('The dialog was closed');
        });
    }

    updateListeners() {
        this.messageService.notify('bucket-update');
    }
}

