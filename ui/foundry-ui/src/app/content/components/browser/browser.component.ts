/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ContentFilterType, ContentSummary, ContentType, DataFilter, ProfileDetail } from '../../../core-api-models';
import { BaseComponent } from '../../../shared/components/base.component';
import { ProfileService } from '../../../profile/profile.service';
import { SettingsService } from '../../../root/settings.service';
import { ContentService } from '../../content.service';
import { Observable } from 'rxjs';
import { DataFilterService } from '../../../root/data-filter.service';

@Component({
  selector: 'browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class ContentBrowserComponent extends BaseComponent implements OnInit {
  public contentFilterTypes: ContentFilterType[] = [
    { name: 'recommended', displayName: 'Recommended' },
    { name: 'bookmarked', displayName: 'My Bookmarks' },
    { name: 'myevents', displayName: 'My Events' },
    { name: 'created', displayName: 'My Content' },
  ];
  contentTypes: { value: string; name: string, checked: boolean }[] = [];
  selectedContentTypes = '';
  contents: ContentSummary[];
  selectedIndex: number;
  selectedFilter: string;
  profile: ProfileDetail;
  total: number;
  more: boolean;
  spin = false;
  term = '';
  viewMode = 'tile';
  requestUrl = '';
  public dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: 'top',
    filter: ''
  };

  constructor(
    private contentService: ContentService,
    private profileService: ProfileService,
    private route: ActivatedRoute,
    private settingsSvc: SettingsService,
    private dataFilterSvc: DataFilterService,
    private router: Router
  ) {
    super();

    this.contentService = contentService;
    this.requestUrl = this.settingsSvc.settings.clientSettings.urls.requestUrl;
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.viewMode = this.profileService.getProfileViewMode(p, 'contentviewmode');
    if (p.isPowerUser) {
      this.contentFilterTypes.push({ name: 'flagged', displayName: 'Flagged' });
    }
  }

  ngOnInit(): void {
    this.setContentTypes();
    this.profileService.profile$.subscribe(p => { this.initProfile(p); });

    if (sessionStorage.hasOwnProperty('content-browser')) {
      this.dataFilter = this.dataFilterSvc.getDataFilter('content-browser');
      if (this.dataFilter.filter) {
        if (this.dataFilter.filter.includes('contenttype=')) {
          const types = this.dataFilter.filter.replace(/\s/g, '').replace('contenttype=', '');
          let typesArray = [];
          typesArray = types.split(',');
          this.contentTypes.forEach(c => {
            typesArray.forEach(t => {
              if (t === c.value) {
                c.checked = true;
              }
            });
          });
        } else {
          this.selectedFilter = this.dataFilter.filter;
        }
      }
    }

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    this.route.queryParams.subscribe((params: Params) => {
      this.term = '';

      if (params['term']) {
        this.dataFilter.term = this.term = params['term'];
      } else if (params['type']) {
        this.term = params['type'];
        this.dataFilter.filter = 'contenttype=' + this.term;
      } else if (params['key']) {
        this.term = params['key'];
        this.dataFilter.filter = 'key=' + this.term;
      } else if (params['tag']) {
        this.term = params['tag'];
        this.dataFilter.filter = 'tag=' + this.term;
      } else if (params['filter']) {
        this.dataFilter.filter = params['filter'];
      } else if (params['sort']) {
        this.dataFilter.sort = params['sort'];
      }
      if (!params['term']) {
        this.dataFilter.term = this.term;
      }
      this.reset();
    });
  }

  setContentTypes(): void {
    // tslint:disable-next-line:forin
    for (const n in ContentType) {
      this.contentTypes.push({ value: <any>ContentType[n], name: n, checked: false });
    }
  };

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'contentviewmode', viewMode);
    this.viewMode = viewMode;
  };

  filter(index, filter) {
    this.dataFilter.filter = '';
    this.contentTypes.forEach(c => {
      c.checked = false;
    });
    this.dataFilter.filter = filter.name;
    this.selectedIndex = index;
    this.selectedFilter = filter.displayName;
    this.reset();
  }

  filterByType(filter) {
    this.selectedFilter = '';
    this.selectedIndex = null;
    if (!filter.checked) {
      this.selectedContentTypes += filter.value + ', ';
      this.dataFilter.filter = 'contenttype=' + this.selectedContentTypes;
      filter.checked = true;
    } else {
      this.selectedContentTypes = this.selectedContentTypes.replace(filter.value + ', ', '');
      this.dataFilter.filter = 'contenttype=' + this.selectedContentTypes;
      filter.checked = false;
    }

    if (this.selectedContentTypes === '') {
      this.dataFilter.filter = '';
    }

    this.reset();
  }

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 20;
    this.contents = [];
    this.search();
  }

  clearSearch() {
    this.dataFilter.term = this.term = '';
    this.router.navigate(['content']);
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.contentService.list(this.dataFilter).subscribe(result => {
        const results = result.results as ContentSummary[];
        for (let i = 0; i < results.length; i++) {
          this.contents.push(results[i]);
        }

        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterSvc.setDataFilter('content-browser', this.dataFilter);
  }

  termChanged(term) {
    this.dataFilter.term = term;
    this.reset();
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }
}

