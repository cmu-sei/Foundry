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
import { GroupSummary } from '../../../core-api-models';
import { PlaylistService } from '../../../playlist/playlist.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'group-playlist-select',
  template: '<mat-checkbox (change)="updateGroup(group.id)" [checked]="groupExists" [value]="group.id">{{ group.name }}</mat-checkbox>',
  styleUrls: ['./detail.component.scss'],
})
export class GroupPlaylistSelectComponent extends BaseComponent implements OnInit {
  @Input()
  public group: GroupSummary;
  @Input()
  public playlistId: number;
  public groupExists: boolean;
  playlists: any;

  constructor(
    private playlistService: PlaylistService,
    private msgService: MessageService
  ) {
    super();
  }

  ngOnInit() {
    this.$.push(this.playlistService.load(this.playlistId).subscribe(result => {
      result.groupFollowers.forEach(element => {
        if (element.groupId.toString() === this.group.id) {
          this.groupExists = true;
        }
      });
    }));
  }

  updateGroup(id) {
    if (!this.groupExists) {
      this.$.push(this.playlistService.groupFollow(this.playlistId, id).subscribe(
        () => {
          this.msgService.addSnackBar('Playlist Added To Group');
          this.groupExists = true;
        },
        error => this.msgService.addSnackBar(error.error.message)));

    } else {
      this.$.push(this.playlistService.groupUnfollow(this.playlistId, id).subscribe(
        () => {
          this.msgService.addSnackBar('Playlist Removed From Group');
          this.groupExists = false;
        },
        error => this.msgService.addSnackBar(error.error.message)));
    }
  }
}

