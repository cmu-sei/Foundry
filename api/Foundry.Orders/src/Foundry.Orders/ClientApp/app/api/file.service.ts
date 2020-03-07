/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/


import { Injectable } from "@angular/core";
import { HttpClient, HttpEvent, HttpRequest } from "@angular/common/http";
import { Observable } from 'rxjs/Observable';
import { ApiSettings } from "./api-settings";
import { GeneratedFileService } from "./gen/file.service";
import { DataFilter,FileSummary,IDataFilterFile,PagedResultFileFileSummary } from "./gen/models";
import { SettingsService } from "../svc/settings.service";

@Injectable()
export class FileService extends GeneratedFileService {

    constructor(
       protected http: HttpClient,
       protected api: ApiSettings,
       protected settings : SettingsService
    ) { super(http, api); }

    public upload(orderId: number, file: File): Observable<HttpEvent<string>> {
        let payload: FormData = new FormData();
        payload.append('files', file, encodeURI(file.name));
        let url: string = this.settings.fileStorage.fileStorageUrl + '/api/upload';

        return this.http.request<string>(
            new HttpRequest('POST', url, payload, { reportProgress: true })
        );
    }
}
