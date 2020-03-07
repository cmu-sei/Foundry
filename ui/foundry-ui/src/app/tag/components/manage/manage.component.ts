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
import { DataFilter, PagedResult, Tag } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { TagService } from '../../tag.service';
import { EditTagDialog } from './edit.dialog';

@Component({
  selector: 'tag-manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class TagManageComponent extends BaseComponent implements OnInit {
  @Input()
  result: PagedResult<Tag> = null;
  isPowerUser = false;
  total: number;
  text = '';
  more: boolean;
  spin: boolean;
  term = '';
  viewMode = 'tile';
  requestUrl = '';
  selected: Tag[] = [];
  all = false;
  takes: Array<number> = [10, 25, 50, 100, 200];
  working = false;

  dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 25,
    sort: 'popular',
    filter: 'type='
  };

  constructor(
    private tagService: TagService,
    private profileService: ProfileService,
    private dataFilterService: DataFilterService,
    public dialog: MatDialog
  ) {
    super();
    this.tagService = tagService;
  }

  ngOnInit(): void {

    this.$.push(this.profileService.profile$.subscribe((p) => this.isPowerUser = p.isPowerUser));

    if (this.profileService.profile) { this.isPowerUser = this.profileService.profile.isPowerUser; }

    if (sessionStorage.hasOwnProperty('manage-apps')) {
      this.dataFilter = this.dataFilterService.getDataFilter('manage-tags');
      this.search();
    } else {
      this.reset();
    }
  }

  edit(tag) {
    const dialogRef = this.dialog.open(EditTagDialog, {
      data: {
        tag: tag,
        continueCallback: () => this.reset(),
        cancelCallback: () => { },
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe();
  }

  confirmDelete(tag: Tag) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete \'' + tag.name + '\'',
        message: 'Are you sure you want to delete this tag?',
        yesText: 'Yes',
        yesCallback: () => { this.delete(tag); },
        noText: 'No',
        noCallback: this.dialog.closeAll,
        parent: this
      }
    });

    dialogRef.afterClosed().subscribe(() => { });
  }

  delete(tag: Tag) {
    this.$.push(this.tagService.delete(tag.name).subscribe(() => {
      this.search();
    }));
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
    this.result = null;
    this.clearSelects();
    this.search();
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

  search() {
    if (!this.spin) {
      this.spin = true;

      this.$.push(this.tagService.list(this.dataFilter).subscribe(result => {
        this.result = result;

        const df = this.result.dataFilter;
        const st = df.skip + 1;
        let en = st + df.take - 1;
        if (en > result.total) { en = result.total; }
        this.text = st + ' to ' + en + ' of ' + result.total;
        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterService.setDataFilter('manage-tags', this.dataFilter);
  }

  clearSelects() {
    this.selected = [];
    this.all = false;
  }

  onTagChange(content, event) {
    this.all = false;
    if (event.target.checked) {
      this.selected.push(content);
    } else {
      for (let i = 0; i < this.selected.length; i++) {
        if (this.selected[i].id === content.id) {
          this.selected.splice(i, 1);
        }
      }
    }
  }

  onAllTagChange(event) {
    this.selected = [];
    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        this.selected.push(this.result.results[i]);
      }
    }
  }
}

