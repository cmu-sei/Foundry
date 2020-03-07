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
import { SettingsService } from '../root/settings.service';
import { DataSetResult, DataFilter, DataSet, DataColumn, DataValue, Report } from '../core-api-models';
import { Headers, RequestOptions, Response, ResponseContentType } from '@angular/http';
import { DatePipe } from '@angular/common';

@Injectable()
export class ReportsService {

  constructor(
    private http: HttpClient,
    private settingsService: SettingsService,
    private auth: AuthService,
    private datePipe: DatePipe
  ) { }

  url() {
    return this.settingsService.settings.clientSettings.urls.coreUrl;
  }

  public load(name, search): Observable<any> {
    return this.http.get<any>(this.url() + '/report/' + name + this.auth.queryStringify(search, "?"));
  }

  public list(): Observable<any> {
    return this.http.get<any>(this.url() + '/reports');
  }

  public getFileName(slug, type) {
    var ts = this.datePipe.transform(new Date(), 'yyyyMMddHHmmss');
    return slug + '_' + ts + '.' + type;
  }

  export(slug: string, dataFilter: DataFilter, type: string) {
    this.getExport(slug, type, dataFilter).subscribe(response => {
      var name = this.getFileName(slug, type);
      var t = 'text/csv';
      if (type == 'xls') t = 'application/xlsx';
      if (type == 'pdf') t = 'application/pdf';
      var blob = new Blob([response.blob()], { type: t });

      if (response.text() != null && navigator.msSaveBlob)
        return navigator.msSaveBlob(blob, name);

      var url = window.URL.createObjectURL(blob);
      var a = document.createElement("a");
      a.style.display = 'none';
      a.href = url;
      a.download = name;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove();
    }, error => { }, () => { });
  };

  public getExport(name, type, search): Observable<Response> {

    const accept = 'application/octet-stream';

    const headers = new Headers({ 'Accept': accept });
    const options = new RequestOptions({ headers: headers, responseType: ResponseContentType.Blob });

    return this.auth.postClean(this.url() + '/report/' + name + '/export/' + type, search, options);
  }
}

