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
import { AuthService } from '../../svc/auth.service';
import { BucketService } from '../../svc/bucket.service';
import { FileService } from '../../svc/file.service';
import { SidenavComponent } from '../sidenav/sidenav.component';
import { AccountService } from '../../svc/account.service';
import { AccountDetail } from '../../model';

@Component({
    selector: 'app-file-browser',
    templateUrl: './file-browser.component.html',
    styleUrls: ['./file-browser.component.scss']
})

export class FileBrowserComponent implements OnInit {
    public account: AccountDetail;

    constructor(
        public dialog: MatDialog,
        private fileService: FileService,
        private bucketService: BucketService,
        private authService: AuthService,
        private componentRef: SidenavComponent,
        private accountService: AccountService
    ) { }

    initAccount(account: AccountDetail) {
        if (account) {
            this.account = account;
        }
    }

    ngOnInit() {
        this.accountService.account$.subscribe(account => { this.initAccount(account); });
        this.initAccount(this.accountService.account);
    }
}

