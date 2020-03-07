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
import { Headers, RequestOptions, ResponseContentType } from '@angular/http';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
// tslint:disable-next-line:max-line-length
import { DataFilter, PagedResult, PlaylistCreate, PlaylistDetail, PlaylistRating, PlaylistSectionUpdate, PlaylistSummary, PlaylistUpdate, Rating } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class PlaylistService {

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService
  ) { }

  url(args: any[]) {
    args.unshift(this.settingsSvc.settings.clientSettings.urls.coreUrl, 'playlist');
    return args.join('/');
  }

  completeUrl() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  public list(search: DataFilter): Observable<PagedResult<PlaylistSummary>> {
    return this.http.get<PagedResult<PlaylistSummary>>(this.completeUrl() + '/playlists' + this.auth.queryStringify(search, '?'));
  }

  public getGroupPlaylists(id: any): Observable<PagedResult<PlaylistSummary>> {
    return this.http.get<PagedResult<PlaylistSummary>>(this.completeUrl() + '/group/' + id + '/playlists');
  }

  public organize(id: number, sections: Array<PlaylistSectionUpdate>): Observable<boolean> {
    return this.http.put<boolean>(this.url([id, 'organize']), sections);
  }

  public update(playlist: PlaylistUpdate): Observable<PlaylistSummary> {
    return this.http.put<PlaylistSummary>(this.url([playlist.id]), playlist);
  }

  public contents(id: number): Observable<any> {
    return this.http.get<any>(this.completeUrl() + '/playlist/' + id + '/contents');
  }

  public load(id: number): Observable<PlaylistDetail> {
    return this.http.get<PlaylistDetail>(this.url([id]));
  }

  public follow(id: number): Observable<any> {
    return this.http.post<any>(this.completeUrl() + '/playlist/' + id + '/follow', { id: id });
  }

  public groupFollow(id: number, groupId: number): Observable<any> {
    return this.http.post<any>(this.completeUrl() + '/playlist/' + id + '/group/' + groupId + '/follow', { id: id });
  }

  public groupUnfollow(id: any, groupId: any): Observable<any> {
    return this.http.delete<any>(this.completeUrl() + '/playlist/' + id + '/group/' + groupId + '/unfollow');
  }

  public unfollow(id: number): Observable<any> {
    return this.http.delete<any>(this.completeUrl() + '/playlist/' + id + '/unfollow');
  }

  public delete(id: number): Observable<any> {
    return this.http.delete<any>(this.url([id]));
  }

  public add(playlist: PlaylistCreate): Observable<PlaylistSummary> {
    return this.http.post<PlaylistSummary>(this.completeUrl() + '/playlists', playlist);
  }

  public saveRating(playlistId: number, rating: Rating): Observable<PlaylistRating> {
    return this.http.post<PlaylistRating>(this.completeUrl() + '/playlist/' + playlistId + '/rate/' + rating, null);
  }

  public export(model: any) {

    const accept = model.type === 'csv' ? 'application/octet-stream' : 'application/zip';

    const headers = new Headers({ 'Accept': accept });
    const options = new RequestOptions({ headers: headers, responseType: ResponseContentType.Blob });

    return this.auth.postClean(this.completeUrl() + '/playlists/export', model, options);
  }
  public import(files: any) {
    if (files.length === 0) {
      return;
    }

    return this.auth.request('POST', this.completeUrl() + '/playlists/import', files);
  }
}

