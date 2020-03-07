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
import { ActivatedRoute, Params, Router } from '@angular/router';
import { DataFilterService } from 'src/app/root/data-filter.service';
import { DataFilter, ProfileDetail, ProfileSummary } from '../../../core-api-models';
import { ProfileService } from '../../profile.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'profile-browser',
  templateUrl: './browser.component.html',
})
export class ProfileBrowserComponent extends BaseComponent implements OnInit {
  public profiles: ProfileSummary[];
  dataFilter: DataFilter = { skip: 0, take: 12, term: '', filter: '' };
  spin: boolean;
  more: boolean;
  profileTerm = '';
  viewMode = 'tile';
  profile: ProfileDetail;
  total: number;
  letters: string[] = [];

  constructor(
    private service: ProfileService,
    private activatedRoute: ActivatedRoute,
    private profileService: ProfileService,
    private dataFilterService: DataFilterService,
    private router: Router
  ) {
    super();
    let i = 'a'.charCodeAt(0), j = 'z'.charCodeAt(0);
    for (; i <= j; ++i) {
      this.letters.push(String.fromCharCode(i));
    }
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.viewMode = this.profileService.getProfileViewMode(p, 'profileviewmode');
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe(p => { this.initProfile(p); }));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    if (sessionStorage.hasOwnProperty('profile-browser')) {
      this.dataFilter = this.dataFilterService.getDataFilter('profile-browser');
    }

    this.activatedRoute.queryParams.subscribe((params: Params) => {
      this.dataFilter.term = this.profileTerm = params['term'] ? params['term'] : '';
      this.dataFilter.filter = params['filter'] ? params['filter'] : '';
      this.reset();
    });
  }

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'profileviewmode', viewMode);
    this.viewMode = viewMode;
  }

  startsWith(letter) {
    const filter = letter === '' ? '' : 'letter=' + letter;

    if (this.dataFilter.filter !== filter) {
      this.dataFilter.filter = filter;
      this.reset();
    }
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 16;
    this.profiles = [];
    this.search();
  }

  clearSearch() {
    this.dataFilter.term = this.profileTerm = '';
    this.router.navigate(['profiles']);
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.service.list(this.dataFilter).subscribe(data => {
        const results = data.results as ProfileSummary[];
        for (let i = 0; i < results.length; i++) {
          this.profiles.push(results[i]);
        }

        this.total = data.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterService.setDataFilter('profile-browser', this.dataFilter);
  }

  showMore() {
    if ((this.dataFilter.skip + this.dataFilter.take) < this.total) {
      this.dataFilter.skip += this.dataFilter.take;
      this.search();
    }
  }

  setViewMode(viewMode) {
    this.viewMode = viewMode;
    localStorage.setItem('profileViewMode', this.viewMode);
    this.showMore();
  }
}

