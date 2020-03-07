/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Injectable } from '@angular/core';
import { UserManager, UserManagerSettings, WebStorageStateStore, Log, MetadataService, User } from 'oidc-client';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { SettingsService } from './settings.service';

@Injectable()
export class AuthService {
    mgr: UserManager;
    currentUser: User;
    externalLoginName: string;
    loginSettings: any;
    redirectUrl: string;
    private userSource: Subject<User> = new Subject<User>();
    public user$: Observable<User> = this.userSource.asObservable();
    private tokenStatus: Subject<AuthTokenState> = new Subject<AuthTokenState>();
    tokenStatus$: Observable<AuthTokenState> = this.tokenStatus.asObservable();
    lastCall : number;

    constructor(
        private settings: SettingsService
    ) {
        this.externalLoginName = this.settings.oidc.name || "External";
        this.mgr = new UserManager(this.settings.oidc);
        this.mgr.events.addUserLoaded(user => { this.onTokenLoaded(user); });
        this.mgr.events.addUserUnloaded(user => { this.onTokenUnloaded(user); });
        this.mgr.events.addAccessTokenExpiring(e => { this.onTokenExpiring(e); });
        this.mgr.events.addAccessTokenExpired(e => { this.onTokenExpired(e); });

        this.init();
    }

    init() {
        this.mgr.getUser().then((user) => {
            if (user) {
                this.onTokenLoaded(user);
            }
        });
    }

    isAuthenticated() : Promise<boolean> {
        if (!!this.currentUser)
            return Promise.resolve(true);

        return this.mgr.getUser().then(
            (user) => {
                return Promise.resolve(!!user);
            }
        );
    }

    getAuthorizationHeader() : string {
        this.markAction();
        return ((this.currentUser)
            ? this.currentUser.token_type + " " + this.currentUser.access_token
            : "no_token");
    }

    markAction() {
        this.lastCall = Date.now();
    }

    private onTokenLoaded(user: User) {
        this.currentUser = user;
        this.userSource.next(user);
        this.tokenStatus.next(AuthTokenState.valid);
    }

    private onTokenUnloaded(user: User) {
        this.currentUser = user;
        this.userSource.next(user);
        this.tokenStatus.next(AuthTokenState.invalid);
    }

    private onTokenExpiring(e: any) {
        if (Date.now() - this.lastCall < 30000)
            this.refreshToken();
        else
            this.tokenStatus.next(AuthTokenState.expiring);
    }

    private onTokenExpired(e: any) {
        this.tokenStatus.next(AuthTokenState.expired);

        //give any clean process 10 seconds or so.
        setTimeout(() => {
            this.mgr.removeUser();
        }, 10000);
    }

    externalLogin(url: string) {
        this.mgr.signinRedirect({ state: url }).then(function () {
            //console.log("signinRedirect done");
        }).catch(function (err) {
            debugger
            console.log(err);
        });
    }

    externalLoginCallback(url: string) : Promise<User> {
        return this.mgr.signinRedirectCallback(url);
    }

    logout() {
        if (this.currentUser) {
            this.mgr.signoutRedirect().then(resp => {
                console.log("initiated external logout");
            }).catch(err => {
                console.log(err);
            });
        }
    }

    refreshToken() {
        this.mgr.signinSilent().then(() => {
        });
    }

    isAdmin() {
        return (this.currentUser && this.currentUser.profile.isAdmin);
    }

    cleanUrl(url: string) {
        return url
            .replace(/[?&]auth-hint=[^&]*/, '')
            .replace(/[?&]contentId=[^&]*/, '')
            .replace(/[?&]profileId=[^&]*/, '');
    }
}

export enum AuthTokenState {
    valid = <any>'valid',
    invalid = <any>'invalid',
    expiring = <any>'expiring',
    expired = <any>'expired'
}

