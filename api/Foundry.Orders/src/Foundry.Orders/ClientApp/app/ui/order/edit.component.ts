/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatStepper } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { OrderDetail, OrderEdit, OrderEditFile, OrderEditStatusEnum } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { SharedService } from '../../svc/shared.service';
import { OrderReviewComponent } from './orderreview.component';
import { PeopleComponent } from './people.component';
import { ScenarioComponent } from './scenario.component';
import { ScheduleEventComponent } from './scheduleevent.component';
import { TerrainComponent } from './terrain.component';
import { TrainingContentComponent } from './trainingcontent.component';

@Component({
    selector: 'order-editor',
    templateUrl: './edit.component.html'
})

export class EditComponent {
    @ViewChild('peopleForm')
    private peopleForm: PeopleComponent;
    @ViewChild('trainingForm')
    private trainingForm: TrainingContentComponent;
    @ViewChild('eventForm')
    private eventForm: ScheduleEventComponent;
    @ViewChild('terrainForm')
    private terrainForm: TerrainComponent;
    @ViewChild('scenarioForm')
    private scenarioForm: ScenarioComponent;
    @ViewChild('reviewForm')
    private reviewForm: OrderReviewComponent;
    isLinear = false;
    trainingContentOrderFormGroup: FormGroup;
    scheduleEventOrderFormGroup: FormGroup;
    reviewOrderFormGroup: FormGroup;
    peopleOrderFormGroup: FormGroup;
    scenarioOrderFormGroup: FormGroup;
    orderId: number;
    class: string;
    orderDetail: OrderDetail;

    order: OrderEdit = { files: new Array<OrderEditFile>() };

    constructor(
        private route: ActivatedRoute,
        private _formBuilder: FormBuilder,
        private svc: OrderService,
        private sharedSvc: SharedService,
        private router: Router) { }

    ngOnInit() {
        this.orderId = 0;

        this.isLinear = true;

        this.route.params.subscribe((params: Params) => {
            var value = params['id'];
            this.orderId = (value ? (+ value) : 0);
        });

        if (this.orderId > 0) {
            //this.isLinear = false;
            this.route.params.switchMap((params: Params) => this.svc.getOrderEdit(this.orderId))
                .subscribe(result => {
                    this.order = result;
                });
        }
        else {
            //this.isLinear = true;
        }
    }

    submitPeople(stepper: MatStepper) {
        if (this.peopleForm.validate()) {
            this.peopleForm.submit().subscribe((result: OrderDetail) => {
                this.order.id = result.id;

                this.svc.getOrderEdit(this.order.id).subscribe(result => {
                    this.order = result;
                    stepper.selectedIndex = 1;
                    //this.isLinear = false;
                });
            },
            error => {
                console.log(error.message);
            });
        }
        else {
            // TODO: need to figure out why stepper header highlights the wrong step
            console.log("invalid");
            //stepper.selected = stepper._steps[0];
            stepper.selectedIndex = 0;
            //this.isLinear = true;
            stepper._focusIndex = 0;
        }
    }

    submitTraining() {
        this.trainingForm.submit().subscribe((result: OrderDetail) => {
            this.orderDetail = result;
        },
        error => {
            console.log(error.message);
        });
    }

    submitEvent() {
        this.eventForm.submit().subscribe((result: OrderDetail) => {
            this.orderDetail = result;
        },
        error => {
            console.log(error.message);
        });
    }

    submitTerrain() {
        this.terrainForm.submit().subscribe((result: OrderDetail) => {
            this.orderDetail = result;
        },
        error => {
            console.log(error.message);
        });
    }

    submitScenario() {
        this.scenarioForm.submit().subscribe((result: OrderDetail) => {
            this.svc.getOrder(this.order.id).subscribe(result => {
                this.orderDetail = result;
            });
        },
            error => {
                console.log(error.message);
            });
    };


    submitOrder() {
        console.log(this.order.id);
        this.order.status = OrderEditStatusEnum.Submitted;

        this.svc.putOrder(this.order.id, this.order).subscribe(result => {
            this.orderDetail = result;
            this.sharedSvc.sendMessage("Your order has been updated!");
            this.router.navigateByUrl("order/" + this.order.id);
        },
        error => {
            this.order.status = OrderEditStatusEnum.Draft;
            console.log(error.message);
        });

        this.svc.postOrderEmail(this.order.id).subscribe(result => {
            this.orderDetail = result;
            this.sharedSvc.sendMessage("Your order has been submitted!");
            this.router.navigateByUrl("order/" + this.order.id);
        },
        error => {
            console.log(error.message);
        });
    }
}

