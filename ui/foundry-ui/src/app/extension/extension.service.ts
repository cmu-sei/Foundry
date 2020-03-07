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
import { AuthService } from '../auth/auth.service';
import { ApplicationSummary, ApplicationUpdate, DataFilter, PagedResult } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class ExtensionService {

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService
  ) {
  }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  list(search: DataFilter) {
    return this.http.get<PagedResult<ApplicationSummary>>(this.url() + '/apps' + this.auth.queryStringify(search, '?'));
  }

  myApps() {
    return this.http.get<PagedResult<ApplicationSummary>>(this.url() + '/myapps');
  }

  add(app: ApplicationSummary) {
    return this.http.put(this.url() + '/myapps/' + app.slug, null);
  }

  push(ids: Array<number>) {
    return this.http.post(this.url() + '/apps/push', ids);
  }

  sync() {
    return this.http.post(this.url() + '/apps/sync', null);
  }

  update(model: ApplicationUpdate) {
    return this.http.put<ApplicationSummary>(this.url() + '/app/' + model.id, model);
  }

  remove(app: ApplicationSummary) {
    return this.http.delete(this.url() + '/myapps/' + app.slug);
  }
}

