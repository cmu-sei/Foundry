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
import { ActivatedRoute } from '@angular/router';
import { DataFilter, NotificationSummary, NotificationSummaryValue } from '../../../core-api-models';
import { NotificationService } from '../../notification.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'notification-preview',
  templateUrl: './preview.component.html',
  styleUrls: ['./preview.component.scss']
})
export class NotificationPreviewComponent extends BaseComponent {
  @Input()
  notifications: NotificationSummary[] = [];
  total: number;
  spin: boolean = false;

  public dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 3,
    sort: '-recent',
    filter: 'unread'
  };
  constructor(
    private notificationService: NotificationService,
    private route: ActivatedRoute
  ) {
    super();
  }

  ngOnInit(): void {
    this.preview();
  };

  notificationIcon(values: NotificationSummaryValue[]) {
    return this.notificationService.notificationIcon(values);
  }

  navigate(notification: NotificationSummary) {
    this.notificationService.navigate(notification);
  }

  preview() {
    if (!this.spin) {
      this.spin = true;

      this.$.push(this.notificationService.list(this.dataFilter).subscribe(result => {
        var results = result.results as NotificationSummary[];
        for (var i = 0; i < results.length; i++) {
          this.notifications.push(results[i]);
        }

        this.total = result.total;
        this.spin = false;
      }));
    }
  }
}

