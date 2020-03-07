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
import { GroupSummary, PlaylistSummary } from 'src/app/core-api-models';
import { GroupService } from 'src/app/group/group.service';
import { PlaylistService } from 'src/app/playlist/playlist.service';

@Component({
  selector: 'app-select-dialog',
  templateUrl: './select-dialog.component.html',
  styleUrls: ['./select-dialog.component.scss']
})
export class SelectDialogComponent implements OnInit {

  playlists: Array<any>;
  groups: Array<any>;
  managedGroups: Array<any>;

  constructor(
    public dialogRef: MatDialogRef<SelectDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private playlistService: PlaylistService,
    private groupService: GroupService
    ) { }

  ngOnInit() {
    if (this.data.type === 'playlist') {
      this.loadPlaylists();
    } else {
      this.loadGroups();
    }
  }

  loadPlaylists() {
    this.playlistService.list({ sort: 'alphabetical', filter: 'managed+' }).subscribe((result) => {
      this.playlists = result.results as PlaylistSummary[];
    });
  }

  loadGroups() {
    this.groupService.list({ filter: 'managed+' }).subscribe(result => {
      this.groups = result.results as GroupSummary[];
    });
  }
}

