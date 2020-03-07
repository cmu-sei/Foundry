/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClientModule } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
// import { Ng2BreadcrumbModule } from 'ng2-breadcrumb/ng2-breadcrumb';
import { BreadcrumbModule } from 'angular-crumbs';
import { DragulaModule } from 'ng2-dragula';
import { LazyLoadImagesModule } from 'ngx-lazy-load-images';
import { AnalyticsService } from './analytics/analytics.service';
import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from './auth/auth.module';
import { SystemNotificationDialog } from './dashboard/components/browser/system.notification.component';
import { DocumentModule } from './document/document.module';
import { ExtensionModule } from './extension/extension.module';
import { NotificationModule } from './notification/notification.module';
import { PostsModule } from './posts/posts.module';
import { ProfileModule } from './profile/profile.module';
import { RequestService } from './request/request.service';
import { AboutComponent } from './root/about.component';
// { APP_BASE_HREF } from '@angular/common';
import { AppComponent } from './root/app.component';
import { BrowseListComponent } from './root/browse-menu/browse-menu.component';
import { HelpComponent } from './root/help/help.component';
import { HomeComponent } from './root/home.component';
import { LicensesComponent } from './root/licenses/licenses.component';
import { MessageService } from './root/message.service';
import { OidcSilentComponent } from './root/oidc-silent/oidc-silent.component';
import { getOriginUrl, getShowdownOpts, ORIGIN_URL, SettingsService, SHOWDOWN_OPTS } from './root/settings.service';
import { SimsComponent } from './root/sims/sims.component';
import { SettingsModule } from './settings/settings.module';
import { ConfirmDialogComponent } from './shared/components/confirm-dialog/confirm-dialog.component';
import { SharedModule } from './shared/shared.module';
import { TagModule } from './tag/tag.module';
import { NavbarComponent } from './navbar/navbar.component';
import { PageViewMetricsComponent } from './navbar/page-view-metrics.component';

@NgModule({
  bootstrap: [AppComponent],
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
    SystemNotificationDialog,
    BrowseListComponent,
    SimsComponent,
    LicensesComponent,
    HelpComponent,
    OidcSilentComponent,
    NavbarComponent,
    PageViewMetricsComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    AuthModule,
    SharedModule,
    ProfileModule,
    ExtensionModule,
    SettingsModule,
    PostsModule,
    NotificationModule,
    TagModule,
    DocumentModule,
    BreadcrumbModule,
    LazyLoadImagesModule,
    AppRoutingModule,
    DragulaModule,
    NgbModule.forRoot()
  ],
  exports: [
    SystemNotificationDialog
  ],
  entryComponents: [SystemNotificationDialog, ConfirmDialogComponent],
  providers: [
    SettingsService,
    MessageService,
    {
      provide: APP_INITIALIZER,
      useFactory: initSettings,
      deps: [SettingsService],
      multi: true
    },
    {
      provide: ORIGIN_URL,
      useFactory: (getOriginUrl)
    },
    {
      provide: SHOWDOWN_OPTS,
      useFactory: (getShowdownOpts)
    },
    AnalyticsService,
    RequestService
  ]
})
export class AppModule {
}

export function initSettings(settings: SettingsService) {
  return () => settings.load();
}

