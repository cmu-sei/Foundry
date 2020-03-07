/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { ApplicationSummary, ApplicationUpdate, DataFilter, PagedResult } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { ExtensionService } from '../../extension.service';

@Component({
  selector: 'manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class ApplicationManageComponent extends BaseComponent
  implements OnInit {
  @Input()
  result: PagedResult<ApplicationSummary> = null;
  isPowerUser = false;
  total: number;
  text = '';
  more: boolean;
  spin: boolean;
  term = '';
  requestUrl = '';
  selected: ApplicationSummary[] = [];
  all = false;
  takes: Array<number> = [10, 25, 50, 100, 200];
  public importing = false;
  public exporting = false;
  public working = false;
  public synchronizing = false;
  public pushing = false;
  public updating = false;
  public progress: number;
  public message: string;

  dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 25,
    sort: '-alphabetic',
    filter: ''
  };

  constructor(
    private profileService: ProfileService,
    private extensionService: ExtensionService,
    private msgService: MessageService,
    private route: ActivatedRoute,
    private dataFilterSvc: DataFilterService,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe((p) => { this.isPowerUser = p.isPowerUser; }));
    if (this.profileService.profile) { this.isPowerUser = this.profileService.profile.isPowerUser; }

    if (sessionStorage.hasOwnProperty('manage-apps')) {
      this.dataFilter = this.dataFilterSvc.getDataFilter('manage-apps');
      this.search();
    } else {
      this.reset();
    }
  }

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  }

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  }

  applyTerm(value: string) {
    const term = this.dataFilter.term.toLowerCase().trim();

    if (term === '') {
      return value;
    }

    if (value.toLowerCase().indexOf(term) === -1) {
      return value;
    }

    return value.replace(term, '<span class=\'highlight\'>' + term + '</span>');
  }

  reset() {
    this.dataFilter.skip = 0;
    this.result = null;
    this.clearSelects();
    this.search();
  }

  clearSelects() {
    this.selected = [];
    this.all = false;
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.$.push(this.extensionService.list(this.dataFilter).subscribe(result => {
        this.result = result;
        this.spin = false;
      }));
    }
    this.dataFilterSvc.setDataFilter('manage-apps', this.dataFilter);
  }

  termChanged(term) {
    this.dataFilter.term = term;
    this.reset();
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }

  onChange(app, event) {
    this.all = false;
    if (event.target.checked) {
      this.selected.push(app);
    } else {
      for (let i = 0; i < this.selected.length; i++) {
        if (this.selected[i].id === app.id) {
          this.selected.splice(i, 1);
        }
      }
    }
  }

  onAllChange(value, event) {
    this.selected = [];
    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        this.selected.push(this.result.results[i]);
      }
    }
  }

  push() {
    const ids = [];
    for (let i = 0; i < this.selected.length; i++) {
      ids.push(this.selected[i].id);
    }

    if (ids.length > 0) {
      this.working = this.pushing = true;
      this.$.push(this.extensionService.push(ids).subscribe(() => {
        this.working = this.pushing = false;
        this.msgService.addSnackBar(ids.length + ' Application' + (ids.length == 1 ? ' has' : 's have') + ' been pushed to all users.');
        this.selected = [];
        this.all = false;
      }));
    }
  }
  synchronize() {
    this.working = this.synchronizing = true;
    this.$.push(this.extensionService.sync().subscribe(() => {
      this.working = this.synchronizing = false;
      this.reset();
      this.msgService.addSnackBar('Applications have been synchronized.');
    }));
  }

  toggleHidden(app: ApplicationSummary, index: number) {
    this.update(app.id, index, !app.isHidden, app.isPinned);
  }

  togglePinned(app: ApplicationSummary, index: number) {
    this.update(app.id, index, app.isHidden, !app.isPinned);
  }

  update(id: number, index: number, isHidden: boolean, isPinned: boolean) {
    this.working = this.updating = true;

    const model: ApplicationUpdate = {
      id: id,
      isHidden: isHidden,
      isPinned: isPinned
    };

    this.$.push(this.extensionService.update(model).subscribe(result => {
      this.working = this.updating = false;
      this.result.results[index] = result;
      this.msgService.addSnackBar(result.displayName + ' has been updated');
    }));
  }
}

