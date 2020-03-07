/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ContentService } from '../../../content/content.service';
import { PlaylistService } from '../../../playlist/playlist.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'dashboard-tile',
  templateUrl: './tile.component.html',
  styleUrls: ['./tile.component.scss']
})
export class DashboardTileComponent extends BaseComponent {
  @Input() content: any;
  @Input() public group: any;
  @Input() public index: number;
  @Input() public itemType: string;
  private _viewMode: string;
  @Input()
  isSelected = false;
  @Output()
  changeContent = new EventEmitter();
  @Output()
  updateBookmarks: EventEmitter<any> = new EventEmitter();
  @Output()
  updatePlaylists: EventEmitter<any> = new EventEmitter();
  @Input()
  active: boolean;
  @Input()
  activeContentId: number;
  contentVisible = false;
  advanced: boolean;
  intermediate: boolean;
  beginner: boolean;
  unrated: boolean;
  difficulty: number;
  btnDisabled = false;
  encodedLogoUrl: string;
  encodedHoverUrl: string;

  constructor(
    private service: ContentService,
    private msgService: MessageService,
    private playlistService: PlaylistService,
    private router: Router
  ) {
    super();
  }

  get viewMode(): string {
    return this._viewMode;
  }

  @Input()
  set viewMode(viewMode: string) {
    this._viewMode = viewMode;
  }

  // tslint:disable-next-line:use-life-cycle-interface
  ngOnInit() {
    if (this.itemType === 'content') {
      this.level();
    }
    this.encodedLogoUrl = encodeURI(this.content.logoUrl);
    this.encodedHoverUrl = encodeURI(this.content.hoverUrl);
  }

  level(): any {
    if ((this.content.difficulty.beginner > this.content.difficulty.intermediate) &&
      (this.content.difficulty.beginner > this.content.difficulty.advanced)) {
      this.beginner = true;
    }
    if ((this.content.difficulty.intermediate > this.content.difficulty.beginner) &&
      (this.content.difficulty.intermediate > this.content.difficulty.advanced)) {
      this.intermediate = true;
    }
    if ((this.content.difficulty.advanced > this.content.difficulty.intermediate) &&
      (this.content.difficulty.advanced > this.content.difficulty.beginner)) {
      this.advanced = true;
    }
  }

  sendContent() {
    this.changeContent.emit(this.content);
  }

  thumb(): string {
    return (this.content.logoUrl)
      ? this.content.logoUrl
      : 'static/images/48-' + this.content.type + '.png';
  }

  addBookmark() {
    this.btnDisabled = true;
    this.$.push(this.service.addBookmark(this.content.id).subscribe(
      () => {
        this.content.isBookmarked = true;
        this.btnDisabled = false;
        this.updateBookmarks.emit(true);
        this.msgService.addSnackBar(this.content.start ? 'Watching Event' : 'Bookmark Added');
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }

  removeBookmark() {
    this.btnDisabled = true;
    this.$.push(this.service.removeBookmark(this.content.id).subscribe(
      () => {
        this.content.isBookmarked = false;
        this.btnDisabled = false;
        this.updateBookmarks.emit(true);
        this.msgService.addSnackBar(this.content.start ? 'Stopped Watching Event' : 'Bookmark Removed');
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }

  followPlaylist() {
    this.btnDisabled = true;
    this.$.push(this.playlistService.follow(this.content.id).subscribe(
      () => {
        this.content.isFollowing = true;
        this.btnDisabled = false;
        this.updatePlaylists.emit(true);
        this.msgService.addSnackBar('Subscription Added');
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }

  unFollowPlaylist() {
    this.btnDisabled = true;
    this.$.push(this.playlistService.unfollow(this.content.id).subscribe(
      () => {
        this.content.isFollowing = false;
        this.btnDisabled = false;
        this.updatePlaylists.emit(true);
        this.msgService.addSnackBar('Subscription Removed');
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }

  navigateToItem(type) {
    this.router.navigate([`/${type}`, this.content.id, this.content.slug]);
  }

}

