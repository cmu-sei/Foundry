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
import { GroupCreate, GroupDetail, GroupSummary, GroupUpdate, MemberDetail, PagedResult, TreeGroupSummary } from '../models';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';

@Injectable({
  providedIn: 'root'
})
export class GroupService {

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService
  ) { }

  url() {
    return this.settingsSvc.settings.urls.coreUrl;
  }

  public load(id: string): Observable<GroupDetail> {
    return this.http.get<GroupDetail>(this.url() + '/group/' + id);
  }

  public list(search): Observable<PagedResult<GroupSummary>> {
    return this.http.get<PagedResult<GroupSummary>>(this.url() + '/groups' + this.auth.queryStringify(search, '?'));
  }

  public tree(): Observable<TreeGroupSummary> {
    return this.http.get<TreeGroupSummary>(this.url() + '/tree');
  }

  public listChildren(id: string): Observable<PagedResult<GroupSummary>> {
    return this.http.get<PagedResult<GroupSummary>>(this.url() + '/group/' + id + '/children');
  }

  public add(group: GroupCreate): Observable<GroupDetail> {
    return this.http.post<GroupDetail>(this.url() + '/groups', group);
  }

  public update(group: GroupUpdate): Observable<GroupDetail> {
    return this.http.put<GroupDetail>(this.url() + '/group/' + group.id, group);
  }

  public delete(id: any) {
    return this.http.delete(this.url() + '/group/' + id);
  }

  public migrate() {
    return this.http.post(this.url() + '/migrate', null);
  }

  /* member invite codes */
  public getByMemberInviteCode(code: string): Observable<GroupDetail> {
    return this.http.get<GroupDetail>(this.url() + '/member-invite/' + code);
  }

  public acceptMemberInvite(code: string): Observable<MemberDetail> {
    return this.http.put<MemberDetail>(this.url() + '/member-invite/' + code, null);
  }

  public getMemberInviteCodeById(id: string) {
    return this.http.get(this.url() + '/group/' + id + '/member-invite', { responseType: 'text' });
  }

  public updateMemberInviteCodeById(id: string) {
    return this.http.put(this.url() + '/group/' + id + '/member-invite', null, { responseType: 'text' });
  }

  public deleteMemberInviteCodeById(id: string): Observable<boolean> {
    return this.http.delete<boolean>(this.url() + '/group/' + id + '/member-invite');
  }

  /* group invite codes */
  public getByGroupInviteCode(code: string): Observable<GroupDetail> {
    return this.http.get<GroupDetail>(this.url() + '/group-invite/' + code);
  }

  public acceptGroupInvite(code: string, acceptingGroupId: string): Observable<MemberDetail> {
    return this.http.put<MemberDetail>(this.url() + '/group-invite/' + code + '/group/' + acceptingGroupId, null);
  }

  public getGroupInviteCodeById(id: string) {
    return this.http.get(this.url() + '/group/' + id + '/group-invite', { responseType: 'text' });
  }

  public updateGroupInviteCodeById(id: string){
    return this.http.put(this.url() + '/group/' + id + '/group-invite', null, { responseType: 'text' });
  }

  public deleteGroupInviteCodeById(id: string): Observable<boolean> {
    return this.http.delete<boolean>(this.url() + '/group/' + id + '/group-invite');
  }
}

