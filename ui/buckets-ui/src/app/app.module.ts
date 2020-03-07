/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
// tslint:disable-next-line:max-line-length
import { MatAutocompleteModule, MatButtonModule, MatButtonToggleModule, MatCardModule, MatChipsModule, MatDialogModule, MatDividerModule, MatExpansionModule, MatFormFieldModule, MatIconModule, MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatToolbarModule, MatTooltipModule, MatTreeModule, MatTabsModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing/app-routing.module';
import { AppComponent } from './app.component';
import { AuthGuardService } from './svc/auth-guard.service';
import { AuthService } from './svc/auth.service';
import { AuthInterceptor } from './svc/http-auth-interceptor';
import { SettingsService } from './svc/settings.service';
import { AccountBrowserComponent } from './ui/account-browser/account-browser.component';
import { AccountDetailComponent } from './ui/account-detail/account-detail.component';
import { AccountEditComponent } from './ui/account-edit/account-edit.component';
import { AccountComponent } from './ui/account/account.component';
import { AuthFailedComponent } from './ui/auth/auth-failed.component';
import { AuthComponent } from './ui/auth/auth.component';
import { BucketBrowserComponent } from './ui/bucket-browser/bucket-browser.component';
import { BucketDetailComponent } from './ui/bucket-detail/bucket-detail.component';
import { BucketManageComponent } from './ui/bucket-manage/bucket-manage.component';
import { BucketComponent } from './ui/bucket/bucket.component';
import { DefaultBucketSelectComponent } from './ui/default-bucket-select/default-bucket-select.component';
import { EditComponent } from './ui/edit/edit.component';
import { FileBrowserComponent } from './ui/file-browser/file-browser.component';
import { FileListComponent } from './ui/file-list/file-list.component';
import { FileUploadComponent } from './ui/file-upload/file-upload.component';
import { FileComponent } from './ui/file/file.component';
import { ImportComponent } from './ui/import/import.component';
import { LandingComponent } from './ui/landing/landing.component';
import { NavBarComponent } from './ui/navbar/nav-bar.component';
import { PageNotFoundComponent } from './ui/page-not-found/page-not-found.component';
import { PagerComponent } from './ui/pager/pager';
import { SidenavComponent } from './ui/sidenav/sidenav.component';
import { TagBrowserComponent } from './ui/tag-browser/tag-browser.component';
import { ConfirmDialogComponent } from './ui/confirm-dialog/confirm-dialog.component';
import { FileInfoDialogComponent } from './ui/file-info-dialog/file-info-dialog.component';

@NgModule({
    declarations: [
        AppComponent,
        NavBarComponent,
        EditComponent,
        LandingComponent,
        AuthComponent,
        AuthFailedComponent,
        FileBrowserComponent,
        ImportComponent,
        BucketBrowserComponent,
        BucketManageComponent,
        TagBrowserComponent,
        DefaultBucketSelectComponent,
        BucketDetailComponent,
        PageNotFoundComponent,
        FileUploadComponent,
        BucketComponent,
        FileComponent,
        AccountComponent,
        SidenavComponent,
        FileListComponent,
        PagerComponent,
        AccountBrowserComponent,
        AccountDetailComponent,
        AccountEditComponent,
        ConfirmDialogComponent,
        FileInfoDialogComponent
    ],
    imports: [
        BrowserModule,
        HttpModule,
        HttpClientModule,
        MatButtonModule,
        MatSnackBarModule,
        MatTreeModule,
        MatExpansionModule,
        MatTooltipModule,
        MatToolbarModule,
        MatIconModule,
        MatCardModule,
        MatDividerModule,
        MatListModule,
        MatDialogModule,
        MatFormFieldModule,
        MatInputModule,
        MatSlideToggleModule,
        MatMenuModule,
        MatButtonToggleModule,
        MatProgressBarModule,
        MatSelectModule,
        MatChipsModule,
        MatAutocompleteModule,
        MatSidenavModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        FormsModule,
        ReactiveFormsModule,
        MatTabsModule
    ],
    exports: [MatButtonModule, MatSnackBarModule, MatTreeModule, MatExpansionModule, MatTooltipModule, MatProgressBarModule,
        MatToolbarModule, MatIconModule, MatCardModule, MatInputModule, MatSlideToggleModule, MatSelectModule, MatButtonToggleModule,
        MatDividerModule, MatListModule, MatChipsModule, MatDialogModule, BrowserAnimationsModule, MatFormFieldModule,
        MatAutocompleteModule, MatSidenavModule, MatMenuModule, ConfirmDialogComponent, FileInfoDialogComponent],
    entryComponents: [ConfirmDialogComponent, FileInfoDialogComponent],
    providers: [
        AuthService,
        AuthGuardService,
        SettingsService,
        SidenavComponent,
        {
            provide: APP_INITIALIZER,
            useFactory: initSettings,
            deps: [SettingsService],
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        }],
    bootstrap: [AppComponent]
})
export class AppModule { }

export function initSettings(settings: SettingsService) {
    return () => settings.load();
}

