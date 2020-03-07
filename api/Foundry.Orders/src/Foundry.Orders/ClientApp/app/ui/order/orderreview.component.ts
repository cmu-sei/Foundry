/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component,Input } from '@angular/core';
import { OrderDetail, OrderDetailStatusEnum, AudienceSummary } from '../../api/gen/models';

@Component({
    selector: 'order-review',
    templateUrl: './orderreview.component.html'
})
export class OrderReviewComponent {
    @Input()
    order: OrderDetail;
    class: string;

    ngOnInit() {
    }

    orderStatusLabel() :string {
        if (this.order.status == OrderDetailStatusEnum.Draft) {
            this.class = "badge badge-secondary";
        }
        if (this.order.status == OrderDetailStatusEnum.InProgress) {
            this.class = "badge badge-success";
        }
        if (this.order.status == OrderDetailStatusEnum.NeedsInformation) {
            this.class = "badge badge-warning";
        }
        if (this.order.status == OrderDetailStatusEnum.Submitted) {
            this.class = "badge badge-primary";
        }
        if (this.order.status == OrderDetailStatusEnum.Complete) {
            this.class = "badge badge-info";
        }
        if (this.order.status == OrderDetailStatusEnum.Closed) {
            this.class = "badge badge-danger";
        }
        return this.class;
    }
}


