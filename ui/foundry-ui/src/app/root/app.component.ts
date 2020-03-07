/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, HostBinding, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { AuthService, AuthTokenState } from '../auth/auth.service';
import { APPCONFIG } from '../config';
import { fadeInAnimation } from '../shared/animations';
import { BaseComponent } from '../shared/components/base.component';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';

@Component({
  selector: 'browser-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  animations: [fadeInAnimation]
})
export class AppComponent extends BaseComponent implements OnInit {
  @HostBinding('@routeAnimation') routeAnimation = true;
  @HostBinding('style.display') display = 'block';
  showExpiring: boolean;
  showExpired: boolean;
  public AppConfig: any;
  appName: string;

  constructor(
    private auth: AuthService,
    private router: Router,
    private msgSvc: MessageService,
    private settingsSvc: SettingsService,
    private titleService: Title
  ) {
    super();
  }
  ngOnInit() {
    this.appName = this.settingsSvc.settings.branding.applicationName;
    this.setTitle(this.appName);
    this.AppConfig = APPCONFIG;

    this.$.push(this.auth.tokenStatus$.subscribe(
      (status) => {
        this.showExpiring = (status === AuthTokenState.expiring);
        if (status === AuthTokenState.expired) {
          this.clearExpiredUser();
        }
      }));
  }

  public setTitle(newTitle: string) {
    this.titleService.setTitle(newTitle);
  }

  continue() {
    this.auth.refreshToken();
  }

  clearExpiredUser() {
    this.msgSvc.notify('clear-profile');
    sessionStorage.removeItem('currentUserId');
    this.router.navigate(['/home']);
  }
}

