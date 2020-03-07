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
import { map } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { DataFilter, PagedResult, Tag } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class TagService {

  constructor(
    private http: HttpClient,
    private settingsSvc: SettingsService,
    private auth: AuthService) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  public list(search: DataFilter): Observable<PagedResult<Tag>> {
    return this.http.get<PagedResult<Tag>>(this.url() + '/tags' + this.auth.queryStringify(search, '?'));
  }

  public count(): Observable<number> {
    return this.http.get<number>(this.url() + '/tags/count');
  }

  public delete(name: string): Observable<boolean> {
    return this.http.delete<boolean>(this.url() + '/tag/' + name);
  }

  public add(model) {
    return this.http.post(this.url() + '/tags', model).pipe(map(() => {
    }));
  }

  public update(id, model) {
    return this.http.put(this.url() + '/tag/' + id, model).pipe(map(() => {
    }));
  }
}

