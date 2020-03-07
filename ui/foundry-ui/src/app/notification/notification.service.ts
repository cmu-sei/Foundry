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
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ConfigurationItem, DataFilter, NotificationSummary, NotificationSummaryValue, PagedResult } from '../core-api-models';
import { SettingsService, NotificationRewrite } from '../root/settings.service';

@Injectable()
export class NotificationService {

  public notificationHistory = new BehaviorSubject<Array<NotificationSummary>>(new Array<NotificationSummary>());
  public connection: HubConnection;

  constructor(
    private http: HttpClient,
    private auth: AuthService,
    private settingsSvc: SettingsService,
    private router: Router
  ) { }

  // url() {
  //     console.log(this.settingsSvc.settings.clientSettings.urls.communicationUrl);
  //     return this.settingsSvc.settings.clientSettings.urls.communicationUrl;
  // }

  notificationHubUrl() {
    return this.settingsSvc.settings.clientSettings.urls.communicationUrl.replace('/api', '/hubs/notifications');
  }

  communicationUrl() {
    return this.settingsSvc.settings.clientSettings.urls.communicationUrl;
  }

  coreUrl() {
    return this.settingsSvc.settings.clientSettings.urls.coreUrl;
  }

  groupNotificationRewrite(): NotificationRewrite {
    return this.settingsSvc.settings.clientSettings.notifications.group;
  }

  public list(search: DataFilter): Observable<PagedResult<NotificationSummary>> {
    return this.http.get<PagedResult<NotificationSummary>>(this.communicationUrl() + '/notifications'
      + this.auth.queryStringify(search, '?'));
  }

  public unreadCount(): Observable<number> {
    return this.http.get<number>(this.communicationUrl() + '/notifications/count/unread');
  }

  public navigate(notification: NotificationSummary) {
    let url : string = notification.url.replace(this.settingsSvc.originUrl, '');

    const groupsRewrite = this.groupNotificationRewrite();

    if (groupsRewrite) {
      url = url.replace(groupsRewrite.from, groupsRewrite.to);
    }

    if (url.indexOf('http') == 0) {
      window.open(url, "_blank");
    }
    else {
      this.router.navigateByUrl(url);
    }
  }

  public delete(id: number): Observable<boolean> {
    return this.http.delete<boolean>(this.communicationUrl() + '/notification/' + id);
  }

  public deleteAll(ids: Array<number>): Observable<boolean> {
    return this.http.request<boolean>('delete', this.communicationUrl() + '/notifications', { body: ids });
  }

  public markAsRead(id: number): Observable<boolean> {
    return this.http.put<boolean>(this.communicationUrl() + '/notification/' + id + '/read', null);
  }

  public markAsUnread(id: number): Observable<boolean> {
    return this.http.put<boolean>(this.communicationUrl() + '/notification/' + id + '/unread', null);
  }

  public notificationIcon(values: NotificationSummaryValue[]) {
    const kv = values.find(v => v.key === 'label');

    if (kv != null) {
      const label = kv.value;
      if (label === 'add') {
        return 'add_circle_outline';
      }
      if (label === 'delete') {
        return 'remove_circle_outline';
      }
      if (label === 'update') {
        return 'update';
      }
      if (label === 'system') {
        return 'report';
      }
      if (label === 'warning') {
        return 'warning';
      }
      if (label === 'post') {
        return 'message';
      }
    }

    return 'help_outline';
  }

  public add(model): Observable<Object> {
    return this.http.post(this.coreUrl() + '/system-notifications', model);
  }

  start(id: string): Promise<boolean> {
    const url = this.notificationHubUrl();
    console.log('connecting to notification hub ' + url);
    this.connection = new HubConnectionBuilder()
      .withUrl(url + '?bearer=' + this.auth.currentUser.access_token).build();

    return this.connection.start()
      .then(() => {

        this.connection.on('History', (data: [NotificationSummary]) => {
          this.notificationHistory.next(data);
        });

        this.connection.invoke('History', id);
        console.log('Notification connection started');

        return true;
      });
  }

  public status(): Observable<any> {
    return this.http.get(this.communicationUrl() + '/status');
  }

  public configuration(): Observable<Array<ConfigurationItem>> {
    return this.http.get<Array<ConfigurationItem>>(this.communicationUrl() + '/configuration');
  }
}

