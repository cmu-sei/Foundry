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
import { Converter } from 'showdown/dist/showdown';
import { AnalyticsService } from '../analytics/analytics.service';
import { AuthService } from '../auth/auth.service';
import { ProfileDetail, SettingDetail } from '../core-api-models';
import { DocumentService } from '../document/document.service';
import { NotificationService } from '../notification/notification.service';
import { ProfileService } from '../profile/profile.service';
import { RequestService } from '../request/request.service';
import { CustomSettingsService } from '../settings/custom-settings.service';
import { SHOWDOWN_OPTS } from '../shared/constants/ui-params';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';
import { BaseComponent } from '../shared/components/base.component';

@Component({
  // moduleId: module.id,
  selector: 'home',
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss']
})
export class HomeComponent extends BaseComponent implements OnInit {

  constructor(
    private settingsSvc: SettingsService,
    private authService: AuthService,
    private router: Router,
    private profileService: ProfileService,
    private customSettingService: CustomSettingsService,
    public msgService: MessageService,
    public notificationService: NotificationService,
    public analyticsService: AnalyticsService,
    public requestService: RequestService,
    public documentService: DocumentService
  ) {
    super();
    this.converter = new Converter(SHOWDOWN_OPTS);
  }

  errorMessage: string;
  appname: string;
  profile: ProfileDetail;
  profile$: Subscription;
  landingText: string;
  converter: Converter;
  isIE = false;

  ngOnInit() {

    this.isIE = /msie\s|trident\//i.test(window.navigator.userAgent);

    this.appname = this.settingsSvc.settings.branding.applicationName;
    if (this.authService.currentUser != null) {
      this.router.navigate(['/dashboard']);
    }

    this.$.push(this.customSettingService.load('landingPageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          if (result.value.length) {
            this.landingText = result.value;
          } else {
            // tslint:disable-next-line:max-line-length
            this.landingText = this.appname + ' is a marketplace for cyber operations training that matches users to mission-focused and community-curated training content.';
          }
        } else {
          // tslint:disable-next-line:max-line-length
          this.landingText = this.appname + ' is a marketplace for cyber operations training that matches users to mission-focused and community-curated training content.';
        }
      }));
  }

  startSigninMainWindow() {
    this.authService.initiateLogin('/dashboard');
  }

  renderedDescription() {
    return this.converter.makeHtml(this.landingText);
  }
}

