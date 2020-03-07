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
import { iif, Observable, of, Subject } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
// tslint:disable-next-line:max-line-length
import { Achievement, ContentSummary, DataFilter, GroupSummary, KeyValue, PagedResult, PlaylistDetail, PlaylistSummary, PlaylistUpdate, ProfileDetail, ProfileInfo, ProfileSummary } from '../core-api-models';
import { MessageService } from '../root/message.service';
import { SettingsService } from '../root/settings.service';
import { EntityCache } from './profile.cache';

@Injectable()
export class ProfileService {
  profile: ProfileDetail = null;
  profileSource: Subject<ProfileDetail> = new Subject<ProfileDetail>();
  profile$: Observable<ProfileDetail> = this.profileSource.asObservable();
  username = '';
  globalId = '';
  defaultImageUri = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH4wMBFA4JX2hAvwAAAB1pVFh0Q29tbWVudAAAAAAAQ3JlYXRlZCB3aXRoIEdJTVBkLmUHAAAAF0lEQVQY02P8//8/AzGAiYFIMKqQOgoB1RIDEeZCrrsAAAAASUVORK5CYII=';

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService,
    private msgService: MessageService,
    private profileCache: EntityCache
  ) {
    this.auth.user$
      .subscribe(u => {
        if (u) {
          this.username = u.profile.name;
          this.globalId = u.profile.sub;
          this.loadProfile();
        } else {
          this.username = '';
          this.profile = null;
          this.profileSource.next(this.profile);
        }
      });
  }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  groupUrl() {
    return this.settingsSvc.settings.clientSettings.urls.groupsUrl;
  }

  public loadProfile(): void {
    this.loadByGlobalId(this.globalId).subscribe((data) => {
      this.profile = data;
      this.profile.name = this.username;
      this.update();
    });
  }

  public getProfileInfo(globalId: string): Observable<ProfileInfo> {
    const profile = this.profileCache.get(globalId);
    return iif(
      () => !!profile,
      of(profile as ProfileInfo),
      this.http
        .get<ProfileInfo>(this.settingsSvc.settings.clientSettings.urls.profileQueryUrl + '/' + globalId)
        .pipe(
          tap(info => this.profileCache.set(globalId, info))
        )
    );
  }

  public getProfileUrl() {
    return this.settingsSvc.settings.clientSettings.urls.profileEditUrl;
  }

  public loadById(id: number): Observable<any> {
    return this.http.get<any>(this.url() + '/profile/' + id);
  }

  public loadByGlobalId(globalId: string): Observable<any> {
    return this.http.get<any>(this.url() + '/profile/' + globalId);
  }

  public list(search: DataFilter): Observable<PagedResult<ProfileSummary>> {
    return this.http.get<PagedResult<ProfileSummary>>(this.url() + '/profiles' + this.auth.queryStringify(search, '?'));
  }

  public testAddAchievement(ach: Achievement): Observable<Achievement> {
    return this.http.post<Achievement>(this.url() + '/achievements', ach);
  }

  public toggleAdministrator(id: number): Observable<ProfileSummary> {
    return this.http.put<any>(this.url() + '/profile/' + id + '/permissions/administrator', null);
  }

  public togglePowerUser(id: number): Observable<ProfileSummary> {
    return this.http.put<any>(this.url() + '/profile/' + id + '/permissions/poweruser', null);
  }

  public setDisabled(id: number, isDisabled: boolean): Observable<ProfileSummary> {
    if (isDisabled)
      return this.http.delete<any>(this.url() + '/profile/' + id + '/disabled');

    return this.http.put<any>(this.url() + '/profile/' + id + '/disabled', null);
  }

  public achievements(id: number): Observable<PagedResult<Achievement>> {
    return this.http.get<PagedResult<Achievement>>(this.url() + '/profile/' + id + '/achievements');
  }

  public memberships(id: string, search: DataFilter): Observable<PagedResult<GroupSummary>> {
    return this.http.get<PagedResult<GroupSummary>>(this.groupUrl() + '/account/' + id  +
      '/groups'  + this.auth.queryStringify(search, '?'));
  }

  public content(id: number, search: DataFilter): Observable<PagedResult<ContentSummary>> {
    return this.http.get<PagedResult<ContentSummary>>(this.url() + '/profile/' + id + '/content' + this.auth.queryStringify(search, '?'));
  }

  public playlists(id: number, search: DataFilter): Observable<PagedResult<PlaylistSummary>> {
    return this.http.get<PagedResult<PlaylistSummary>>(this.url() + '/profile/' + id + '/playlists'
    + this.auth.queryStringify(search, '?'));
  }

  public updateAuthor(id: number, ids: number[]): Observable<boolean> {
    return this.http.put<boolean>(this.url() + '/author/' + id + '/contents', ids);
  }

  public statements(id: number): Observable<PagedResult<GroupSummary>> {
    return this.http.get<PagedResult<GroupSummary>>(this.url() + '/profile/' + id + '/xapistatements');
  }

  public loadPlaylist(id: number): Observable<PlaylistDetail> {
    return this.http.get<PlaylistDetail>(this.url() + '/playlist/' + id);
  }

  public addSetting(setting: KeyValue): Observable<ProfileDetail> {
    const id = sessionStorage.getItem('currentUserId');
    return this.http.put<ProfileDetail>(this.url() + '/profile/' + id + '/' + setting.key, setting.value);
  }

  public addPlaylist(name: string): Observable<PlaylistDetail> {
    const playlist = {
      name: name,
      isDefault: this.profile.playlists.length === 0
    };
    return this.http.post<PlaylistDetail>(this.url() + '/playlists', playlist).pipe(
      map(result => {
        this.append(this.profile.playlists, result);
        return result;
      }));
  }

  public updatePlaylist(playlist: PlaylistUpdate): Observable<PlaylistUpdate> {
    return this.http.put<PlaylistUpdate>(this.url() + '/playlist/' + playlist.id, playlist).pipe(
      map(result => {
        this.replace(this.profile.playlists, result);
        return result;
      }));
  }

  public deletePlaylist(id: number): Observable<boolean> {
    return this.http.delete<boolean>(this.url() + '/playlist/' + id).pipe(
      map(result => {
        this.removeById(this.profile.playlists, id);
        return result;
      }));
  }

  onFollowPlaylist(playlist: PlaylistUpdate) {
    this.append(this.profile.playlists, playlist);
  }

  onUpdatePlaylist(playlist: PlaylistUpdate) {
    this.replace(this.profile.playlists, playlist);
  }

  onRemovePlaylist(id: number) {
    this.removeById(this.profile.playlists, id);
  }

  onJoin(group: GroupSummary) {
    this.append(this.profile.groups, group);
  }

  onLeave(id: number) {
    this.removeById(this.profile.groups, id);
  }

  update() {
    this.profileSource.next(this.profile);
  }

  append(a, t) {
    a.push(t);
    this.update();
  }

  replace(a, t) {
    for (let i = 0; i < a.length; i++) {
      if (a[i].id === t.id) {
        a[i] = t;
        this.update();
        break;
      }
    }
  }

  removeById(a: any[], id: number) {
    for (let i = 0; i < a.length; i++) {
      if (a[i].id === id) {
        a.splice(i, 1);
        this.update();
        break;
      }
    }
  }

  getProfileViewMode(profile: ProfileDetail, key: string) : string {
    var viewMode = 'tile';
    profile.keyValues.forEach(element => { if (element.key === key) { viewMode = element.value; } });
    return viewMode;
  }

  setProfileViewMode(profile: ProfileDetail, key: string, value: string) {
    var keyValue: KeyValue = null;

    // find key in existing profile keyValue collection
    profile.keyValues.forEach(kv => { if (kv.key === key) { keyValue = kv; } });

    if (keyValue == null) {
      // viewMode not set for profile, create new and add to profile
      keyValue = { key: key };
      profile.keyValues.push(keyValue);
    }

    if (keyValue.value != value) {
      // viewMode changed, send update
      keyValue.value = value;
      this.addSetting(keyValue).subscribe(result => {
        this.msgService.addSnackBar('View preference saved.');
      });
    }
  }
}

