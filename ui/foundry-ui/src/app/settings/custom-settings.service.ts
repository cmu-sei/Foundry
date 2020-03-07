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
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ConfigurationItem, PagedResult, SettingCreate, SettingDetail, SettingUpdate } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class CustomSettingsService {
  public apis: any = [
    { name: 'Market', status: { available: false } },
    { name: 'Communcation', status: { available: false } },
    { name: 'Analytics', status: { available: false } },
    { name: 'Order Portal', status: { available: false } },
    { name: 'Buckets', status: { available: false } },
    { name: 'Groups', status: { available: false } },
  ];
  constructor(
    private http: HttpClient,
    private settingsSvc: SettingsService,
    private auth: AuthService
  ) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  public list(search: any): Observable<PagedResult<SettingDetail>> {
    return this.http.get<PagedResult<SettingDetail>>(this.url() + '/settings' + this.auth.queryStringify(search, '?'));
  }

  public configuration(): Observable<Array<ConfigurationItem>> {
    return this.http.get<Array<ConfigurationItem>>(this.url() + '/configuration');
  }

  public load(key: string): Observable<SettingDetail> {
    return this.auth.getAnonymous(this.url() + '/setting/' + key);
  }

  public status(): Observable<any> {
    return this.auth.getAnonymous(this.url() + '/status');
  }

  public update(setting: SettingUpdate): Observable<SettingDetail> {
    return this.http.put(this.url() + '/setting/' + setting.key, setting);
  }

  public add(setting: SettingCreate): Observable<SettingDetail> {
    return this.http.post(this.url() + '/settings', setting);
  }
}

