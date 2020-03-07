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
import { DataFilter, PlaylistSummary, ProfileDetail } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { PlaylistService } from '../../playlist.service';

@Component({
  selector: 'playlist-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class PlaylistBrowserComponent extends BaseComponent implements OnInit {
  playlists: PlaylistSummary[];
  playlistTerm = '';
  total: number;
  more: boolean;
  spin = false;
  viewMode = 'tile';
  profile: ProfileDetail;
  public dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '-recent',
    filter: ''
  };
  constructor(
    private service: PlaylistService,
    private activatedRoute: ActivatedRoute,
    private profileService: ProfileService,
    private dataFilterService: DataFilterService,
    private router: Router
  ) {
    super();
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.viewMode = this.profileService.getProfileViewMode(p, 'playlistviewmode');
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe(p => this.initProfile(p)));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    if (sessionStorage.hasOwnProperty('playlist-browser')) {
      this.dataFilter = this.dataFilterService.getDataFilter('playlist-browser');
    }

    this.activatedRoute.queryParams.subscribe((params: Params) => {
      this.playlistTerm = '';

      if (params['term']) {
        this.dataFilter.term = this.playlistTerm = params['term'] ? params['term'] : '';
      } else if (params['tag']) {
        this.playlistTerm = params['tag'];
        this.dataFilter.filter = 'tag=' + this.playlistTerm;
      }

      if (!params['term']) {
        this.dataFilter.term = '';
      }

      this.reset();
    });
  }

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'playlistviewmode', viewMode);
    this.viewMode = viewMode;
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.service.list(this.dataFilter).subscribe(result => {

        const results = result.results;

        for (let i = 0; i < results.length; i++) {
          this.playlists.push(results[i]);
        }

        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterService.setDataFilter('playlist-browser', this.dataFilter);
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
    this.playlists = [];
    this.search();
  }

  clearSearch() {
    this.dataFilter.term = this.playlistTerm = '';
    this.router.navigate(['playlist']);
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }
}

