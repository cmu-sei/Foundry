/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../api/order.service';
import { OrderSummary, ProfileSummary, PagedResultOrderOrderSummary } from '../../api/gen/models';
import { SharedService } from '../../svc/shared.service';
import { ProfileService } from '../../api/profile.service';

@Component({
    selector: 'list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.css']
})
export class ListComponent {
    constructor(
        private orderService: OrderService,
        private sharedService: SharedService,
        private profileService: ProfileService
    ) {
    }
    orders: PagedResultOrderOrderSummary;
    title: string;
    columns = [];
    profile: ProfileSummary;

    ngOnInit() {
        this.title = "View All Orders";
        this.columns = ['orderNumber', 'requestor', 'created', 'description', 'branchName', 'due', 'orderStatus', 'commentCount'];

        this.load();

        this.profileService.profile$.subscribe(p => {
            this.initProfile(p);
        });

        this.initProfile(this.profileService.profile);
    }

    initProfile(profile: ProfileSummary) {
        if (profile) {
            this.profile = profile;
        }
    }

    load() {
        this.orderService.getOrders({}).subscribe(
            (result) => {
                this.orders = result.results as PagedResultOrderOrderSummary;
            }
        )
    }

    setStatus(index, id, orderStatus: string) {
        this.orderService.setStatus(id, orderStatus).subscribe((result) => {
            this.sharedService.sendMessage('Order status changed.');
            this.load();
        });
    };
}
