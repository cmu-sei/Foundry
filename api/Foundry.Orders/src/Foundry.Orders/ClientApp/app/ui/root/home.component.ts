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
import { Router } from "@angular/router";
import { OrderSummary, OrderSummaryStatusEnum, ProfileSummary } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { ProfileService } from '../../api/profile.service';
import { SharedService } from '../../svc/shared.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    profile: ProfileSummary;
    columns = ['order', 'isPrivate', 'created', 'branchName', 'rankName', 'unit', 'orderStatus', 'commentCount'];
    collections: Array<OrderCollection> = new Array<OrderCollection>(5);
    title: string;

    constructor(
        private orderService: OrderService,
        private profileService: ProfileService,
        private sharedService: SharedService,
        private router: Router
    ) { }

    initProfile(profile: ProfileSummary) {
        if (profile) {
            this.profile = profile;
            if (this.profile.isAdministrator) {
                if (this.profile.isAdministrator) {
                    this.title = "Event Designer Dashboard";
                }
                else {
                    this.title = "My Dashboard";
                }
            }

            this.load();
        }
    }

    ngOnInit() {

        this.profileService.profile$.subscribe(p => {
            this.initProfile(p);
        });

        this.initProfile(this.profileService.profile);
    };

    load() {
        this.orderService.getOrders({}).subscribe((result) => {
            if (result.total == 0 && !this.profile.isAdministrator) {
                this.router.navigate(['/order/add']);
            }
            else {
                var drafts = result.results.filter(r => r.status === OrderSummaryStatusEnum.Draft);
                this.collections[0] = new OrderCollection(drafts, 'My Drafts', 'draft');

                var submitted = result.results.filter(r => r.status === OrderSummaryStatusEnum.Submitted);
                this.collections[1] = new OrderCollection(submitted, 'Awaiting Approval', 'submitted');

                var inProgress = result.results.filter(r => r.status === OrderSummaryStatusEnum.InProgress);
                this.collections[2] = new OrderCollection(inProgress, 'In Progress', 'inprogress');

                var needsInformation = result.results.filter(r => r.status === OrderSummaryStatusEnum.NeedsInformation);
                this.collections[3] = new OrderCollection(needsInformation, 'Needs Information', 'needsinformation');

                var completed = result.results.filter(r => r.status === OrderSummaryStatusEnum.Complete);
                this.collections[4] = new OrderCollection(completed, 'Completed', 'complete');
            }
        });
    }

    setStatus(index, id, orderStatus: string) {
        this.orderService.setStatus(id, orderStatus).subscribe((result) => {
            this.sharedService.sendMessage('Order status changed.');
            this.load();
        });
    };
}

export class OrderCollection {
    orders: Array<OrderSummary>;
    text: string = '';
    status: string = '';

    constructor(orders: Array<OrderSummary>, text: string, status: string) {
        this.orders = orders;
        this.text = text;
        this.status = status.toLowerCase();
    }
}
