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
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { DataFilter, GroupSummary, ProfileDetail } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { GroupService } from '../../group.service';
import { InviteInputComponent } from '../invite-input/invite-input.component';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'group-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss'],
})
export class GroupBrowserComponent extends BaseComponent implements OnInit  {
  public groups: GroupSummary[];
  total: number;
  more: boolean;
  spin: boolean = false;
  groupTerm: string = '';
  viewMode: string = 'tile';
  profile: ProfileDetail;
  public dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '-recent',
    filter: ''
  };

  constructor(
    private service: GroupService,
    private activatedRoute: ActivatedRoute,
    private profileService: ProfileService,
    private dataFilterService: DataFilterService,
    private router: Router,
    public inviteDialog: MatDialog
  ) {
    super();
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.viewMode = this.profileService.getProfileViewMode(p, 'groupviewmode');
  }

  ngOnInit(): void {

    this.$.push(this.profileService.profile$.subscribe(p => { this.initProfile(p); }));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    if (sessionStorage.hasOwnProperty('group-browser')) {
      this.dataFilter = this.dataFilterService.getDataFilter('group-browser');
    }

    this.$.push(this.activatedRoute.queryParams.subscribe((params: Params) => {
      this.groupTerm = '';
      if (params['term']) {
        this.dataFilter.term = this.groupTerm = params['term'];
      }
      if (params['filter']) {
        this.dataFilter.filter = params['filter'];
      }
      if (!params['term']) {
        this.dataFilter.term = '';
      }
      this.reset();
    }));
  }

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'groupviewmode', viewMode);
    this.viewMode = viewMode;
  }

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  }

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 20;
    this.groups = [];
    this.search();
  }

  clearSearch() {
    this.dataFilter.term = this.groupTerm = '';
    this.router.navigate(['group']);
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.service.list(this.dataFilter).subscribe(data => {

        const results = data.results as GroupSummary[];

        for (let i = 0; i < results.length; i++) {
          this.groups.push(results[i]);
        }

        this.total = data.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterService.setDataFilter('group-browser', this.dataFilter);
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }


  openInviteInputDialog() {
    const dialogRef = this.inviteDialog.open(InviteInputComponent, {
      width: '600px'
    });
  }
}

