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
import { ActivatedRoute, Params } from '@angular/router';
import { Subscription } from 'rxjs';
import { DataFilter, DataSetResult, Report } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { ReportsService } from '../../reports.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'report',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})

export class ReportDetailComponent extends BaseComponent implements OnInit {
  profile$: Subscription;
  errorMsg: string;
  isAdministrator = false;
  exportType: string = null;
  spin: boolean = false;
  total: number;
  text: string = '';
  term: string = '';
  result: DataSetResult;
  takes: Array<number> = [10, 25, 50, 100, 200];
  report: Report;
  reports: Array<Report> = [];
  reportName: string;

  dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 25,
    sort: '',
    filter: ''
  };

  constructor(
    private route: ActivatedRoute,
    private profileService: ProfileService,
    private reportsService: ReportsService) {
    super();
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe(
      (p) => {
        this.isAdministrator = p.isAdministrator;
      }
    ));

    if (this.profileService.profile) { this.isAdministrator = this.profileService.profile.isAdministrator; }

    this.route.params.subscribe((params: Params) => {
      this.reportName = params['slug'];

      this.reportsService.list().subscribe(result => {
        this.reports = result;
        this.report = this.reports.filter(r => r.slug === this.reportName)[0];
        this.dataFilter.sort = this.report.defaultSort;
        this.reset();
      });
    });
  }

  export(type) {
    var dataFilter: DataFilter = {
      skip: 0, //export entire data set
      take: 0, //export entire data set
      sort: this.dataFilter.sort,
      term: this.dataFilter.term,
      filter: this.dataFilter.filter
    };

    this.reportsService.export(this.report.slug, dataFilter, type);
  };

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  };

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  };

  reset() {
    this.dataFilter.skip = 0;
    this.result = null;
    this.search();
  };

  isDisabled(verb) {
    if (verb === 'start') {
      return this.dataFilter.skip === 0;
    }

    if (verb === 'previous') {
      return this.dataFilter.skip === 0;
    }

    if (verb === 'next') {
      return (this.dataFilter.skip + this.dataFilter.take) > this.result.total;
    }

    if (verb === 'end') {
      return (this.dataFilter.skip + this.dataFilter.take) > this.result.total;
    }
  }

  action(verb) {
    if (this.isDisabled(verb)) return false;

    if (verb === 'start') { this.dataFilter.skip = 0; }
    if (verb === 'previous') { this.dataFilter.skip = this.dataFilter.skip - this.dataFilter.take; }
    if (verb === 'next') { this.dataFilter.skip = this.dataFilter.skip + this.dataFilter.take; }
    if (verb === 'end') { this.dataFilter.skip = Math.floor(this.result.total / this.dataFilter.take) * this.dataFilter.take; }

    this.search();
  }

  search() {
    if (!this.spin) {
      this.spin = true;

      this.$.push(this.reportsService.load(this.report.slug, this.dataFilter).subscribe(result => {
        this.spin = false;
        this.result = result;
        var showingStart = 0;
        var showingEnd = 0;

        if (this.result.total > 0) {
          showingStart = this.dataFilter.skip + 1;
          showingEnd = this.dataFilter.take == 0
            ? result.total
            : showingStart + this.dataFilter.take - 1;
          if (showingEnd > result.total) showingEnd = result.total;
        }
        this.text = showingStart + ' to ' + showingEnd + ' of ' + result.total;
        this.total = result.total;
      }));
    }
  }
}

