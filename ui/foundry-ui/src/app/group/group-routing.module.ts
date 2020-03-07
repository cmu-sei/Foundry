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
import { RouterModule } from '@angular/router';
import { AuthGuardService } from '../auth/auth-guard.service';
import { GroupBrowserComponent } from './components/browser/browser.component';
import { GroupDetailComponent } from './components/detail/detail.component';
import { GroupEditComponent } from './components/edit/edit.component';
import { GroupComponent } from './group.component';

@NgModule({
  imports: [RouterModule.forChild([
    {
      path: '',
      component: GroupComponent,
      canActivate: [AuthGuardService],
      children: [
        {
          path: '',
          children: [
            { path: 'add', component: GroupEditComponent, data: { breadcrumb: 'Add Group' } },
            { path: 'edit/:id', component: GroupEditComponent, data: { breadcrumb: 'Edit Group' } },
            { path: ':id/:slug', component: GroupDetailComponent, data: { breadcrumb: 'Detail' } },
            { path: '', component: GroupBrowserComponent }
          ]
        }
      ]
    }
  ])],
  exports: [RouterModule]
})
export class GroupRoutingModule {}

