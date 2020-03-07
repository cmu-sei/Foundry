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
import { DataFilter, PagedResult, ProfileDetail, ProfileSummary } from '../../../core-api-models';
import { GroupService } from '../../../group/group.service';
import { ProfileService } from '../../../profile/profile.service';
import { DataFilterService } from '../../../root/data-filter.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { AdvancedSelectDialogComponent } from '../../../shared/components/advanced-select-dialog/advanced-select-dialog.component';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'manage',
  templateUrl: './manage.component.html',
  styleUrls: ['./manage.component.scss']
})
export class ProfileManageComponent extends BaseComponent implements OnInit {
  @Input()
  result: PagedResult<ProfileSummary> = null;
  isAdministrator = false;
  profileId: number;
  total: number;
  text = '';
  more: boolean;
  spin: boolean;
  term = '';
  requestUrl = '';
  selected: ProfileSummary[] = [];
  all = false;
  addToGroupCount: number;
  takes: Array<number> = [10, 25, 50, 100, 200];
  public working = false;
  public progress: number;
  public message: string;

  dataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 25,
    sort: 'alphabetic',
    filter: ''
  };

  constructor(
    private profileService: ProfileService,
    private groupService: GroupService,
    private messageService: MessageService,
    private settingsSvc: SettingsService,
    private dataFilterSvc: DataFilterService,
    public dialog: MatDialog
  ) {
    super();
    this.requestUrl = this.settingsSvc.settings.clientSettings.urls.requestUrl;
  }

  initProfile(p: ProfileDetail) {
    if (p) {
      this.isAdministrator = p.isAdministrator;
      this.profileId = p.id;
    }
  }

  ngOnInit(): void {
    this.$.push(this.profileService.profile$.subscribe(
      (p) => {
        this.initProfile(p);
      }
    ));

    this.initProfile(this.profileService.profile);

    if (sessionStorage.hasOwnProperty('manage-profiles')) {
      this.dataFilter = this.dataFilterSvc.getDataFilter('manage-profiles');
      this.search();
    } else {
      this.reset();
    }
  }

  toggleAdministrator(profile: ProfileSummary, index: number) {
    if (!this.working) {
      this.working = true;
      this.$.push(this.profileService.toggleAdministrator(profile.id).subscribe(result => {
        this.result.results[index] = result;
        this.messageService.addSnackBar('Administrator ' + (result.isAdministrator ? 'Added' : 'Removed'));
        this.working = false;
      }));
    }
  };

  togglePowerUser(profile: ProfileSummary, index: number) {
    if (!this.working) {
      this.working = true;
      this.$.push(this.profileService.togglePowerUser(profile.id).subscribe(result => {
        this.result.results[index] = result;
        this.messageService.addSnackBar('Power User ' + (result.isPowerUser ? 'Added' : 'Removed'));
        this.working = false;
      }));
    }
  };

  toggleDisabled(profile: ProfileSummary, index: number) {
    if (!this.working) {
      this.working = true;

      this.$.push(this.profileService.setDisabled(profile.id, !profile.isDisabled).subscribe(result => {
        this.result.results[index] = result;
        this.messageService.addSnackBar('User has been ' + (result.isDisabled ? 'disabled' : 'enabled'));
        this.working = false;
      }));
    }
  };

  clearSelects() {
    this.selected = [];
    this.all = false;
    this.addToGroupCount = 0;
  }

  onChange(profile: ProfileSummary, event) {
    this.all = false;
    if (event.target.checked) {
      this.addToGroupCount++;
      this.selected.push(profile);
    } else {
      for (let i = 0; i < this.selected.length; i++) {
        if (this.selected[i].id === profile.id) {
          this.selected.splice(i, 1);
          this.addToGroupCount--;
        }
      }
    }
  }

  onAllChange(value, event) {
    this.clearSelects();

    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        var profile = this.result.results[i];
        this.addToGroupCount++;
        this.selected.push(profile);
      }
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

    var result = value.replace(term, '<span class=\'highlight\'>' + term + '</span>');

    return result;
  }

  reset() {
    this.dataFilter.skip = 0;
    this.result = null;
    this.search();
  }

  search() {
    if (!this.spin) {
      this.clearSelects();
      this.spin = true;

      this.$.push(this.profileService.list(this.dataFilter).subscribe(result => {
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
    this.dataFilterSvc.setDataFilter('manage-profiles', this.dataFilter);
  }

  termChanged(term) {
    this.dataFilter.term = term;
    this.reset();
  }

  openGroupSelectorDialog(): void {
    this.working = true;
    const dialogRef = this.dialog.open(AdvancedSelectDialogComponent, {
      maxHeight: '500px',
      data: {
        title: 'Choose author to add content to',
        selected: [],
        itemType: 'group',
        filter: ''
      }
    });

    this.$.push(dialogRef.afterClosed().subscribe(groupIds => {
      console.log('The dialog was closed');

      this.working = false;

      if (groupIds && groupIds.length > 0) {
        const profileIds = this.selected.map((p) => {
          return p.id;
        });

        this.$.push(this.groupService.addMembersToGroups(groupIds, profileIds).subscribe((result) => {
          this.messageService.addSnackBar('Profiles added to groups');
          this.search();
        }));
      }
    }));
  }
}

