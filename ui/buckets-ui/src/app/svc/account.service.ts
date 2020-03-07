/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { AccountDetail, AccountSummary, DataFilter, PagedResult, AccountUpdate, AccountCreate } from '../model';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    public account: AccountDetail;
    accountSource: Subject<AccountDetail> = new Subject<AccountDetail>();
    account$: Observable<AccountDetail> = this.accountSource.asObservable();

    constructor(
        private http: HttpClient,
        private settingsService: SettingsService,
        private authService: AuthService
    ) {
        this.authService.user$.subscribe((p) => {
            var user = (p) ? p.profile : p;
            if (user) {
                var globalId = this.authService.currentUser.profile.sub;
                this.load(globalId).subscribe((account) => {
                    this.account = account;
                    this.accountSource.next(this.account);
                });
            }
        });
    }

    url() {
        return this.settingsService.settings.urls.apiUrl;
    }

    public load(globalId: string): Observable<AccountDetail> {
        return this.http.get<AccountDetail>(this.url() + '/account/' + globalId);
    }

    public list(search: DataFilter): Observable<PagedResult<AccountSummary>> {
        return this.http.get<PagedResult<AccountSummary>>(this.url() + '/accounts' + this.authService.queryStringify(search, '?'));
    }

    public update(model: AccountUpdate): Observable<AccountDetail> {
        return this.http.put<AccountDetail>(this.url() + '/account/' + model.globalId, model);
    }

    public add(model: AccountCreate): Observable<AccountDetail> {
        return this.http.post<AccountDetail>(this.url() + '/accounts', model);
    }
}

