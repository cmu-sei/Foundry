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
import { BucketCreate, BucketDetail, BucketSummary, BucketUpdate, DataFilter, PagedResult, FileSummary, BucketAccountCreate } from '../model';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';
import { FileUploadComponent } from '../ui/file-upload/file-upload.component';

@Injectable({
    providedIn: 'root'
})
export class BucketService {
    private bucketUpdateSource = new Subject<boolean>();
    bucketsUpdated$ = this.bucketUpdateSource.asObservable();

    constructor(
        private http: HttpClient,
        private settingsSvc: SettingsService,
        private auth: AuthService
    ) { }

    updateBuckets() {
        this.bucketUpdateSource.next(true);
    }

    url() {
        return this.settingsSvc.settings.urls.apiUrl;
    }

    public load(id: number): Observable<BucketDetail> {
        return this.http.get<BucketDetail>(this.url() + '/bucket/' + id);
    }

    public list(search: DataFilter): Observable<PagedResult<BucketSummary>> {
        return this.http.get<PagedResult<BucketSummary>>(this.url() + '/buckets' + this.auth.queryStringify(search, '?'));
    }

    public listFiles(search: DataFilter, id: number): Observable<PagedResult<FileSummary>> {
        return this.http.get<any>(this.url() + '/bucket/' + id + '/files' + this.auth.queryStringify(search, '?'));
    }

    public add(model: BucketCreate): Observable<BucketDetail> {
        return this.http.post<BucketDetail>(this.url() + '/buckets', model);
    }

    public addAccount(bucketId: number, model: BucketAccountCreate): Observable<BucketDetail> {
        return this.http.post<BucketDetail>(this.url() + '/bucket/' + bucketId + '/accounts', model);
    }

    public update(model: BucketUpdate): Observable<BucketDetail> {
        return this.http.put<BucketDetail>(this.url() + '/bucket/' + model.id, model);
    }

    public setDefault(id: number): Observable<BucketDetail> {
        return this.http.put(this.url() + '/bucket/' + id + '/default', null);
    }

    // public uploadToBucket(file: File, id: number): Observable<any> {
    //   const payload: FormData = new FormData();
    //   payload.append('files', file, name);
    //   return this.http.post<any>(this.url() + '/bucket/' + id + '/upload', payload);
    // }

    public uploadToBucket(files: Set<File>, id: number, fileUploadComponent: FileUploadComponent, onError?: Function): { [key: string]: Observable<any> } {
        const status = {};

        files.forEach(file => {
            const formData = new FormData();
            formData.append('files', file);
            const req = new HttpRequest('POST', this.url() + '/bucket/' + id + '/upload', formData, {
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
}

