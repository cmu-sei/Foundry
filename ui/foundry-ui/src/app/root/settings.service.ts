/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient } from '@angular/common/http';
import { Injectable, InjectionToken } from '@angular/core';
import { UserManagerSettings } from 'oidc-jam';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';


@Injectable()
export class SettingsService {
  url = 'assets/config/settings.json';
  originUrl: string;
  showdownOptions: any;
  settings: Settings;

  constructor(
    private http: HttpClient
  ) {
    this.originUrl = getOriginUrl();
    this.showdownOptions = getShowdownOpts();
    this.timezoneOffsetMinutes = (new Date).getTimezoneOffset();
  }


  timezoneOffsetMinutes: number;
  tabRefs: any = {};
  public showTab(url: string): void {
    if (typeof this.tabRefs[url] === 'undefined' || this.tabRefs[url].closed) {
      this.tabRefs[url] = window.open(url, '_blank');
    } else {
      this.tabRefs[url].focus();
    }
  }

  public load(): Promise<boolean> {
    return new Promise((resolve, reject) => {
      this.http.get(this.url)
        .pipe(
          catchError((error: any): any => {
            console.log('invalid settings.json');
            return of(new Object());
          })
        )
        .subscribe((data: Settings) => {
          this.settings = data;
          this.http.get(this.url.replace(/json$/, 'env.json'))
            .pipe(
              catchError((error: any): any => {
                return of(new Object());
              })
            )
            .subscribe((customData: Settings) => {
             //TODO: implement a deep merge
             if (customData.branding) {this.settings.branding = { ...this.settings.branding, ...customData.branding }};
             if (customData.reporting) {this.settings.reporting = { ...this.settings.reporting, ...customData.reporting }};
             if (customData.clientSettings) {
               if (customData.clientSettings.oidc) {this.settings.clientSettings.oidc = { ...this.settings.clientSettings.oidc, ...customData.clientSettings.oidc }};
               if (customData.clientSettings.urls) {this.settings.clientSettings.urls = { ...this.settings.clientSettings.urls, ...customData.clientSettings.urls }};
             }
              resolve(true);
            });
        });
    });
  }
}

export const ORIGIN_URL = new InjectionToken<string>('ORIGIN_URL');
export function getOriginUrl() {
  return (window && window.location) ? window.location.origin : '';
}

export const SHOWDOWN_OPTS = new InjectionToken<string>('SHOWDOWN_OPTS');
export function getShowdownOpts() {
  return {
    strikethrough: true,
    tables: true,
    parseImgDimensions: true,
    smoothLivePreview: true,
    tasklists: true
  };
}

export interface Settings {
  branding?: BrandingSettings;
  clientSettings?: ClientSettings;
  reporting?: ReportingSettings;
}

export interface BrandingSettings {
  applicationName?: string;
  logoUrl?: string;
  navColor?: string;
  identityLogoBaseUrl?: string;
}

export interface ClientSettings {
  oidc?: UserManagerSettings;
  urls?: AppUrlSettings;
  notifications?: NotificationsSettings;
}

export interface NotificationsSettings {
  group?: NotificationRewrite;
}

export interface NotificationRewrite {
  from?: string;
  to?: string;
}

export interface ReportingSettings {
  displaySTEPLeaderboardData?: boolean;
  displayLRSData?: boolean;
  exerciseLeaderboardIds?: any[];
  exerciseLeaderboardMaxResultsCount?: number;
  registerSearchEvents?: boolean;
  registerContentLaunchEvents?: boolean;
  registerContentViewEvents?: boolean;
  registerLoginEvents?: boolean;
}

export interface AppUrlSettings {
  coreUrl?: string;
  uploadUrl?: string;
  requestUrl?: string;
  analyticsUrl?: string;
  communicationUrl?: string;
  forumUrl?: string;
  profileQueryUrl?: string;
  profileEditUrl?: string;
  groupsUrl?: string;
}

