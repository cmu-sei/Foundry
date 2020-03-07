/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
import { AnalyticsService } from '../analytics/analytics.service';
import { PageViewMetric, PageViewMetricHistory } from '../core-api-models';
import { Router } from '@angular/router';
import { BaseComponent } from '../shared/components/base.component';

@Component({
  selector: 'page-view-metrics',
  templateUrl: 'page-view-metrics.component.html',
  styleUrls: ['page-view-metrics.component.scss'],
  providers: [NgbDropdownConfig]
})

export class PageViewMetricsComponent extends BaseComponent {
  @Input() public lastUrl: string;
  public pageViewMetric: PageViewMetric;
  public histories: Array<PageViewMetricHistory>;
  public max: number = 20;

  constructor(
    private analyticsService: AnalyticsService,
    private router: Router,
    config: NgbDropdownConfig
  ) {
    super();
    config.placement = 'bottom-right';
  }

  togglePageViewMetrics(event) {
    if (event) {
      this.$.push(this.analyticsService.metric(this.lastUrl).subscribe(result => {
        this.pageViewMetric = result;
        this.histories = this.pageViewMetric.history ? this.pageViewMetric.history.concat([]).slice(0, 5) : [];
      }));
    }
  }

  navigate(url: string) {
    this.router.navigateByUrl(url);
  }
}

