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
import { Observable, Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { DataFilter, PagedResult, FileDetail } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable({
  providedIn: 'root'
})
export class ImagesService {

  private fileSource = new Subject<any>();
  private fileUrlSource = new Subject<any>();

  fileItem$ = this.fileSource.asObservable();
  fileUrlItem$ = this.fileUrlSource.asObservable();
  constructor(
    private http: HttpClient,
    private settingsSvc: SettingsService,
    private auth: AuthService
  ) { }

  updateFileSource(fileUrl) {
    this.fileSource.next(fileUrl);
  }

  updateFileUrlSource(fileUrl) {
    this.fileUrlSource.next(fileUrl);
  }

  public url () {
    return this.settingsSvc.settings.clientSettings.urls.uploadUrl;
  }

  public list(search: DataFilter): Observable<PagedResult<FileDetail>> {
    return this.http.get<PagedResult<any>>(this.url() + '/api/files' + this.auth.queryStringify(search, '?'));
  }
}

