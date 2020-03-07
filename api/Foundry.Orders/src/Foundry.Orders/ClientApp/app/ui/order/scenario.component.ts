/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnInit, OnChanges } from '@angular/core';
import { OrderDetail, OrderEdit, OperatingSystemTypeSummary, SecurityToolSummary, SimulatorSummary, TerrainSummary, ServiceSummary, ClassificationSummary, ThreatSummary, SupportSummary, EmbeddedTeamSummary, AssessmentTypeSummary } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { OperatingSystemTypeService } from '../../api/operatingsystemtype.service';
import { ServiceService } from '../../api/service.service';
import { GeneratedSecurityToolService } from '../../api/gen/securitytool.service';
import { SecurityToolService } from '../../api/securitytool.service';
import { SimulatorService } from '../../api/simulator.service';
import { TerrainService } from '../../api/terrain.service';
import { ClassificationService } from '../../api/classification.service';
import { ThreatService } from '../../api/threat.service';
import { SupportService } from '../../api/support.service';
import { EmbeddedTeamService } from '../../api/embeddedteam.service';
import { AssessmentTypeService } from '../../api/assessmenttype.service';

@Component({
    selector: 'order-scenario',
    templateUrl: './scenario.component.html',
    styleUrls: ['./scenario.component.css']

})
export class ScenarioComponent implements OnInit, OnChanges{
    @Input()
    order: OrderEdit;
    classificationLevels: Array<ClassificationSummary> = new Array<ClassificationSummary>();
    threatActors: ThreatSummary;
    supportTypes: SupportSummary;
    embeddedTeamTypes: EmbeddedTeamSummary;
    assessmentTypes: AssessmentTypeSummary;
    selectedClassification: string;

    constructor(
        private svc: OrderService,
        private classificationSvc : ClassificationService,
        private threatSvc: ThreatService,
        private supportSvc: SupportService,
        private embeddedSvc: EmbeddedTeamService,
        private assessmentSvc: AssessmentTypeService
    ) { }

    ngOnChanges() {
        if (this.order && this.order.classificationName) {
            this.selectedClassification = this.order.classificationName;
        }
    }

    ngOnInit() {
        this.classificationSvc.getClassifications({}).subscribe(
            result => {
                this.classificationLevels = result.results.sort((x1,x2) => x1.id - x2.id);
            }
        );
        this.threatSvc.getThreats({}).subscribe(
            result => {
                this.threatActors = result.results as ThreatSummary;
            }
        );
        this.supportSvc.getSupports({}).subscribe(
            result => {
                this.supportTypes = result.results as SupportSummary;
            }
        );
        this.embeddedSvc.getEmbeddedteams({}).subscribe(
            result => {
                this.embeddedTeamTypes = result.results as EmbeddedTeamSummary;
            }
        );
        this.assessmentSvc.getAssessmenttypes({}).subscribe(
            result => {
                this.assessmentTypes = result.results as AssessmentTypeSummary;
            }
        );
    }

    submit() {

        console.log("Submitting scenario form...");

        return this.svc.putOrder(this.order.id, this.order);

    }

    getClassificationText(id){
        if (this.classificationLevels && id) {
           let classification = this.classificationLevels.find(x => x.id == id);
           if (classification)
             this.selectedClassification = classification.name;
         }
         else
            this.selectedClassification = "";
    }

}

