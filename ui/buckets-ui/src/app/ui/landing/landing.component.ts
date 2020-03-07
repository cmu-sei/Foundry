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
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../svc/auth.service';
import { SettingsService } from '../../svc/settings.service';
import { BucketService } from '../../svc/bucket.service';
import { PagedResult, BucketSummary } from '../../model';

@Component({
    selector: 'app-landing',
    templateUrl: './landing.component.html',
    styleUrls: ['./landing.component.scss']
})
export class LandingComponent implements OnInit {
    title = '';
    user: any;
    profile$: Subscription;
    userExists: boolean;
    result: PagedResult<BucketSummary>;

    constructor(
        private settingsSvc: SettingsService,
        private authService: AuthService,
        private bucketService: BucketService,
        private router: Router
    ) { }

    ngOnInit() {
        this.title = this.settingsSvc.settings.branding.applicationName;

        if (this.authService.currentUser) {
            this.load();
        }
    }

    load() {

        this.bucketService.list({ sort: 'name' }).subscribe((result) => {
            this.result = result;
        });
    }
}

