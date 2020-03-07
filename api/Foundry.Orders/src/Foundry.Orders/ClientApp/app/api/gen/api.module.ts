/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { AssessmentTypeService } from "../assessmenttype.service";
import { AudienceService } from "../audience.service";
import { AudienceItemService } from "../audienceitem.service";
import { BranchService } from "../branch.service";
import { ClassificationService } from "../classification.service";
import { CommentService } from "../comment.service";
import { ContentTypeService } from "../contenttype.service";
import { EmbeddedTeamService } from "../embeddedteam.service";
import { EventTypeService } from "../eventtype.service";
import { FacilityService } from "../facility.service";
import { FileService } from "../file.service";
import { OperatingSystemTypeService } from "../operatingsystemtype.service";
import { OrderService } from "../order.service";
import { ProducerService } from "../producer.service";
import { ProfileService } from "../profile.service";
import { RankService } from "../rank.service";
import { SecurityToolService } from "../securitytool.service";
import { ServiceService } from "../service.service";
import { SimulatorService } from "../simulator.service";
import { SupportService } from "../support.service";
import { TerrainService } from "../terrain.service";
import { ThreatService } from "../threat.service";

import { NgModule } from '@angular/core';
import { HttpClientModule } from "@angular/common/http";
import { ApiSettings } from "../api-settings";

@NgModule({
    imports: [ HttpClientModule ],
    providers: [
        ApiSettings,
        AssessmentTypeService,
		AudienceService,
		AudienceItemService,
		BranchService,
		ClassificationService,
		CommentService,
		ContentTypeService,
		EmbeddedTeamService,
		EventTypeService,
		FacilityService,
		FileService,
		OperatingSystemTypeService,
		OrderService,
		ProducerService,
		ProfileService,
		RankService,
		SecurityToolService,
		ServiceService,
		SimulatorService,
		SupportService,
		TerrainService,
		ThreatService
    ]
})
export class ApiModule { }

