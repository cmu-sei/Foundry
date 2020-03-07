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
import { Observable } from "rxjs/Observable";
import { OperatingSystemTypeSummary, OrderDetail, OrderEdit, OrderEditFile, SecurityToolSummary, ServiceSummary, SimulatorSummary, TerrainSummary } from '../../api/gen/models';
import { OperatingSystemTypeService } from '../../api/operatingsystemtype.service';
import { OrderService } from '../../api/order.service';
import { SecurityToolService } from '../../api/securitytool.service';
import { ServiceService } from '../../api/service.service';
import { SimulatorService } from '../../api/simulator.service';
import { TerrainService } from '../../api/terrain.service';
import { SettingsService } from '../../svc/settings.service';

@Component({
    selector: 'order-terrain',
    templateUrl: './terrain.component.html',
    styleUrls: ['./terrain.component.css']

})
export class TerrainComponent {
    @Input()
    order: OrderEdit;
    operatingSystemTypes: Array<OperatingSystemTypeSummary> = new Array<OperatingSystemTypeSummary>();
    servicesTypes: Array<ServiceSummary> = new Array<ServiceSummary>();
    securityTools: Array<SecurityToolSummary> = new Array<SecurityToolSummary>();
    simulators: Array<SimulatorSummary> = new Array<SimulatorSummary>();
    terrains: Array<TerrainSummary> = new Array<TerrainSummary>();
    //uploadedFiles: Array<OrderEditFile> = new Array<OrderEditFile>();
    //uploadedNetworkDiagrams: Array<OrderEditFile> = new Array<OrderEditFile>();
    otherOSVisible: boolean = false;
    otherServiceVisible: boolean = false;
    otherToolVisible: boolean = false;
    otherSimulatorVisible: boolean = false;
    otherTerrainVisible: boolean = false;
    fileRootUrl: string = "";

    constructor(
        private svc: OrderService,
        private operatingSystemSvc: OperatingSystemTypeService,
        private serviceTypeSvc: ServiceService,
        private securityToolSvc: SecurityToolService,
        private simulatorSvc: SimulatorService,
        private terrainSvc: TerrainService,
        private settings: SettingsService
    ) {
        this.fileRootUrl = settings.urls.fileLocationUrl;
    }

    ngOnInit() {
        this.operatingSystemSvc.getOperatingsystemtypes({}).subscribe(
            result => {
                this.operatingSystemTypes = result.results;
                this.checkForOtherOS(this.order.operatingSystemTypes);
            }
        );

        this.serviceTypeSvc.getServices({}).subscribe(
            result => {
                this.servicesTypes = result.results;
                this.checkForOtherServices(this.order.services);
            }
        );

        this.securityToolSvc.getSecuritytools({}).subscribe(
            result => {
                this.securityTools = result.results;
                this.checkForOtherTools(this.order.securityTools);
            }
        );

        this.simulatorSvc.getSimulators({}).subscribe(
            result => {
                this.simulators = result.results;
                this.checkForOtherSimulators(this.order.simulators);
            }
        );

        this.terrainSvc.getTerrains({}).subscribe(
            result => {
                this.terrains = result.results;
                this.checkForOtherTerrains(this.order.terrains);
            }
        );

        if (!this.order.files) {
            this.order.files = new Array<OrderEditFile>();
        }
    }

    submit(): Observable<OrderDetail> {
        console.log("Submitting terrain form...");
        return this.svc.putOrder(this.order.id, this.order);
    }

    fileUploadComplete(result: FileStorageResult) {
        this.addFileToOrder(result, 'supportingFile');
    }

    networkDiagramUploadComplete(result: FileStorageResult) {
        this.addFileToOrder(result, 'networkDiagram');
    }

    addFileToOrder(result: FileStorageResult, type: string) {
        let url: string = this.settings.fileStorage.fileStorageUrl + result.file.urlWithExtension;
        let orderEditFile: OrderEditFile = { url: url, type: type };
        this.order.files.push(orderEditFile);
    }

    filteredFiles(type: string): Array<any> {
        return this.order.files.filter(
            (item) => { return item.type == type }
        );
    }

    checkForOtherOS(ids : Array<number>) {
        let other  = this.operatingSystemTypes.find(x => x.name == "Other");
        if (this.order && this.order.operatingSystemTypes) {
            if (this.order.operatingSystemTypes.indexOf(other.id) > -1) {
                this.otherOSVisible = true;
            } else {
                this.otherOSVisible = false;
            }
        }
    }

    checkForOtherServices(ids : Array<number>) {
        let other  = this.servicesTypes.find(x => x.name == "Other");
        if (this.order && this.order.services) {
            if (this.order.services.indexOf(other.id) > -1) {
                this.otherServiceVisible= true;
            } else {
                this.otherServiceVisible= false;
            }
        }
    }

    checkForOtherTools(ids : Array<number>) {
        let other  = this.securityTools.find(x => x.name == "Other");
        if (this.order && this.order.securityTools) {
            if (this.order.securityTools.indexOf(other.id) > -1) {
                this.otherToolVisible= true;
            } else {
                this.otherToolVisible= false;
            }
        }
    }

    checkForOtherSimulators(ids : Array<number>) {
        let other  = this.simulators.find(x => x.name == "Other");
        if (this.order && this.order.simulators) {
            if (this.order.simulators.indexOf(other.id) > -1) {
                this.otherSimulatorVisible= true;
            } else {
                this.otherSimulatorVisible= false;
            }
        }
    }

    checkForOtherTerrains(ids : Array<number>) {
        let other  = this.terrains.find(x => x.name == "Other");
        if (this.order && this.order.terrains) {
            if (this.order.terrains.indexOf(other.id) > -1) {
                this.otherTerrainVisible= true;
            } else {
                this.otherTerrainVisible= false;
            }
        }
    }
}

interface FileStorageResult {
    exception: any,
    file: FileDetail,
    fileName: string,
    globalId: string,
    type: string
}

interface FileDetail {
    bucketId: number
    bucketName: string,
    contentType: string,
    extension: string,
    globalId: string,
    id: number,
    length: number,
    name: string,
    slug: string,
    tags: Array<string>,
    url: string,
    urlWithExtension: string,
    versionNumber: number
}
