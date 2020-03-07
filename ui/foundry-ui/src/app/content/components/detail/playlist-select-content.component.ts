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
import { ContentService } from '../../../content/content.service';
import { ContentSummary, PlaylistDetail } from '../../../core-api-models';
import { PlaylistService } from '../../../playlist/playlist.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'playlist-select-content',
  template: '<mat-checkbox (change)="updatePlaylist(playlist.id)" [checked]="contentExists" [value]="playlist.id">{{ playlist.name }}</mat-checkbox>',
  styleUrls: ['./detail.component.scss'],
})
export class PlaylistSelectComponent extends BaseComponent implements OnInit {
  @Input()
  public playlist: PlaylistDetail;
  @Input()
  public contentId: number;
  contentExists: boolean;
  contents: Array<ContentSummary>;

  constructor(
    private service: ContentService,
    private playlistService: PlaylistService,
    private msgService: MessageService
  ) { super(); }

  ngOnInit() {
    this.$.push(this.playlistService.contents(this.playlist.id).subscribe(result => {
      this.contents = result.results as ContentSummary[];
      result.results.forEach(element => {
        if (element.id === this.contentId) {
          this.contentExists = true;
        }
      });
    }));
  }

  updatePlaylist(id) {
    if (!this.contentExists) {
      this.$.push(this.service.addToPlaylist(this.contentId, id).subscribe(result => {
        if (result === true) {
          this.contentExists = true;
          this.msgService.addSnackBar('Added to Playlist.');
        }
      }));
    } else {
      this.$.push(this.service.removeFromPlaylist(this.contentId, id).subscribe(result => {
        if (result === true) {
          this.contentExists = false;
          this.msgService.addSnackBar('Removed from Playlist.');
        }
      }));
    }
  }
}

