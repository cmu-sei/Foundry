/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Routes, RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { OrderComponent } from './order.component';
import { ListComponent } from './list.component';
import { DetailComponent } from './detail.component';
import { AuthGuard } from '../../svc/auth-guard.service';
import { EditComponent } from './edit.component';
import { PeopleComponent } from './people.component';
import { TrainingContentComponent } from './trainingcontent.component';
import { ScheduleEventComponent } from './scheduleevent.component';
import { OrderReviewComponent } from './orderreview.component';
import { TerrainComponent } from './terrain.component';
import { PdfComponent } from './pdf.component';
import { ScenarioComponent } from './scenario.component';
import { HelpComponent } from './help.component';

const routes: Routes = [
    {
        path: 'order',
        component: OrderComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: '',
                children: [
                    { path: 'add', component: EditComponent },
                    { path: 'edit/:id', component: EditComponent },
                    { path: ':id', component: DetailComponent },
                    { path: '', component: ListComponent }
                ]
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class OrderRoutingModule {
    static components = [
        OrderComponent,
        ListComponent,
        DetailComponent,
        TrainingContentComponent,
        TerrainComponent,
        ScenarioComponent,
        EditComponent,
        PeopleComponent,
        OrderReviewComponent,
        ScheduleEventComponent,
        PdfComponent,
        HelpComponent
    ]
}

