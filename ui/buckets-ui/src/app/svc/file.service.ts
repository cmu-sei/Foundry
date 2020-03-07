/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient, HttpEventType, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { DataFilter, FileSummary, FileUpdate, PagedResult, ImportFileSummary, ImportFileUpdate } from '../model';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';
import { FileUploadComponent } from '../ui/file-upload/file-upload.component';

@Injectable({
    providedIn: 'root'
})
export class FileService {
    private fileUpdateSource = new Subject<boolean>();
    filesUpdated$ = this.fileUpdateSource.asObservable();

    constructor(
        private http: HttpClient,
        private settingsSvc: SettingsService,
        private auth: AuthService
    ) { }

    updateFiles() {
        this.fileUpdateSource.next(true);
    }

    public url() {
        return this.settingsSvc.settings.urls.apiUrl;
    }

    public load(id: number): Observable<any> {
        return this.http.get<any>(this.url() + '/file/' + id);
    }

    public delete(file: FileSummary): Observable<any> {
        return this.http.delete(this.url() + '/file/' + file.id);
    }

    public download(file: FileSummary): Observable<HttpResponse<Blob>> {
        var url = this.url().replace("/api", "") + file.urlWithExtension;
        return this.http.get(url, { responseType: 'blob', observe: 'response' });
    }

    public update(file: FileUpdate): Observable<any> {
        return this.http.put(this.url() + '/file/' + file.id, file);
    }

    public import(file: ImportFileUpdate): Observable<any> {
        var files = [];
        files.push(file);
        return this.http.post(this.url() + '/import/files', files);
    }

    public setTags(id: number, tags: Array<string>): Observable<any> {
        return this.http.post(this.url() + '/file/' + id + '/tags', tags);
    }

    public list(search: DataFilter): Observable<PagedResult<FileSummary>> {
        return this.http.get<PagedResult<any>>(this.url() + '/files' + this.auth.queryStringify(search, '?'));
    }

    public listForImport(search: DataFilter): Observable<PagedResult<ImportFileSummary>> {
        return this.http.get<PagedResult<ImportFileSummary>>(this.url() + '/import/files' + this.auth.queryStringify(search, '?'));
    }

    // upload files to default bucket and return progress
    public uploadToDefault(files: Set<File>, fileUploadComponent: FileUploadComponent, onError?: Function): { [key: string]: Observable<any> } {
        const status = {};

        files.forEach(file => {
            const formData = new FormData();
            formData.append('files', file);
            const req = new HttpRequest('POST', this.url() + '/upload', formData, {
                reportProgress: true
            });

            const progress = new Subject<number>();

            this.http.request(req).subscribe(event => {
                if (event.type === HttpEventType.UploadProgress) {
                    const percentDone = Math.round((100 * event.loaded) / event.total);
                    progress.next(percentDone);
                } else if (event instanceof HttpResponse) {
                    progress.complete();
                }
            },
            err => {
                if (onError) {
                    onError(fileUploadComponent, file);
                };
            },
            () => { });

            status[file.name] = {
                progress: progress.asObservable()
            };
        });

        return status;
    }

    public toFileSize(size: number): string {
        if (isNaN(size))
            size = 0;

        if (size < 1024)
            return size + ' B';

        size /= 1024;

        if (size < 1024)
            return size.toFixed(2) + ' KB';

        size /= 1024;

        if (size < 1024)
            return size.toFixed(2) + ' MB';

        size /= 1024;

        if (size < 1024)
            return size.toFixed(2) + ' GB';

        size /= 1024;

        return size.toFixed(2) + ' TB';
    }


}

