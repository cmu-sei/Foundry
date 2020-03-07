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
import { AuthGuardService } from './auth/auth-guard.service';
import { AboutComponent } from './root/about.component';
import { HelpComponent } from './root/help/help.component';
import { HomeComponent } from './root/home.component';
import { LicensesComponent } from './root/licenses/licenses.component';
import { OidcSilentComponent } from './root/oidc-silent/oidc-silent.component';
import { SimsComponent } from './root/sims/sims.component';
import { PageNotFoundComponent } from './shared/components/page-not-found/page-not-found.component';

const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  {
    path: 'content',
    loadChildren: './content/content.module#ContentModule',
    canActivate: [AuthGuardService],
    data: { breadcrumb: 'Browse Content' }
   },
  {
    path: 'playlist',
    loadChildren: './playlist/playlist.module#PlaylistModule',
    canActivate: [AuthGuardService],
    data: { breadcrumb: 'Browse Playlists' }
  },
  {
    path: 'group',
    loadChildren: './group/group.module#GroupModule',
    canActivate: [AuthGuardService],
    data: { breadcrumb: 'Browse Groups'}
   },
  { path: 'dashboard', loadChildren: './dashboard/dashboard.module#DashboardModule', canActivate: [AuthGuardService] },
  {
    path: 'calendar',
    loadChildren: './calendar/calendar.module#CalendarViewModule',
    canActivate: [AuthGuardService],
    data: { breadcrumb: 'Upcoming Events'}
   },
   {
    path: 'reports',
    loadChildren: './reports/reports.module#ReportsModule',
    canActivate: [AuthGuardService]
   },
  {
    path: 'leaderboard',
    loadChildren: './leaderboard/leaderboard.module#LeaderboardModule',
    canActivate: [AuthGuardService],
    data: { breadcrumb: 'Leaderboards'},
  },
  { path: 'about', component: AboutComponent },
  { path: 'sims', component: SimsComponent },
  { path: 'help', component: HelpComponent},
  { path: 'licenses', component: LicensesComponent },
  { path: 'oidc-silent', component: OidcSilentComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      scrollPositionRestoration: 'enabled'
    })
  ],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }

