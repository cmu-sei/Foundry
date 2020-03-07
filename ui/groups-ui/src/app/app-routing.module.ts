/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuardService } from './svc/auth-guard.service';
import { AuthFailedComponent } from './ui/auth/auth-failed.component';
import { AuthComponent } from './ui/auth/auth.component';
import { GroupDetailComponent } from './ui/group-detail/group-detail.component';
import { GroupEditComponent } from './ui/group-edit/group-edit.component';
import { LandingComponent } from './ui/landing/landing.component';
import { OidcSilentComponent } from './ui/oidc-silent/oidc-silent.component';
import { PageNotFoundComponent } from './ui/page-not-found/page-not-found.component';
import { MigrateComponent } from './ui/migrate/migrate.component';

const routes: Routes = [
  { path: '', component: LandingComponent, pathMatch: 'full' },
  { path: 'detail/:id/:slug', canActivate: [AuthGuardService], component: GroupDetailComponent },
  { path: 'add', canActivate: [AuthGuardService], component: GroupEditComponent },
  { path: 'edit/:id', canActivate: [AuthGuardService], component: GroupEditComponent },
  { path: 'auth', component: AuthComponent },
  { path: 'migrate', component: MigrateComponent },
  { path: 'oidc-silent', component: OidcSilentComponent },
  { path: 'failed', component: AuthFailedComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

