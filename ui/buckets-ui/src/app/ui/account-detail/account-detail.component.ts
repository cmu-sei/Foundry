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
import { AccountDetail, AccountDetailBucket, DataFilter } from '../../model';
import { MessageService } from '../../svc/message.service';
import { AccountService } from '../../svc/account.service';

@Component({
    selector: 'account-detail',
    templateUrl: './account-detail.component.html',
    styleUrls: ['./account-detail.component.scss']
})
export class AccountDetailComponent implements OnInit {
    public account: AccountDetail;

    constructor(
        private route: ActivatedRoute,
        private accountService: AccountService,
        private messageService: MessageService,
        public dialog: MatDialog,
    ) { }

    ngOnInit() {
        this.route.params.pipe(
            switchMap((params: Params) => this.accountService.load(params['id'])))
            .subscribe(result => { this.account = result; }, error => { console.log(error); });
    }

    load() {
        this.accountService.load(this.account.globalId).subscribe(result => {
            this.account = result;
        }, error => { console.log(error); });
    }

    updateListeners() {
        this.messageService.notify('account-update');
    }
}

