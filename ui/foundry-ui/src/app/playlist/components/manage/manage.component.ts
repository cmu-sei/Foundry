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
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute } from '@angular/router';
import { DataFilter, ImportResult, PagedResult, PlaylistSummary } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { PlaylistService } from '../../playlist.service';
import { ExportPlaylistDialog } from './export.playlist.dialog';

@Component({
  selector: 'manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class PlaylistManageComponent extends BaseComponent implements OnInit {
  @Input()
  result: PagedResult<PlaylistSummary> = null;
  isPowerUser = false;
  total: number;
  text = '';
  more: boolean;
  spin: boolean;
  term = '';
  requestUrl = '';
  selected: PlaylistSummary[] = [];
  all = false;
  takes: Array<number> = [10, 25, 50, 100, 200];
  exportPlaylistCount: number = 0;
  exportContentCount: number = 0;
  public importing = false;
  public exporting = false;
  public working = false;
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
    private playlistService: PlaylistService,
    private profileService: ProfileService,
    private msgService: MessageService,
    private settingsSvc: SettingsService,
    private dataFilterSvc: DataFilterService,
    public dialog: MatDialog
  ) {
    super();
    this.requestUrl = this.settingsSvc.settings.clientSettings.urls.requestUrl;
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe(
      (p) => {
        this.isPowerUser = p.isPowerUser;
      }
    ));

    if (this.profileService.profile) { this.isPowerUser = this.profileService.profile.isPowerUser; }

    if (sessionStorage.hasOwnProperty('manage-playlist')) {
      this.dataFilter = this.dataFilterSvc.getDataFilter('manage-playlist');
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
    this.search();
  }

  clearSelects() {
    this.selected = [];
    this.all = false;
    this.exportPlaylistCount = 0;
    this.exportContentCount = 0;
  }

  search() {
    if (!this.spin) {
      this.clearSelects();
      this.spin = true;

      this.$.push(this.playlistService.list(this.dataFilter).subscribe(result => {
        this.result = result;
        const df = this.result.dataFilter;
        const st = df.skip + 1;
        let en = st + df.take - 1;
        if (en > result.total) {
          en = result.total;
        }
        this.text = st + ' to ' + en + ' of ' + result.total;
        this.total = result.total;
        this.spin = false;
      }));
    }
    this.dataFilterSvc.setDataFilter('manage-playlist', this.dataFilter);
  }

  termChanged(term) {
    this.dataFilter.term = term;
    this.reset();
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search();
  }

  onChange(playlist: PlaylistSummary, event) {
    this.all = false;
    if (event.target.checked) {
      this.exportPlaylistCount++;
      this.exportContentCount += playlist.contentCount;
      this.selected.push(playlist);
    } else {
      for (let i = 0; i < this.selected.length; i++) {
        if (this.selected[i].id === playlist.id) {
          this.selected.splice(i, 1);
          this.exportPlaylistCount--;
          this.exportContentCount -= playlist.contentCount;
        }
      }
    }
  }

  onAllChange(value, event) {
    this.clearSelects();

    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        var playlist = this.result.results[i];
        this.exportPlaylistCount++;
        this.exportContentCount += playlist.contentCount;
        this.selected.push(playlist);
      }
    }
  }

  export() {
    const ids = this.selected.map(({ id }) => id);

    const dialogRef = this.dialog.open(ExportPlaylistDialog, {
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
    this.$.push(this.playlistService.import(files).subscribe(
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
}

