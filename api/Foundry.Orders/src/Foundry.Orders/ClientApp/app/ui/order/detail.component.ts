/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { CommentService } from '../../api/comment.service';
import { CommentDetail, CommentEdit, OrderDetail, OrderDetailStatusEnum, ProfileSummary } from '../../api/gen/models';
import { OrderService } from '../../api/order.service';
import { ProfileService } from '../../api/profile.service';
import { SettingsService } from '../../svc/settings.service';
import { SharedService } from '../../svc/shared.service';
import { ConfirmDialogComponent } from '../common-ui/confirm-dialog.component';

@Component({
    selector: 'order-detail',
    templateUrl: './detail.component.html',
    styleUrls: ['./detail.component.css']
})
export class DetailComponent {
    constructor(
        private orderService: OrderService,
        private commentService: CommentService,
        private route: ActivatedRoute,
        private profileService: ProfileService,
        private settings: SettingsService,
        private sharedService: SharedService,
        public dialog: MatDialog,
        private router: Router
    ) {
        this.topoMojoUrl = settings.urls.topoMojoUrl;
        this.cartographerUrl = settings.urls.cartographerUrl;
    }

    commentError: string;
    order: OrderDetail;
    comments: CommentDetail[];
    error: string;
    title: string;
    message: string;
    class: string;
    profile: ProfileSummary;
    topoMojoUrl: string = "";
    cartographerUrl: string = "";

    initProfile(profile: ProfileSummary) {
        if (profile) {
            this.profile = profile;
        }
    }

    ngOnInit() {

        this.profileService.profile$.subscribe(p => {
            this.initProfile(p);
        });

        this.initProfile(this.profileService.profile);

        this.route.params
            .switchMap((params: Params) => {
                var id = params['id'];
                return this.orderService.getOrder(id);
            })
            .subscribe(result => {
                this.order = result as OrderDetail;

                this.loadComments();
            });
    }

    loadComments() {

        this.commentService.getOrderComments(this.order.id, {}).subscribe(result => {
            this.comments = result.results;
        });
    };

    addComment() {
        this.commentError = null;
        var comment: CommentEdit = { title: this.title, message: this.message };

        this.commentService.postOrderComments(this.order.id, comment).subscribe(result => {
            this.title = '';
            this.message = '';
            this.loadComments();
        },
            err => {
                this.commentError = err.error;
            },
            () => { }
        );
    };

    orderStatusLabel(): string {
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
    };

    setStatus() {
        this.orderService.setStatus(this.order.id, this.order.status.toString()).subscribe((result) => {
            this.sharedService.sendMessage("Order status has been changed.");
        });
    };

    deleteOrder() {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
          data: {
            title: 'Confirm Delete',
            message: 'Are you sure you want to delete this order?',
            yesText: 'Delete',
            yesCallback: () => {
                if (this.order.status === OrderDetailStatusEnum.InProgress) {
                    this.sharedService.sendMessage("Orders in progress cannot be deleted");
                    dialogRef.close();
                } else {
                    this.orderService.deleteOrder(this.order.id).subscribe((result) => {
                        this.sharedService.sendMessage("Order has been deleted.");
                        dialogRef.close();
                        this.router.navigateByUrl('/order');
                    }, error => {
                        this.sharedService.sendMessage(error.error.message);
                    });
                }
            },
            noText: 'Cancel',
            noCallback: () => { dialogRef.close();},
            parent: this
          }
        });
        dialogRef.afterClosed().subscribe(result => { });
      }
}

