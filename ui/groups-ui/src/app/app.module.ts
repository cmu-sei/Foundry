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
import { MatButtonModule, MatDialogModule, MatDividerModule, MatExpansionModule, MatFormFieldModule, MatIconModule, MatInputModule, MatListModule, MatMenuModule, MatPaginatorModule, MatProgressBarModule, MatProgressSpinnerModule, MatSelectModule, MatSidenavModule, MatSnackBarModule, MatStepperModule, MatTableModule, MatTabsModule, MatToolbarModule, MatTooltipModule, MatTreeModule } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthGuardService } from './svc/auth-guard.service';
import { AuthService } from './svc/auth.service';
import { AuthInterceptor } from './svc/http-auth-interceptor';
import { SettingsService } from './svc/settings.service';
import { AuthFailedComponent } from './ui/auth/auth-failed.component';
import { AuthComponent } from './ui/auth/auth.component';
import { ChildRequestsComponent } from './ui/child-requests/child-requests.component';
import { ConfirmDeleteComponent } from './ui/confirm-delete/confirm-delete.component';
import { ConfirmDialogComponent } from './ui/confirm-dialog/confirm-dialog.component';
import { GroupDetailComponent } from './ui/group-detail/group-detail.component';
import { GroupEditComponent } from './ui/group-edit/group-edit.component';
import { GroupMembersComponent } from './ui/group-members/group-members.component';
import { GroupRequestsComponent } from './ui/group-requests/group-requests.component';
import { ImageBrowserComponent } from './ui/image-browser/image-browser.component';
import { InviteInputComponent } from './ui/invite-input/invite-input.component';
import { InviteComponent } from './ui/invite/invite.component';
import { LandingComponent } from './ui/landing/landing.component';
import { MigrateComponent } from './ui/migrate/migrate.component';
import { NavbarComponent } from './ui/navbar/navbar.component';
import { OidcSilentComponent } from './ui/oidc-silent/oidc-silent.component';
import { PageNotFoundComponent } from './ui/page-not-found/page-not-found.component';
import { PagerComponent } from './ui/pager/pager';
import { SidenavChildrenComponent } from './ui/sidenav-children/sidenav-children.component';
import { SidenavComponent } from './ui/sidenav/sidenav.component';
import { TreeDialogComponent } from './ui/tree-dialog/tree-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    PageNotFoundComponent,
    AuthComponent,
    AuthFailedComponent,
    LandingComponent,
    SidenavComponent,
    GroupDetailComponent,
    GroupEditComponent,
    OidcSilentComponent,
    ImageBrowserComponent,
    SidenavChildrenComponent,
    GroupMembersComponent,
    GroupRequestsComponent,
    ConfirmDeleteComponent,
    MigrateComponent,
    PagerComponent,
    ConfirmDialogComponent,
    TreeDialogComponent,
    ChildRequestsComponent,
    InviteComponent,
    InviteInputComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatToolbarModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatButtonModule,
    MatListModule,
    HttpClientModule,
    MatDividerModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatFormFieldModule,
    MatStepperModule,
    MatSelectModule,
    MatTabsModule,
    MatTableModule,
    MatDialogModule,
    MatExpansionModule,
    MatTreeModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    MatProgressBarModule,
    MatPaginatorModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule
  ],
  entryComponents: [ImageBrowserComponent, ConfirmDialogComponent, TreeDialogComponent, InviteComponent, InviteInputComponent],
  providers: [
    AuthService,
    AuthGuardService,
    SettingsService,
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

