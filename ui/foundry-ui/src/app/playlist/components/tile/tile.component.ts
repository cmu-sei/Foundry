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
import { PlaylistDetail, PlaylistSummary } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { PlaylistService } from '../../playlist.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'playlist-tile',
  templateUrl: './tile.component.html',
  styleUrls: ['./tile.component.scss'],
})
export class PlaylistTileComponent extends BaseComponent implements OnInit {
  @Input() public playlist: PlaylistSummary;
  @Input() public groupId: number;
  @Input() public canManageGroup: boolean;
  public playlistFollow: PlaylistDetail;
  errorMsg: string;
  submitSpin: boolean;
  @Input() public viewMode = 'tile';
  @Input() public index: number;
  encodedLogoUrl: string;

  constructor(
    private service: PlaylistService,
    private msgService: MessageService
  ) {
    super();
  }

  ngOnInit() {
    this.encodedLogoUrl = encodeURI(this.playlist.logoUrl);
  }

  follow() {
    this.submitSpin = true;
    this.$.push(this.service.follow(this.playlist.id).subscribe(
      result => {
        this.submitSpin = false;
        this.playlist.isFollowing = true;
        this.playlist.canFollow = false;
        this.msgService.addSnackBar('Subscription Added');
      },
      error => {
        this.submitSpin = false;
        this.errorMsg = error.error.message;
      }
    ));
  }

  unFollow() {
    this.submitSpin = true;
    this.$.push(this.service.unfollow(this.playlist.id).subscribe(
      result => {
        this.submitSpin = false;
        this.playlist.isFollowing = false;
        this.playlist.canFollow = true;
        this.msgService.addSnackBar('Subscription Removed');
      },
      error => {
        this.submitSpin = false;
        this.errorMsg = error.error.message;
      }
    ));
  }

  removeFromGroup() {
    this.$.push(this.service.groupUnfollow(this.playlist.id, this.groupId).subscribe(
      result => {
        if (result) {
          this.msgService.addSnackBar('Playlist Removed From Group');
          this.msgService.notify('group-update');
        }
      },
      error => {
        this.msgService.addSnackBar(error.error.message);
      }
    ));
  }

  featuredSash = { className: 'sash-info', icon: 'star', text: 'Featured' }
  recommendedSash = { className: 'sash-warning', icon: 'flash_on', text: 'Recommended' }
  recommendedFeaturedSash = { className: 'sash-mixed', icon: 'stars', text: 'Recommended/Featured' }

  chooseSash(playlist) {
    if (playlist.isFeatured && playlist.isRecommended) return this.recommendedFeaturedSash;
    if (playlist.isFeatured) return this.featuredSash;
    if (playlist.isRecommended) return this.recommendedSash;

    return { className: null, icon: null, text: null };
  }

  getSash(playlist) {
    var sash = this.chooseSash(playlist);

    if (sash.text != null) {
      return '<div class="sash sash-triangle-right ' + sash.className + '">' +
        '<div><i class="material-icons">' + sash.icon + '</i><span class="sash-text">' + sash.text + '</span >' +
        '</div></div>';
    }
    return '';
  }
}

