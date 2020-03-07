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
import { MessageService } from '../../../root/message.service';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'content-tile',
  templateUrl: './tile.component.html',
  styleUrls: ['./tile.component.scss']
})
export class ContentTileComponent extends BaseComponent {
  @Input() content: any;
  @Input() public group: any;
  @Input() public index: number;
  private _viewMode: string;
  @Input()
  isSelected: boolean = false;
  @Output()
  changeContent = new EventEmitter();
  @Input()
  active: boolean;
  @Input()
  activeContentId: number;
  btnDisabled: boolean = false;
  encodedLogoUrl: string;
  advanced: boolean;
  intermediate: boolean;
  beginner: boolean;
  unrated: boolean;
  difficulty: number;

  constructor(
    private service: ContentService,
    private msgService: MessageService
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

  ngOnInit() {
    this.level();
    this.encodedLogoUrl = encodeURI(this.content.logoUrl);
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

  newSash = { className: 'sash-success', icon: 'warning', text: 'New' }
  topSash = { className: '', icon: 'verified_user', text: 'Top' }
  disabledSash = { className: 'sash-danger', icon: 'remove_circle_outline', text: 'Disabled' }
  featuredSash = { className: 'sash-info', icon: 'star', text: 'Featured' }
  recommendedSash = { className: 'sash-warning', icon: 'flash_on', text: 'Recommended' }
  recommendedFeaturedSash = { className: 'sash-mixed', icon: 'stars', text: 'Recommended/Featured' }

  chooseSash(content) {

    if (content.isDisabled) return this.disabledSash;
    if (content.isFeatured && content.isRecommended) return this.recommendedFeaturedSash;
    if (content.isFeatured) return this.featuredSash;
    if (content.isRecommended) return this.recommendedSash;

    if (content.labels.indexOf('top') >= 0) return this.topSash;
    if (content.labels.indexOf('new') >= 0) return this.newSash;

    return { className: null, icon: null, text: null };
  }

  getSash(content) {
    var sash = this.chooseSash(content);

    if (sash.text != null) {
      return '<div class="sash sash-triangle-right ' + sash.className + '">' +
        '<div><i class="material-icons">' + sash.icon + '</i><span class="sash-text">' + sash.text + '</span >' +
        '</div></div>';
    }
    return '';
  }

  sendContent() {
    this.changeContent.emit(this.content);
  }

  thumb(): string {
    return (this.content.logoUrl)
      ? this.content.logoUrl
      : 'static/images/48-' + this.content.type + '.png'
  }

  addBookmark() {
    this.btnDisabled = true;
    this.$.push(this.service.addBookmark(this.content.id).subscribe(
      () => {
        this.content.isBookmarked = true;
        this.btnDisabled = false;
        this.msgService.addSnackBar(this.content.start ? "Watching Event" : "Bookmark Added");
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
        this.msgService.addSnackBar(this.content.start ? "Stopped Watching Event" : "Bookmark Removed");
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }
}

