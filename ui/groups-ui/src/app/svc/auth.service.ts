/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User, UserManager } from 'oidc-jam';
import { Observable, Subject } from 'rxjs';
import { MessageService } from './message.service';
import { SettingsService } from './settings.service';



@Injectable()
export class AuthService {
    mgr: UserManager;
    public currentUser: User;
    loggedIn  = false;
    lastCall: number;
    private userSource: Subject<User> = new Subject<User>();
    public user$: Observable<User> = this.userSource.asObservable();
    private tokenStatus: Subject<AuthTokenState> = new Subject<AuthTokenState>();
    public tokenStatus$: Observable<AuthTokenState> = this.tokenStatus.asObservable();

    constructor(
        private settingsSvc: SettingsService,
        private msgService: MessageService,
        private httpClient: HttpClient,
    ) {
        this.mgr = new UserManager(this.settingsSvc.settings.clientSettings.oidc);
        this.mgr.events.addUserLoaded(user => { this.onTokenLoaded(user); });
        this.mgr.events.addUserUnloaded(user => { this.onTokenUnloaded(user); });
        this.mgr.events.addAccessTokenExpiring(e => { this.onTokenExpiring(e); });
        this.mgr.events.addAccessTokenExpired(e => { this.onTokenExpired(e); });

        this.init();
    }

    init() {
        this.mgr.getUser().then(user => {
            if (user) { this.onTokenLoaded(user); }
        });
    }


    mapData(response: any) {
        return response.json();
    }

    isAuthenticated(): Promise<boolean> {
        if (!!this.currentUser) {
            return Promise.resolve(true);
        }

        return this.mgr.getUser().then(
            (user) => {
                return Promise.resolve(!!user);
            }
        );
    }

    getAuthorizationHeaderValue(): string {
        this.markAction();
        return 'Bearer ' + ((this.currentUser)
            ? this.currentUser.access_token
            : 'no_token');
    }

    getAuthorizationHeader(): string {
        this.markAction();
        return ((this.currentUser)
            ? this.currentUser.token_type + ' ' + this.currentUser.access_token
            : 'no_token');
    }

    markAction() {
        this.lastCall = Date.now();
    }

    private onTokenLoaded(user) {
        this.currentUser = user;
        this.loggedIn = (user !== null);
        this.userSource.next(user);
        this.tokenStatus.next(AuthTokenState.valid);
    }

    private onTokenUnloaded(user) {
        this.currentUser = user;
        this.userSource.next(user);
        this.tokenStatus.next(AuthTokenState.invalid);
    }

    private onTokenExpiring(e) {
        if (Date.now() - this.lastCall < 30000) {
            this.refreshToken();
        } else {
            this.tokenStatus.next(AuthTokenState.expiring);
        }
    }

    private onTokenExpired(e) {

        // technically, we want Expiring to warning the user
        // and Expired to clean up.  But we need to adjust the
        // Expired timeout to be a few seconds before the token
        // expiration so any cleanup requiring a token will still
        // work (like a signalr hub, for instance).

        setTimeout(() => {
            this.mgr.removeUser();
            this.tokenStatus.next(AuthTokenState.expired);
        }, 2000);
    }

    initiateLogin(url) {
        this.mgr.signinRedirect({ state: url }).then(function () {
            // console.log("signinRedirect done");
        }).catch(err =>
            this.msgService.add(err)
        );
    }

    validateLogin(url): Promise<User> {
        return this.mgr.signinRedirectCallback(url);
    }

    logout() {
        // if (this.currentUser) {
            this.mgr.signoutRedirect().then(resp => {
                // console.log("initiated external logout");
                sessionStorage.clear();
            }).catch(err => {
                console.log(err.text());
            });
        // }
    }

    refreshToken() {
        this.mgr.signinSilent().then(() => { });
    }

    silentLoginCallback(): void {
        this.mgr.signinSilentCallback();
    }


    encodeKVP(key: string, value: string) {
        return encodeURIComponent(key) + '=' + encodeURIComponent(value);
    }

    queryStringify(obj: object, prefix?: string) {
        const segments = [];
        for (const p in obj) {
            const prop = obj[p];
            if (prop) {
                if (Array.isArray(prop)) {
                    prop.forEach(element => {
                        segments.push(this.encodeKVP(p, element));
                    });
                } else {
                    segments.push(this.encodeKVP(p, prop));
                }
            }
        }
        const qs = segments.join('&');
        return (qs) ? (prefix || '') + qs : '';
    }

    addAuth(opts: any) {
        if (opts.headers == null) { opts.headers = new Headers(); }
        opts.headers.set('Authorization', this.getAuthorizationHeaderValue());
    }

    request(verb, url, files: any, opts = {}) {

        const formData = new FormData();

        for (const file of files) {
            formData.append(file.name, file);
        }

        const uploadReq = new HttpRequest<FormData>(verb, url, formData, {
            reportProgress: true,
            headers: new HttpHeaders({ 'Authorization': this.getAuthorizationHeaderValue() })
        });

        return this.httpClient.request(uploadReq);
    }
}

export enum AuthTokenState {
    valid = <any>'valid',
    invalid = <any>'invalid',
    expiring = <any>'expiring',
    expired = <any>'expired'
}

