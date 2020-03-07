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
import { ClientEventCreate, ClientEventSummary, ConfigurationItem, ContentEventCreate, ContentEventSummary, DataFilter, PagedResult, PageViewMetric, UserEventCreate, UserEventSummary, AnalyticsEventSummary } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class AnalyticsService {
  constructor(private http: HttpClient, private settingsSvc: SettingsService, private auth: AuthService) { }

  url() {
    return this.settingsSvc.settings.clientSettings.urls.analyticsUrl;
  }

  public getAll(): Observable<AnalyticsEventSummary[]> {
    return this.http.get<AnalyticsEventSummary[]>(this.url() + '/events');
  }

  public addClientEvent(model: ClientEventCreate) {
    return this.http.post(this.url() + '/events/client', model);
  }

  public getAllClientEvents(search: DataFilter): Observable<PagedResult<ClientEventSummary>> {
    return this.http.get<PagedResult<ClientEventSummary>>(this.url() + '/events/client' + this.auth.queryStringify(search, '?'));
  }

  public addUserEvent(model: UserEventCreate) {
    return this.http.post(this.url() + '/events/user', model);
  }

  public getAllUserEvents(search: DataFilter): Observable<PagedResult<UserEventSummary>> {
    return this.http.get<PagedResult<UserEventSummary>>(this.url() + '/events/user' + this.auth.queryStringify(search, '?'));
  }

  public addContentEvent(model: ContentEventCreate) {
    return this.http.post(this.url() + '/events/content', model);
  }

  public getAllContentEvents(search: DataFilter): Observable<PagedResult<ContentEventSummary>> {
    return this.http.get<PagedResult<ContentEventSummary>>(this.url() + '/events/content' + this.auth.queryStringify(search, '?'));
  }

  public metric(url: string): Observable<PageViewMetric> {
    return this.http.get<PageViewMetric>(this.url() + '/events/client/page-views-metric?url=' + url);
  }

  public exerciseLeaderboard(exerciseGuid: string, maxRecordCount: number): Observable<any> {
    return this.http.get<any>(this.url() + '/integrations/pctc/exercise/' + exerciseGuid + '/leaderboards/' + maxRecordCount);
  }

  public status(): Observable<any> {
    return this.http.get(this.url() + '/status');
  }

  public configuration(): Observable<Array<ConfigurationItem>> {
    return this.http.get<Array<ConfigurationItem>>(this.url() + '/configuration');
  }
}

