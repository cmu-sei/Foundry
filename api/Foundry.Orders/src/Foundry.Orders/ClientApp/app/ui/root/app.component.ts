/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { OrderService } from '../../api/order.service';
import { ProfileService } from '../../api/profile.service';
import { AuthService, AuthTokenState } from '../../svc/auth.service';
import { SettingsService } from '../../svc/settings.service';
import { SharedService } from '../../svc/shared.service';
import { ProfileSummary } from '../../api/gen/models';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
    profile: ProfileSummary;
    profile$: Subscription;
    status$: Subscription;
    message: any;
    subscription: Subscription;
    count: number = 0;
    showExpiring: boolean;
    showExpired: boolean;

    constructor(
        private settings: SettingsService,
        private sharedService: SharedService,
        private profileService: ProfileService,
        private orderService: OrderService,
        private auth: AuthService
    ){
        this.appName = settings.branding.applicationName || "Order Portal";
        this.subscription = this.sharedService.getMessage().subscribe(message => { this.message = message; })
    }

    appName: string = "";

    initProfile(profile: ProfileSummary) {
        if (profile) {
            this.profile = profile;
            if (this.profile.isAdministrator) {
                this.getSubmittedCount();
            }
        }
    }

    ngOnInit() {

        this.profileService.profile$.subscribe(p => {
            this.initProfile(p);
        });

        this.initProfile(this.profileService.profile);

        this.status$ = this.auth.tokenStatus$.subscribe(
            (status) => {
                this.showExpiring = (status == AuthTokenState.expiring);
                this.showExpired = (status == AuthTokenState.expired);
                if (status == AuthTokenState.invalid) {
                    //this.router.navigate(['/home']);
                    //onTokenExpired in auth service is calling logout and redirecting.
                }
        });
    }

    getSubmittedCount() {
        this.orderService.getOrders({ filter: 'status=submitted' }).subscribe((result) => {
            this.count = result.total;
        });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
        this.status$.unsubscribe();
    }

    continue() {
        this.auth.refreshToken();
    }

    logout() {
        this.auth.logout();
    }
}

