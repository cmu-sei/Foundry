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
import { ConfigurationItem, GroupCreate, GroupMemberCreate, GroupSummary, GroupUpdate, MemberRequestDetail, PagedResult } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class GroupService {

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService
  ) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.groupsUrl;
  }

  public configuration(): Observable<Array<ConfigurationItem>> {
    return this.http.get<Array<ConfigurationItem>>(this.url() + '/configuration');
  }

  public status(): Observable<any> {
    return this.http.get(this.url() + '/status');
  }

  public load(id: string): Observable<GroupSummary> {
    return this.http.get<GroupSummary>(this.url() + '/group/' + id);
  }

  public list(search): Observable<PagedResult<GroupSummary>> {
    return this.http.get<PagedResult<GroupSummary>>(this.url() + '/groups' + this.auth.queryStringify(search, '?'));
  }

  public getAllByProfileWithOwner(search) {
    return this.http.get(this.url() + '/profile/me/groups' + this.auth.queryStringify(search, "?"));
  }


  public updateSponsor(id: number, ids: number[]): Observable<boolean> {
    return this.http.put<boolean>(this.url() + '/sponsor/' + id + '/contents', ids);
  }

  public add(group: GroupCreate) {
    return this.http.post(this.url() + '/groups', group);
  }

  public update(group: GroupUpdate): Observable<GroupUpdate> {
    return this.http.put<GroupUpdate>(this.url() + '/group/' + group.id, group);
  }

  public delete(id: any) {
    return this.http.delete(this.url() + '/group/' + id);
  }

  public join(groupId: any): Observable<GroupSummary> {
    return this.http.post<GroupSummary>(this.url() + '/group/' + groupId + '/join', null);
  }

  public members(id: string, search): Observable<any> {
    return this.http.get<any>(this.url() + '/group/' + id + '/members' + this.auth.queryStringify(search, '?'));
  }

  public addMember(groupId: string, model: GroupMemberCreate): Observable<GroupMemberCreate> {
    return this.http.put<GroupMemberCreate>(this.url() + '/group/' + groupId + '/members', model);
  }

  public updateMember(groupId: string, accountId: string, model: GroupMemberCreate): Observable<GroupMemberCreate> {
    return this.http.put<GroupMemberCreate>(this.url() + '/group/' + groupId + '/member/' + accountId, model);
  }

  public addMembersToGroups(groupIds: number[], profileIds: number[]): Observable<boolean> {
    return this.http.post<boolean>(this.url() + '/group/members', { groupIds: groupIds, profileIds: profileIds });
  }

  public leave(groupId: any): Observable<boolean> {
    return this.http.post<boolean>(this.url() + '/group/' + groupId + '/leave', null);
  }

  public admit(gid: any, mid: any): Observable<boolean> {
    return this.http.post<boolean>(this.url() + `/group/${gid}/admit/${mid}`, {});
  }

  public removeMember(groupId: string, accountId: string) {
    return this.http.delete(this.url() + '/group/' + groupId + '/member/' + accountId);
  }

  public removeMemberRequest(groupId: string, accountId: string) {
    return this.http.delete(this.url() + '/group/' + groupId + '/member-request/' + accountId);
  }

  public promote(gid: number, mid: number): Observable<boolean> {
    return this.http.get<boolean>(this.url() + `/group/${gid}/promote/${mid}`);
  }

  public demote(gid: number, mid: number): Observable<boolean> {
    return this.http.get<boolean>(this.url() + `/group/${gid}/demote/${mid}`);
  }

  public listRequestsByGroup(id: string, search: any) {
    return this.http.get<PagedResult<MemberRequestDetail>>(this.url() + '/group/' + id + '/member-requests' +
      this.auth.queryStringify(search, '?'));
  }

  public addRequest(id: string, model: any) {
    return this.http.post(this.url() + '/group/' + id + '/member-requests', model);
  }

  public updateRequest(id: string, accountId: string, model: any) {
    return this.http.put(this.url() + '/group/' + id + '/member-request/' + accountId, model);
  }

    /* member invite codes */
    public getByMemberInviteCode(code: string): Observable<any> {
      return this.http.get<any>(this.url() + '/member-invite/' + code);
    }

    public acceptMemberInvite(code: string): Observable<any> {
      return this.http.put<any>(this.url() + '/member-invite/' + code, null);
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
    public getByGroupInviteCode(code: string): Observable<any> {
      return this.http.get<any>(this.url() + '/group-invite/' + code);
    }

    public acceptGroupInvite(code: string, acceptingGroupId: string): Observable<any> {
      return this.http.put<any>(this.url() + '/group-invite/' + code + '/group/' + acceptingGroupId, null);
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

