/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input } from '@angular/core';
import { OrderEdit, OrderDetail, ClassificationSummary, FacilitySummary } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { ClassificationService } from '../../api/classification.service';
import { FacilityService } from '../../api/facility.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'order-schedule-event',
    templateUrl: './scheduleevent.component.html'
})
export class ScheduleEventComponent {

    @Input()
    order: OrderEdit;
    classifications: Array<ClassificationSummary> = new Array<ClassificationSummary>();
    facilities: Array<FacilitySummary> = new Array<FacilitySummary>();

    eventTypes = [
        { value: '0', viewValue: 'Training Exercise' },
        { value: '1', viewValue: 'Certification Exercise' },
        { value: '2', viewValue: 'Training Instruction' },
        { value: '3', viewValue: 'Mission Rehearsal' },
        { value: '4', viewValue: 'Demonstration' },
        { value: '5', viewValue: 'Experimentation' },
        { value: '6', viewValue: 'Test and Evaluation' },
        { value: '7', viewValue: 'Tactics, Techniques, and Procedure Development' },
        { value: '8', viewValue: 'Concept of Operations Development' },
        { value: '9', viewValue: 'Other' },
    ];

    constructor(
        private svc: OrderService,
        private classificationSvc: ClassificationService,
        private facilitySvc: FacilityService
    ) { }

    ngOnInit () {
        this.classificationSvc.getClassifications({}).subscribe(
            result => {
                this.classifications = result.results;
            }
        );

        this.facilitySvc.getFacilities({}).subscribe(
            result => {
                this.facilities = result.results;
            }
        );
    }

    submit(): Observable<OrderDetail> {        

        console.log("submitting event form...");

        return this.svc.putOrder(this.order.id, this.order);        
    }
}
