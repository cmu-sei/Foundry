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
import { Injectable } from '@angular/core';
import { UserManagerSettings } from 'oidc-client';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class SettingsService {

  url = 'assets/config/settings.json';
  settings: Settings;

  constructor(
    private http: HttpClient
  ) {

  }

  public load(): Promise<boolean> {
    return new Promise((resolve, reject) => {
        this.http.get<Settings>(this.url)
            .pipe(
                catchError((error: any): any => {
                    console.log('invalid settings.json');
                    return of(new Object());
                })
            )
            .subscribe((data: Settings) => {
                this.settings = { ...this.settings, ...data };
                this.http.get(this.url.replace(/json$/, 'env.json'))
                    .pipe(
                        catchError((error: any): any => {
                            return of(new Object());
                        })
                    )
                    .subscribe((customData: Settings) => {
                        this.settings = { ...this.settings, ...customData };
                        resolve(true);
                    });
            });
    });
}
}

export interface Settings {
  branding?: BrandingSettings;
  errorHandling?: ErrorHandlingSettings;
  clientSettings?: ClientSettings;
  connectionStrings?: ConnectionStringSettings;
  corsPolicy?: CorsPolicySettings;
  logging?: LoggingSettings;
  urls?: UrlSettings;
}

export interface BrandingSettings {
    applicationName?: string;
    identityLogoBaseUrl?: string;
    commit?: string;
}

export interface ErrorHandlingSettings {
    showDeveloperExceptions?: boolean;
}

export interface ConnectionStringSettings {
    postgreSQL?: string;
    sqlite?: string;
    sqlServer?: string;
}

export interface CorsPolicySettings {
    origins?: any[];
    methods?: any[];
    headers?: any[];
    allowAnyOrigin?: boolean;
    allowAnyMethod?: boolean;
    allowAnyHeader?: boolean;
    supportsCredentials?: boolean;
}

export interface ClientSettings {
    oidc?: UserManagerSettings;
}


export interface LoggingSettings {
    includeScopes?: boolean;
    logLevel?: LogLevelSettings;
}

export interface LogLevelSettings {
    default?: string;
}

export interface  UrlSettings {
    apiUrl?: string;
}

