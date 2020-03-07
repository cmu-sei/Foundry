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
import { ContentService } from '../../../content/content.service';
import { DataFilter, ProfileDetail } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'playlist-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class TagBrowserComponent extends BaseComponent implements OnInit {
  selectedFilter = 'all';
  searchTerm = '';
  contents: any[] = [];
  playlists: any[] = [];
  items: any[] = [];
  total: number;
  more: boolean;
  spin = false;
  viewMode = 'tile';
  profile: ProfileDetail;
  public dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 50,
    sort: '-recent',
    filter: ''
  };

  constructor(
    private contentService: ContentService,
    private activatedRoute: ActivatedRoute,
    private profileService: ProfileService
  ) {
    super();
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.viewMode = this.profileService.getProfileViewMode(p, 'tagviewmode');
  }

  ngOnInit(): void {

    this.$.push(this.profileService.profile$.subscribe(p => this.initProfile(p)));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    this.activatedRoute.queryParams.subscribe((params: Params) => {
      this.searchTerm = '';

      if (params['tag']) {
        this.searchTerm = params['tag'];
        this.dataFilter.filter = 'tag=' + this.searchTerm;
      }

      if (!params['tag']) {
        this.dataFilter.filter = '';
      }

      this.reset();
    });

    this.reset();
  }

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'tagviewmode', viewMode);
    this.viewMode = viewMode;
  };

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.contentService.getFeaturedContent(this.dataFilter).subscribe(data => {
        const itemArray = [];
        if ((this.selectedFilter === 'content') || (this.selectedFilter === 'all')) {
          this.contents = data[0].results.map(c => {
            const item = {
              ...c, objectType: 'content', encodedLogo: encodeURI(c.logoUrl)

            };
            return item;
          });

          this.contents.forEach(item => {
            itemArray.push(item);
          });
        }

        if ((this.selectedFilter === 'playlists') || (this.selectedFilter === 'all')) {
          this.playlists = data[1].results.map(p => {
            const item = {
              ...p, objectType: 'playlist', encodedLogo: encodeURI(p.logoUrl)
            };
            return item;
          });

          this.playlists.forEach(item => {
            itemArray.push(item);
          });
        }

        this.items = itemArray.sort(function (x, y) {
          x = new Date(x.created);
          y = new Date(y.created);
          return x > y ? -1 : x < y ? 1 : 0;
        });
        this.total = this.items.length;
        this.spin = false;
      }));
    }
  }

  filter(filter) {
    this.selectedFilter = filter;
    this.reset();
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 16;
    this.items = [];
    this.search();
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }
}

