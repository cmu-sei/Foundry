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
import { MemberCreate } from '../models';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService
  ) { }

  url() {
    return this.settingsSvc.settings.urls.coreUrl;
  }

  public members(id: string, search): Observable<any> {
    return this.http.get<any>(this.url() + '/group/' + id + '/members' + this.auth.queryStringify(search, '?'));
  }

  public addMember(groupId: string, model: MemberCreate): Observable<MemberCreate> {
    return this.http.post<MemberCreate>(this.url() + '/group/' + groupId + '/members', model);
  }

  public updateMember(groupId: string, accountId: string, model: MemberCreate): Observable<MemberCreate> {
    return this.http.put<MemberCreate>(this.url() + '/group/' + groupId + '/member/' + accountId, model);
  }

  public removeMember(groupId: string, accountId: string) {
    return this.http.delete(this.url() + '/group/' + groupId + '/member/' + accountId);
  }

 public slugifyName(input: string) {
      return input.toString().toLowerCase()
        .replace(/\s+/g, '-')
        .replace(/[^\w\-]+/g, '')
        .replace(/\-\-+/g, '-')
        .replace(/^-+/, '')
        .replace(/-+$/, '');
  }

}

