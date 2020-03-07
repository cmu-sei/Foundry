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
import { ContentBrowserComponent } from './components/browser/browser.component';
import { ContentDetailComponent } from './components/detail/detail.component';
import { ContentEditComponent } from './components/edit/edit.component';
import { ContentManageComponent } from './components/manage/manage.component';
import { ContentComponent } from './content.component';

const routes: Routes = [
  {
    path: '',
    component: ContentComponent,
    children: [
      {
        path: '',
        children: [
          { path: 'add', component: ContentEditComponent, data: { breadcrumb: 'Add Content' } },
          { path: 'manage', component: ContentManageComponent, data: { breadcrumb: 'Manage Content' } },
          { path: 'edit/:id', component: ContentEditComponent, data: { breadcrumb: 'Edit Content' } },
          { path: ':id/:slug', component: ContentDetailComponent, data: { breadcrumb: 'Detail' } },
          { path: '', component: ContentBrowserComponent },
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ContentRoutingModule {

}

