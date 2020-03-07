/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, Output, OnInit, EventEmitter, ViewChild, ElementRef, AfterViewChecked, OnChanges } from '@angular/core';
import { Router } from '@angular/router';
import { OrderService } from '../../api/order.service';
import { OrderEdit, OrderDetail, OrderEditStatusEnum, OrderDetailStatusEnum, BranchSummary, RankSummary, AudienceSummary, ProducerSummary, AudienceItemSummary, OrderEditDurationTypeEnum } from '../../api/gen/models';
import { NgForm, FormControl, Validators } from '@angular/forms';
import { BranchService } from '../../api/branch.service';
import { RankService } from '../../api/rank.service';
import { AudienceService } from '../../api/audience.service';
import { OrderErrorStateMatcher } from './form.validators';
import { AudienceItemService } from '../../api/audienceitem.service';
import { ProducerService } from '../../api/producer.service';
import { SharedService } from '../../svc/shared.service';
import { forEach } from '@angular/router/src/utils/collection';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'order-identify-people',
    templateUrl: './people.component.html',
    styleUrls: ['./people.component.css']
})
export class PeopleComponent implements OnInit, OnChanges {
    @Input()
    order: OrderEdit;
    @Output() 
    notifyValid: EventEmitter<boolean> = new EventEmitter<boolean>();
    @Output()
    notifyNewOrder: EventEmitter<number> = new EventEmitter<number>(); 
    errorMsg: string;
    peopleForm: NgForm;
    branches: Array<BranchSummary> = new Array<BranchSummary>();
    ranks: Array<RankSummary> = new Array<RankSummary>();
    audiences: Array<AudienceSummary> = new Array<AudienceSummary>();
    producers: Array<ProducerSummary> = new Array<ProducerSummary>();    
    audienceItemsTeams: Array<AudienceItemSummary> = new Array<AudienceItemSummary>();
    audienceItemsIndividuals: Array<AudienceItemSummary> = new Array<AudienceItemSummary>();        
    audienceItemOtherVisible: boolean;
    selectedBranch: string;
    selectedAudience: string;
    audienceItemIsOther: boolean;

    constructor(
        private router: Router,
        private svc: OrderService,
        private branchSvc: BranchService,
        private rankSvc: RankService,
        private audienceSvc: AudienceService,
        private audienceItemSvc: AudienceItemService,
        private producerSvc: ProducerService,
        private sharedSvc: SharedService
    ) {
    }

    ngOnChanges() {        
        if (this.order && this.order.branchName) {
            this.selectedBranch = this.order.branchName;
        }
    }

    ngOnInit() {
        this.branchSvc.getBranches({}).subscribe(
            result => {
                this.branches = result.results;
            }
        );
        this.rankSvc.getRanks({}).subscribe(
            result => {
                this.ranks = result.results;
            }
        );
        this.audienceSvc.getAudiences({}).subscribe(
            result => {
                this.audiences = result.results;
                this.getAudienceType(this.order.audienceId);
            }
        );
        this.audienceItemSvc.getAudienceItems(1, {}).subscribe(
            result => {
                this.audienceItemsTeams = result.results;
                this.getAudienceOther(this.order.audienceItems);
     
            }
        );
        this.audienceItemSvc.getAudienceItems(2, {}).subscribe(
            result => {
                this.audienceItemsIndividuals = result.results;
            }
        );
        this.producerSvc.getProducers({}).subscribe(
            result => {
                this.producers = result.results;
            }
        );
    }

    submit(): Observable<OrderDetail> {
        console.log("Submitting people form...");

        return this.order.id > 0
            ? this.svc.putOrder(this.order.id, this.order)
            : this.svc.postOrders(this.order);
    }

    validate(): boolean {
        if (this.emailFormControl.valid && this.requestorFormControl.valid && this.phoneFormControl.valid) {
            return true;
        }

        this.sharedSvc.sendMessage("Complete step 1 before proceeding!");        

        return false;
    }

    getAudienceOther(ids : Array<number>) {
        let other  = this.audienceItemsTeams.find(x => x.name == "Other");
        if (this.order && this.order.audienceItems) {
            if (this.order.audienceItems.indexOf(other.id) > -1) {
                this.audienceItemOtherVisible = true;
            } else {
                this.audienceItemOtherVisible = false;
            }
        }    
    }

    getBranchText(id){ 
        if (this.branches && id) {
           let branch  = this.branches.find(x => x.id == id);
           if (branch)
             this.selectedBranch = branch.name;
         }
         else
            this.selectedBranch = "";
    }

    getAudienceType(id){ 
        if (this.audiences && id) {
           let audience  = this.audiences.find(x => x.id == id);
           if (audience.name.includes("Collective")) {
                this.selectedAudience = "Collective";
           }
           else if (audience.name.includes("Individual")) {
                this.selectedAudience = "Individual";   
           }
           else {
               this.selectedAudience = "";
           }
         }
    }

    //form validators
    emailFormControl = new FormControl('', [
        Validators.required,
        Validators.email
    ]);

    requestorFormControl = new FormControl('', [
        Validators.required
    ]);

    phoneFormControl = new FormControl('', [
        Validators.pattern(/^\d{3}-\d{3}-\d{4}$/)
    ]);
    
    matcher = new OrderErrorStateMatcher();
}

