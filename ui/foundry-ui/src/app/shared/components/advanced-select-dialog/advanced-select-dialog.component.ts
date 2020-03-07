/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DataFilter } from 'src/app/core-api-models';
import { GroupService } from 'src/app/group/group.service';
import { ProfileService } from 'src/app/profile/profile.service';

export interface AdvancedSelectDialogData {
  title: string;
  noText: string;
  yesText: string;
  selected: number[];
  multiselect: boolean;
  filter: string;
  itemType: string;
}

@Component({
  selector: 'app-advanced-select-dialog',
  templateUrl: './advanced-select-dialog.component.html',
  styleUrls: ['./advanced-select-dialog.component.scss']
})

export class AdvancedSelectDialogComponent implements OnInit {
  public items: any[];
  public dataFilter: DataFilter = {
    term: '',
    filter: this.data.filter,
    sort: '-recent',
    skip: 0,
    take: 40,
  };
  more: boolean;
  total: number;
  term: string;

  constructor(
    public dialogRef: MatDialogRef<AdvancedSelectDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AdvancedSelectDialogData,
    private profileService: ProfileService,
    private groupService: GroupService
  ) {
    if (!this.data.title) {
      if (this.data.itemType === 'group') {
        this.data.title = 'Select Groups';
      }
      if (this.data.itemType === 'profile') {
        this.data.title = 'Select Profiles';
      }
    }
    if (!this.data.noText) { this.data.noText = 'Cancel'; }
    if (!this.data.yesText) { this.data.yesText = 'Submit'; }
    if (!this.data.selected) { this.data.selected = []; }
  }

  ngOnInit() {
    this.reset();
  }

  search(term) {
    this.items = [];
    this.dataFilter.term = term;
    if (this.data.itemType === 'group') {
      this.groupService.list(this.dataFilter).subscribe(result => {
        const itemResults = result.results;

        for (let i = 0; i < itemResults.length; i++) {
          this.items.push(itemResults[i]);
        }

        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
      });
    }
    if (this.data.itemType === 'profile') {
      this.profileService.list(this.dataFilter).subscribe(result => {
        const itemResults = result.results;

        for (let i = 0; i < itemResults.length; i++) {
          this.items.push(itemResults[i]);
        }

        this.total = result.total;
        this.more = (this.dataFilter.skip + this.dataFilter.take) < this.total;
      });
    }
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onChange(item: any) {
    const index = this.data.selected.indexOf(item.id);
    if (index === -1) {
      this.data.selected.push(item.id);
    } else {
      this.data.selected.splice(index, 1);
    }
  }

  isChecked(item: any) {
    return this.data.selected.indexOf(item.id) >= 0;
  }

  showMore() {
    this.dataFilter.skip += this.dataFilter.take;
    this.search('');
  }

  reset() {
    this.dataFilter.skip = 0;
    this.dataFilter.take = 40;
    this.dataFilter.term = '';
    this.dataFilter.filter = this.data.filter;
    this.term = '';
    this.items = [];
    this.search('');
  }
}

