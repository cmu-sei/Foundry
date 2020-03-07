/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Headers, RequestOptions, ResponseContentType } from '@angular/http';
import { Router } from '@angular/router';
import { forkJoin, Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
// tslint:disable-next-line:max-line-length
import { ContentCreate, ContentDetail, ContentDifficulty, ContentSummary, ContentUpdate, DataFilter, Difficulty, DiscussionDetailComment, PagedResult, Rating, RatingMetricDetail, Tag } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class ContentService {

  constructor(
    private http: HttpClient,
    private router: Router,
    private settingsSvc: SettingsService,
    private auth: AuthService
  ) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  public load(id: number): Observable<ContentDetail> {
    return this.http.get<ContentDetail>(this.url() + '/content/' + id);
  }

  public list(search: any): Observable<PagedResult<ContentSummary>> {
    return this.http.get<PagedResult<ContentSummary>>(this.url() + '/contents' + this.auth.queryStringify(search, '?'));
  }

  public listFiltered(filter: string, search: DataFilter): Observable<PagedResult<ContentSummary>> {
    return this.http.get<PagedResult<ContentSummary>>(this.url() + filter + '/contents' + this.auth.queryStringify(search, '?'));
  }

  public getFeaturedContent(search: DataFilter): Observable<any> {

    const contentResponse = this.http.get(this.url() + '/contents' + this.auth.queryStringify(search, '?')).map(res => res);
    const playlistResponse = this.http.get(this.url() + '/playlists' + this.auth.queryStringify(search, '?')).map(res => res);

    return forkJoin([contentResponse, playlistResponse]);

  }

  public dashboard(search: any): Observable<PagedResult<ContentSummary>> {
    return this.http.get<PagedResult<ContentSummary>>(this.url() + '/dashboard' + this.auth.queryStringify(search, '?'));
  }

  public export(model: any) {

    const accept = model.type === 'csv' ? 'application/octet-stream' : 'application/zip';

    const headers = new Headers({ 'Accept': accept });
    const options = new RequestOptions({ headers: headers, responseType: ResponseContentType.Blob });

    return this.auth.postClean(this.url() + '/contents/export', model, options);
  }

  public import(files: any) {
    if (files.length === 0) {
      return;
    }

    return this.auth.request('POST', this.url() + '/contents/import', files);
  }

  public disable(ids: number[]): Observable<any> {
    return this.http.put(this.url() + '/contents/disable', ids);
  }

  public enable(ids: number[]): Observable<any> {
    return this.http.put(this.url() + '/contents/enable', ids);
  }

  public dashboardItems(search: any): Observable<any> {
    return this.http.get(this.url() + '/dashboard/tags');
  }

  public contentTypes(): Observable<any> {
    return this.http.get(this.url() + '/contenttypes');
  }

  public dashboardValues() {
    return this.http.get(this.url() + '/dashboard/values');
  }

  public update(content: ContentUpdate): Observable<any> {
    return this.http.put(this.url() + '/content/' + content.id, content);
  }

  public add(content: ContentCreate): Observable<any> {
    return this.http.post(this.url() + '/contents', content);
  }

  public comments(id: number, search: DataFilter): Observable<PagedResult<DiscussionDetailComment>> {
    return this.http.get<PagedResult<DiscussionDetailComment>>(this.url() + '/content/' + id + '/comments' + this.auth.queryStringify(search, '?'));
  }

  public comment(id: number, text: string): Observable<DiscussionDetailComment> {
    return this.http.post(this.url() + '/content/' + id + '/review', text);
  }

  public delete(id: number) {
    return this.http.delete(this.url() + '/content/' + id);
  }

  public addBookmark(id: number) {
    return this.http.post(this.url() + '/content/' + id + '/bookmark', null);
  }

  public removeBookmark(id: number) {
    return this.http.delete(this.url() + '/content/' + id + '/bookmark');
  }

  public addTags(tags: any): Observable<Tag> {
    return this.http.post(this.url() + '/tags', tags);
  }

  public listTags(search: any): Observable<PagedResult<Tag>> {
    return this.http.get<PagedResult<Tag>>(this.url() + '/tags' + this.auth.queryStringify(search, '?'));
  }

  public getContentDiscussion(id: number) {
    return this.http.get(this.url() + '/discussion/' + id + '/comments');
  }

  public addContentSummary(model: ContentCreate): Observable<ContentSummary> {
    const data = { id: model.id, name: model.name, description: model.description };
    return this.http.post(this.url() + '/contents/summary', data);
  }

  public saveContentSummary(model: ContentUpdate): Observable<ContentSummary> {
    const data = { id: model.id, name: model.name, description: model.description };
    return this.http.put(this.url() + '/content/' + model.id + '/summary', data);
  }

  public saveRating(contentId: number, rating: Rating): Observable<RatingMetricDetail> {
    return this.http.post(this.url() + '/content/' + contentId + '/rate/' + rating, null);
  }

  public saveDifficulty(contentId: number, difficulty: Difficulty): Observable<ContentDifficulty> {
    return this.http.post(this.url() + '/content/' + contentId + '/difficulty/' + difficulty, null);
  }

  public addToPlaylist(contentId: number, id: number): Observable<boolean> {
    return this.http.post<boolean>(this.url() + '/playlist/' + id + '/content/' + contentId, { contentId: contentId, id: id });
  }

  public removeFromPlaylist(contentId: number, id: number): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Authorization': this.auth.getAuthorizationHeader() }),
      body: { contentId: contentId, id: id }
    };
    return this.http.delete<boolean>(this.url() + '/playlist/' + id + '/content/' + contentId, httpOptions);
  }

  public launch(launchUrl: string): Observable<any> {
    return this.http.get<any>(this.url() + '/' + launchUrl);
  }

  public addFlag(id: number, comment: any) {
    return this.http.post(this.url() + '/content/' + id + '/flag', comment, { responseType: 'json' });
  }

  public removeFlag(id: number) {
    return this.http.delete(this.url() + '/content/' + id + '/flag');
  }

  public acceptFlag(id: number, profileId: number) {
    return this.http.post(this.url() + '/content/' + id + '/flag/' + profileId, null);
  }

  public rejectFlag(id: number, profileId: number) {
    return this.http.delete(this.url() + '/content/' + id + '/flag/' + profileId);
  }
}

