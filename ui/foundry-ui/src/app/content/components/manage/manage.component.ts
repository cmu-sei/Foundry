/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpEventType } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params } from '@angular/router';
import { DataFilterService } from 'src/app/root/data-filter.service';
import { AdvancedSelectDialogComponent } from 'src/app/shared/components/advanced-select-dialog/advanced-select-dialog.component';
import { ContentSummary, DataFilter, GroupSummary, ImportResult, PagedResult } from '../../../core-api-models';
import { GroupService } from '../../../group/group.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { ContentService } from '../../content.service';
import { ContentQuickEditComponent } from '../quick-edit/quick-edit.component';
import { ExportDialog } from './export.dialog';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class ContentManageComponent extends BaseComponent {
  @Input()
  result: PagedResult<ContentSummary> = null;
  groups: GroupSummary[] = [];
  isPowerUser: boolean = false;
  total: number;
  text: string = ''; 16
  more: boolean;
  spin: boolean;
  term: string = '';
  viewMode: string = 'tile';
  requestUrl: string = '';
  selected: ContentSummary[] = [];
  all: boolean = false;
  takes: Array<number> = [10, 25, 50, 100, 200];
  public importing: boolean = false;
  public exporting: boolean = false;
  public working: boolean = false;
  selectedGroupId: number = 0;
  selectedProfileId: number = 0;

  public progress: number;
  public message: string;

  dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 25,
    sort: '-recent',
    filter: ''
  };

  constructor(
    private contentService: ContentService,
    private groupService: GroupService,
    private profileService: ProfileService,
    private msgService: MessageService,
    private route: ActivatedRoute,
    private settingsSvc: SettingsService,
    private dataFilterSvc: DataFilterService,
    public dialog: MatDialog

  ) {
    super();
    this.contentService = contentService;
    this.requestUrl = this.settingsSvc.settings.clientSettings.urls.requestUrl;
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe(
      (p) => {
        this.isPowerUser = p.isPowerUser;
      }
    ));

    if (this.profileService.profile) this.isPowerUser = this.profileService.profile.isPowerUser;

    if (sessionStorage.hasOwnProperty('manage-content')) {
      this.dataFilter = this.dataFilterSvc.getDataFilter('manage-content');
      this.search();
    } else {
      this.reset();
    }

    this.route.queryParams.subscribe((params: Params) => {

      if (params['term']) {
        this.dataFilter.term = this.term = params['term'];
      }
      else if (params['type']) {
        this.term = params['type'];
        this.dataFilter.filter = 'contenttype=' + this.term;
      }
      else if (params['key']) {
        this.term = params['key'];
        this.dataFilter.filter = 'key=' + this.term;
      }
      else if (params['tag']) {
        this.term = params['tag'];
        this.dataFilter.filter = 'tag=' + this.term;
      }

      else if (params['filter']) {
        this.dataFilter.filter = params['filter'];
      }

      else if (params['sort']) {
        this.dataFilter.sort = params['sort'];
      }
    });

    this.loadGroups();
  };

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  };

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  };

  applyTerm(value: string) {
    var term = this.dataFilter.term.toLowerCase().trim();

    if (term == '')
      return value;

    if (value.toLowerCase().indexOf(term) === -1)
      return value;

    return value.replace(term, '<span class=\'highlight\'>' + term + '</span>');
  }

  reset() {
    this.dataFilter.skip = 0;
    this.result = null;
    this.search();
  };

  clearSelects() {
    this.selected = [];
    this.selectedGroupId = null;
    this.all = false;
  }

  search() {
    if (!this.spin) {
      this.spin = true;
      this.clearSelects();

      this.$.push(this.contentService.list(this.dataFilter).subscribe(result => {
        this.result = result;

        var df = this.result.dataFilter;
        var st = df.skip + 1;
        var en = st + df.take - 1;
        if (en > result.total) en = result.total;
        this.text = st + ' to ' + en + ' of ' + result.total;
        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
        this.spin = false;
      }));
    }
    this.dataFilterSvc.setDataFilter('manage-content', this.dataFilter);
  }

  termChanged(term) {
    this.dataFilter.term = term;
    this.reset();
  }

  loadGroups() {
    this.$.push(this.groupService.list({}).subscribe(data => {
      this.groups = data.results as GroupSummary[];
    }));
  }

  onContentChange(content, event) {
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

  onAllContentChange(value, event) {
    this.selected = [];
    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        this.selected.push(this.result.results[i]);
      }
    }
  }

  disable() {
    const ids = this.selected.map(({ id }) => id);
    this.$.push(this.contentService.disable(ids).subscribe(data => {
      if (data) {
        this.clearSelects();
        this.search();
        this.msgService.addSnackBar('Content disabled');
      }
    }));
  }

  enable() {
    const ids = this.selected.map(({ id }) => id);
    this.$.push(this.contentService.enable(ids).subscribe(data => {
      if (data) {
        this.clearSelects();
        this.search();
        this.msgService.addSnackBar('Content enabled');
      }
    }));
  }

  export() {
    const ids = this.selected.map(({ id }) => id);

    const dialogRef = this.dialog.open(ExportDialog, {
      data: {
        ids: ids,
        continueCallback: () => {
          this.clearSelects();
        },
        cancelCallback: () => {
        },
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }

  import(files) {
    this.working = this.importing = true;
    this.$.push(this.contentService.import(files).subscribe(
      event => {
        if (event.type === HttpEventType.UploadProgress) {
          this.progress = Math.round(100 * event.loaded / event.total);
        }
        if (event.type === HttpEventType.Response) {
          const result = event.body as Array<ImportResult>;
          this.msgService.addSnackBar('Import Complete');
          this.working = this.importing = false;
          this.reset();
        }
      }));
  }

  updateAuthor() {
    const ids = this.selected.map(({ id }) => id);
    this.$.push(this.profileService.updateAuthor(this.selectedProfileId, ids).subscribe(data => {
      if (data) {
        this.msgService.addSnackBar('Content author changed');
        this.clearSelects();
        this.search();
      }
    }));
  }

  openGroupSelectorDialog(): void {
    this.working = true;
    const dialogRef = this.dialog.open(AdvancedSelectDialogComponent, {
      maxHeight: '500px',
      data: {
        title: 'Choose author to add content to',
        selected: [],
        itemType: 'group',
        filter: 'managed+'
      }
    });

    this.$.push(dialogRef.afterClosed().subscribe(groupIds => {
      this.working = false;

      if (groupIds && groupIds.length > 0) {
        const groupId = groupIds[0];
        const ids = this.selected.map(({ id }) => id);

        this.groupService.updateSponsor(groupId, ids).subscribe((result) => {
          this.msgService.addSnackBar('Content sponsor changed');
          this.search();
        });
      }
    }));
  }

  openAuthorSelectorDialog(): void {
    this.working = true;
    const dialogRef = this.dialog.open(AdvancedSelectDialogComponent, {
      maxHeight: '500px',
      data: {
          title: 'Choose author to add content to',
          selected: [],
          itemType: 'profile',
          filter: ''
        }
    });

    this.$.push(dialogRef.afterClosed().subscribe(profileIds => {
      this.working = false;

      if (profileIds && profileIds.length > 0) {
        const profileId = profileIds[0];
        const ids = this.selected.map(({ id }) => id);

        this.profileService.updateAuthor(profileId, ids).subscribe((result) => {
          this.msgService.addSnackBar('Content author changed');
          this.search();
        });
      }
    }));
  }

  openEditDialog(id) {
    const dialogRef = this.dialog.open(ContentQuickEditComponent, {
      width: '800px',
      data: { id: id }
    });

    this.$.push(dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.search();
    }));
  }
}

