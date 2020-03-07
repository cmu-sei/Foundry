/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, ViewChild, ElementRef } from '@angular/core';
import { OrderEdit, OrderDetail, AssessmentTypeSummary, ContentTypeSummary, EventTypeSummary, OrderEditDurationTypeEnum } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { ContentTypeService } from '../../api/contenttype.service';
import { AssessmentTypeService } from '../../api/assessmenttype.service';
import { EventTypeService } from '../../api/eventtype.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'order-training-content',
    templateUrl: './trainingcontent.component.html',
    styleUrls: ['./trainingcontent.component.css']
})
export class TrainingContentComponent {
    @Input()
    order: OrderEdit;
    public fileUrl: string = "";
    errorMsg: string;
    durationTypeEnum: OrderEditDurationTypeEnum;
    assessmentTypes: Array<AssessmentTypeSummary> = new Array<AssessmentTypeSummary>();
    contentTypes: Array<ContentTypeSummary> = new Array<ContentTypeSummary>();
    eventTypes: Array<EventTypeSummary> = new Array<EventTypeSummary>();
    selectedTraining: string;
    defaultContentType: number;
    public state = 'inactive';

    constructor(
        private svc: OrderService,
        private eventTypeSvc: EventTypeService,
        private assessmentTypeSvc: AssessmentTypeService
    ) { }

    ngOnInit() {
        this.eventTypeSvc.getEventtypes({}).subscribe(
            result => {
                this.contentTypes = result.results;
                this.defaultContentType = this.contentTypes.find(x => x.name == "On-Demand - Automated Pre-scripted Scenario").id;
                //set default content type if new order
                if (!this.order.contentTypeId){
                    this.order.contentTypeId = this.defaultContentType;
                }
                this.getContentType(this.order.contentTypeId);
            }
        );

        this.assessmentTypeSvc.getAssessmenttypes({}).subscribe(
            result => {
                this.assessmentTypes = result.results;
            }
        );

    }

    submit(): Observable<OrderDetail> {
        console.log("Submitting training form...");

        return this.svc.putOrder(this.order.id, this.order);
    }

    getContentType(id){ 
        if (this.contentTypes && id) {
           let type  = this.contentTypes.find(x => x.id == id);
           if (type.name.includes("On-Demand")) {
                this.selectedTraining = "On-Demand";
           }
           else if (type.name.includes("Scheduled")) {
                this.selectedTraining = "Scheduled";   
           }
           else {
               this.selectedTraining = "";
           }
         }
    }

    toggleHelpState() { 
        this.state = this.state === 'active' ? 'inactive' : 'active';
    }

    isClosed() {
        this.state = 'inactive';
    }

}

