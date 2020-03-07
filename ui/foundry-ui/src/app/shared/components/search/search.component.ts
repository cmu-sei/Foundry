/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Params, Router } from '@angular/router';
import { AnalyticsService } from 'src/app/analytics/analytics.service';
import { SettingsService } from 'src/app/root/settings.service';
import { ClientEventCreate, UserEventCreate } from '../../../core-api-models';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent implements OnInit {
  public routeName: string;
  registerSearchEvents = false;
  navColor: string;
  timer: any;
  term: string;
  last: string;
  @Output()
  fire: EventEmitter<string> = new EventEmitter<string>();
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private analyticsService: AnalyticsService,
    private settingsSvc: SettingsService
  ) {
    this.router.events.subscribe(e => {
      // detect route changes to set selected search type
      this.checkUrl();
    });
  }

  ngOnInit() {
    this.navColor = this.settingsSvc.settings.branding.navColor;
    this.checkUrl();

    this.router.events.forEach((event) => {
      if (event instanceof NavigationEnd) {
        this.route.queryParams.subscribe((params: Params) => {
         if (params['term']) {
          this.term = params['term'];
         } else {
           this.term = '';
         }
        });
      }
    });
  }

  changeBrowserType(e, term) {
    if (term) {
      this.search(e, term);
    } else {
      this.router.navigate([e]);
    }
  }

  search(type, term) {
    if (term) {
      if (this.registerSearchEvents === true) {
        const model: UserEventCreate = { type: 'search', data: term };

        this.analyticsService.addUserEvent(model).subscribe(result => { });
      }
      if (type === 'tag') {
        this.router.navigate([type], { queryParams: { tag: term } });
      } else {
        this.router.navigate([type], { queryParams: { term: term } });
      }

    } else {
      this.router.navigate([type]);
    }
  }

  refresh(term) {
    this.term = term;
    if (this.timer) { clearTimeout(this.timer); }
    this.timer = setTimeout(() => this.timerFired(), 500);
  }

  timerFired() {
    if (this.term !== this.last) {
      this.fire.emit(this.term);
    }
  }

  checkUrl () {
    if (this.router.url.includes('/content')) {
      this.routeName = 'content';
    } else if (this.router.url.includes('/playlist')) {
      this.routeName = 'playlist';
    } else if (this.router.url.includes('/group')) {
      this.routeName = 'group';
    } else if (this.router.url.includes('/tag')) {
      this.routeName = 'tag';
    } else if (this.router.url.includes('/profiles') || this.router.url.includes('/profile')) {
      this.routeName = 'profiles';
    } else {
      this.routeName = '';
    }
  }

}

