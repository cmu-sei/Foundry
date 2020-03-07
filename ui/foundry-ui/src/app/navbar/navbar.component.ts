/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { AnalyticsService } from '../analytics/analytics.service';
import { AuthService } from '../auth/auth.service';
import { NotificationSummary, ClientEventCreate, PageViewMetric, ProfileDetail, ProfileInfo } from '../core-api-models';
import { NotificationService } from '../notification/notification.service';
import { ProfileService } from '../profile/profile.service';
import { MessageService } from '../root/message.service';
import { SettingsService } from '../root/settings.service';
import { BaseComponent } from '../shared/components/base.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['../root/app.component.scss'],
  providers: [NgbDropdownConfig]
})

export class NavbarComponent extends BaseComponent implements OnInit, OnDestroy {
  profile: ProfileDetail;
  profile$: Subscription;
  public lastUrl = '';
  public requestUrl: string;
  public AppConfig: any;
  public pageViewMetric: PageViewMetric;
  public routeName: string;
  public name: string;
  public type: string;
  public notificationCount = 0;
  public analyticsVisible: boolean;
  appName: string;
  navColor: string;
  appLogo: string;
  navigationSent = false;
  navigationEnd: any;
  registerSearchEvents = false;
  loadingNotifications = false;
  public notificationsHistory: Array<NotificationSummary>;
  profileInfo: ProfileInfo;

  constructor(
    private auth: AuthService,
    private router: Router,
    private profileService: ProfileService,
    private notificationService: NotificationService,
    private analyticsService: AnalyticsService,
    private settingsSvc: SettingsService,
    private messageService: MessageService,
    config: NgbDropdownConfig
  ) {
    super();
    config.placement = 'bottom-right';
    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        this.navigationSent = false;
        this.navigationEnd = e;
        if (this.profile) {
          this.sendPageView();
          this.getNotificationCount();
        } else {
          console.log('NavigationEnd but no profile');
        }
      }
    });

    this.$.push(this.messageService.listen().subscribe((m: any) => {
      if (m === 'notification-update') {
        this.getNotificationCount();
      }
    }));
  }

  togglePageViewMetrics(event) {
    if (event) {
      this.$.push(this.analyticsService.metric(this.lastUrl).subscribe(result => {
        this.pageViewMetric = result;
      }));
    }
  }

  navigate(url: string) {
    this.router.navigateByUrl(url);
  }

  initProfile(profile: ProfileDetail) {
    this.profile = profile;
    this.getNotificationCount();
    sessionStorage.setItem('currentUserId', profile.id.toString());
    console.log('profile loaded');
    this.sendPageView();

    // fetch identity profile
    this.$.push(this.profileService.getProfileInfo(this.profile.globalId).subscribe(
      (info: ProfileInfo) => {
        this.profileInfo = info;
      }
    ));
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe(p => {
      this.initProfile(p);
    }));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    this.requestUrl = this.settingsSvc.settings.clientSettings.urls.requestUrl;
    this.appName = this.settingsSvc.settings.branding.applicationName;
    this.navColor = this.settingsSvc.settings.branding.navColor;
    this.appLogo = this.settingsSvc.settings.branding.logoUrl;
    this.registerSearchEvents = this.settingsSvc.settings.reporting.registerSearchEvents;

    this.$.push(this.messageService.listen().subscribe((m: any) => {
      if (m === 'clear-profile') {
        this.profile = null;
      }
    }));
  }

  defaultImage() {
    return this.profileService.defaultImageUri;
  }

  getNotificationCount() {
    if (this.profile) {
      if (!this.loadingNotifications) {
        this.loadingNotifications = true;
        this.$.push(this.notificationService.unreadCount().subscribe(result => {
          this.notificationCount = result;
          this.loadingNotifications = false;
        }));
      }
    }
  }

  ngOnDestroy() {
    this.profile$.unsubscribe();
  }

  logout() {
    this.auth.logout();
  }

  sendPageView() {
    if (this.profile && !this.navigationSent) {
      const model: ClientEventCreate = {
        url: this.navigationEnd.url,
        lastUrl: this.lastUrl,
        type: 'page-view'
      };

      this.$.push(this.analyticsService.addClientEvent(model).subscribe(() => { }, () => { }));
      this.lastUrl = this.navigationEnd.url;

      this.navigationSent = true;
    }
  }

  toggleCollapsedNav() {
    this.AppConfig.navCollapsed = !this.AppConfig.navCollapsed;
  }

  connectNotifications() {
    this.$.push(this.notificationService.notificationHistory.subscribe(() => {
      this.notificationService.start(this.profile.globalId).then(
        () => {
          // debugger
        });
    }));
  }

  loggedIn(): boolean {
    return this.auth.loggedIn;
  }

}

