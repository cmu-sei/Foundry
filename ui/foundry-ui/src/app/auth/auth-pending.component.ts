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
import { ActivatedRoute, Router } from '@angular/router';
import { AnalyticsService } from '../analytics/analytics.service';
import { SettingsService } from '../root/settings.service';
import { AuthService } from './auth.service';
import { UserEventCreate } from './../core-api-models';
import { BaseComponent } from '../shared/components/base.component';

@Component({
  selector: 'auth-pending',
  templateUrl: 'auth-pending.component.html'
})
export class AuthPendingComponent extends BaseComponent implements OnInit {

  message: string = 'Verifying Authentication';
  errorMessage: string;
  syncErrorMessage: string;
  showLogin: boolean = false;
  isIE = false;
  registerLoginEvents = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: AuthService,
    private analyticsService: AnalyticsService,
    private settingsService: SettingsService
  ) {
    super();
  }

  ngOnInit() {
    this.route.fragment.subscribe(frag => {
      this.validate(frag);
    });

    this.isIE = /msie\s|trident\//i.test(window.navigator.userAgent);
    this.registerLoginEvents = this.settingsService.settings.reporting.registerLoginEvents;
  }

  login() {
    this.service.initiateLogin('/dashboard');
  }

  validate(frag) {
    const now = Math.floor(Date.now() / 1000);
    this.service.validateLogin(frag)
      .then((user) => {
        if (user && user.state) {
          this.message = 'Authentication Verified';
          if (this.registerLoginEvents) {
            const model: UserEventCreate = {
              type: 'logged-in'
            };
            this.$.push(this.analyticsService.addUserEvent(model).subscribe(result => { }));
          }
          this.router.navigateByUrl(user.state || '/home');
        }
      },
        (err) => {
          this.message = 'Authentication Failed';

          if (err) {
            this.errorMessage = err.toString();
          }

          if (this.errorMessage.includes('No matching state found in storage', 0)) {
            this.showLogin = true;
          }

          if (this.errorMessage.indexOf('is in the') > 0) {
            let ts = this.errorMessage.split(':').pop().trim();
            let tokenTime = new Date(parseInt(ts) * 1000);
            let nowTime = new Date(now * 1000);
            this.syncErrorMessage = `Please verify that your system time is correct. ${this.errorMessage} ${now.toString()} (token: ${tokenTime.toISOString()}, browser: ${nowTime.toISOString()})`;
          }
        }
      );
  }
}

