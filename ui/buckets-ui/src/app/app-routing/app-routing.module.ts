/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from '../svc/auth-guard.service';
import { AuthFailedComponent } from '../ui/auth/auth-failed.component';
import { AuthComponent } from '../ui/auth/auth.component';
import { BucketBrowserComponent } from '../ui/bucket-browser/bucket-browser.component';
import { BucketDetailComponent } from '../ui/bucket-detail/bucket-detail.component';
import { BucketManageComponent } from '../ui/bucket-manage/bucket-manage.component';
import { BucketComponent } from '../ui/bucket/bucket.component';
import { DefaultBucketSelectComponent } from '../ui/default-bucket-select/default-bucket-select.component';
import { EditComponent } from '../ui/edit/edit.component';
import { FileBrowserComponent } from '../ui/file-browser/file-browser.component';
import { FileUploadComponent } from '../ui/file-upload/file-upload.component';
import { FileComponent } from '../ui/file/file.component';
import { LandingComponent } from '../ui/landing/landing.component';
import { PageNotFoundComponent } from '../ui/page-not-found/page-not-found.component';
import { TagBrowserComponent } from '../ui/tag-browser/tag-browser.component';
import { ImportComponent } from '../ui/import/import.component';
import { AccountBrowserComponent } from '../ui/account-browser/account-browser.component';
import { AccountDetailComponent } from '../ui/account-detail/account-detail.component';
import { AccountComponent } from '../ui/account/account.component';
import { AccountEditComponent } from '../ui/account-edit/account-edit.component';

const routes: Routes = [
    { path: '', component: LandingComponent, pathMatch: 'full' },
    {
        path: 'file', canActivate: [AuthGuardService], component: FileComponent,
        children: [{
            path: '',
            children: [
                { path: 'add', component: FileUploadComponent },
                { path: '', component: FileBrowserComponent },
            ]
        }]
    },
    {
        path: 'bucket', canActivate: [AuthGuardService], component: BucketComponent,
        children: [{
            path: '',
            children: [
                { path: 'add', component: EditComponent },
                { path: 'manage', component: BucketManageComponent },
                { path: 'default-select', component: DefaultBucketSelectComponent },
                { path: 'edit/:id', component: EditComponent },
                { path: ':id/:slug', component: BucketDetailComponent },
                { path: '', component: BucketBrowserComponent },
            ]
        }]
    },
    { path: 'import', canActivate: [AuthGuardService], component: ImportComponent },
    {
        path: 'account', canActivate: [AuthGuardService], component: AccountComponent,
        children: [{
            path: '',
            children: [
                { path: ':id', component: AccountDetailComponent },
                { path: '', component: AccountBrowserComponent },
            ]
        }]
    },
    { path: 'tag', canActivate: [AuthGuardService], component: TagBrowserComponent },
    { path: 'account-edit', canActivate: [AuthGuardService], component: AccountEditComponent },
    { path: 'auth', component: AuthComponent },
    { path: 'failed', component: AuthFailedComponent },
    { path: '**', component: PageNotFoundComponent }
];

@NgModule({
    imports: [
        CommonModule,
        RouterModule.forRoot(routes)
    ],
    declarations: [],
    exports: [RouterModule]
})
export class AppRoutingModule { }

