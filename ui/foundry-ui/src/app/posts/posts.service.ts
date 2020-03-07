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
import { PagedResult, PostCreate, PostUpdate, PostDetail, DataFilter, PostVoteMetric } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class PostsService {
  constructor(private http: HttpClient, private auth: AuthService, private settingsSvc: SettingsService) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  public list(id: number, search: DataFilter): Observable<PagedResult<PostDetail>> {
    return this.http.get<PagedResult<PostDetail>>(this.url() + '/profile/' + id + '/posts' + this.auth.queryStringify(search, '?'));
  }

  public listReplies(id: number, search: DataFilter): Observable<PagedResult<PostDetail>> {
    return this.http.get<PagedResult<PostDetail>>(this.url() + '/post/' + id + '/replies' + this.auth.queryStringify(search, '?'));
  }

  public update(id: number, post: PostUpdate): Observable<PostDetail> {
    return this.http.put<PostDetail>(this.url() + '/post/' + id, post);
  }

  public add(post: PostCreate): Observable<PostDetail> {
    return this.http.post<PostDetail>(this.url() + '/posts', post);
  }

  public upVote(id: number): Observable<PostVoteMetric> {
    return this.http.post<PostVoteMetric>(this.url() + '/post/' + id + '/up', null);
  }

  public downVote(id: number): Observable<PostVoteMetric> {
    return this.http.post<PostVoteMetric>(this.url() + '/post/' + id + '/down', null);
  }

  public delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(this.url() + '/post/' + id);
  }
}

