/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Location } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, NavigationStart, Params, Router } from '@angular/router';
import { BreadcrumbService } from 'angular-crumbs';
import { switchMap } from 'rxjs/operators';
import { Converter } from 'showdown/dist/showdown';
import { AnalyticsService } from '../../../analytics/analytics.service';
import { ContentDetail, ContentDetailFlag, ContentEventCreate, ProfileDetail, Tag } from '../../../core-api-models';
import { PlaylistService } from '../../../playlist/playlist.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { SelectDialogComponent } from '../../../shared/components/select-dialog/select-dialog.component';
import { SHOWDOWN_OPTS } from '../../../shared/constants/ui-params';
import { ContentService } from '../../content.service';
import { FlagDialog } from './flag-dialog.component';

@Component({
  selector: 'content-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})
export class ContentDetailComponent extends BaseComponent implements OnInit {
  @Input()
  content: ContentDetail;
  isSidebarBrowser = false;
  collapsed: boolean;
  collapseText: string;
  showMore: boolean;
  title: string;
  converter: Converter;
  hide: boolean;
  location: Location;
  loading: boolean;
  errorMsg: string;
  comment: any;
  contentFlag: ContentDetailFlag;
  pendingFlags: ContentDetailFlag[] = [];
  historyFlags: ContentDetailFlag[] = [];
  tempFlag: string;
  btnDisabled = false;
  registerContentLaunchEvents = false;
  registerContentViewEvents = false;
  showTrailer: boolean = false;
  profile: ProfileDetail;
  isPowerUser: boolean = false;
  tags: Tag[] = [];
  niceTags: Tag[] = [];

  @Output() tagFilter = new EventEmitter();

  @Output() activeContentId = new EventEmitter();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: ContentService,
    private profileService: ProfileService,
    private elementRef: ElementRef,
    location: Location,
    private settingsSvc: SettingsService,
    private breadcrumbService: BreadcrumbService,
    private msgService: MessageService,
    private analyticsService: AnalyticsService,
    private playlistService: PlaylistService,
    public dialog: MatDialog,
  ) {
    super();
    this.location = location;
    this.converter = new Converter(SHOWDOWN_OPTS);
  }


  initProfile(p: ProfileDetail) {
    if (p) {
      this.profile = p;
      this.isPowerUser = p.isPowerUser;
      this.registerContentLaunchEvents = this.settingsSvc.settings.reporting.registerContentLaunchEvents;
      this.registerContentViewEvents = this.settingsSvc.settings.reporting.registerContentViewEvents;
      this.loadContent();
    }
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe(p => { this.initProfile(p); }));
    this.initProfile(this.profileService.profile);
    if (this.router.url.includes('/playlist')) {
      this.isSidebarBrowser = true;
    }
    this.router.events.forEach((event) => {
      if (event instanceof NavigationStart) {
        this.content = null;
        this.loading = true;
      }
    });
  }

  loadContent() {
    this.loading = true;
    this.route.params.pipe(
      switchMap((params: Params) => this.service.load(params['id'])))
      .subscribe((result) => {
        this.loading = false;
        this.content = result as ContentDetail;
        this.breadcrumbService.changeBreadcrumb(this.route.snapshot, this.content.name);
        this.content.settings = this.converter.makeHtml(this.content.settings);
        if (this.content.tags) {
          this.content.tags.forEach(element => {
            if (element.tagType !== null && element.tagType.toLowerCase() === 'nice') {
              this.niceTags.push(element);
            } else {
              this.tags.push(element);
            }
          });
        }
        this.activeContentId.emit(this.content.id);
        if (this.content.trailerUrl != null) {
          const ext = this.content.trailerUrl.substr(this.content.trailerUrl.lastIndexOf('.') + 1);
          if (ext === 'mp4' || ext === 'm4v') {
            this.showTrailer = true;
          }
        } else {
          this.showTrailer = false;
        }

        this.viewContent();

        this.pendingFlags = this.content.flags.filter(f => f.flagStatus == 'Pending');
        this.historyFlags = this.content.flags.filter(f => f.flagStatus != 'Pending');
        this.contentFlag = this.content.flags.find(f => f.profileId === this.profile.id);

      }, (err) => {
        this.loading = false;
        this.errorMsg = err.json().message;
      });
    this.getCaretText(this.showMore);
  }

  getCaretText(showing): string {
    return (showing) ? 'Hide' : 'Show';
  }

  sendTag(slug) {
    this.router.navigate(['/content'], { queryParams: { tag: slug } })
  }

  sendContentTag(type) {
    this.router.navigate(['/content'], { queryParams: { contentType: type } })
  }

  hideModal() {
    this.hide = true;
  }

  thumb(): string {
    return this.content.logoUrl;
  }

  apiUrl(): string {
    return this.service.url();
  }

  launch(): void {
    this.$.push(this.service.launch(this.content.launchUrl)
      .subscribe(result => this.settingsSvc.showTab(result.url)));
  }

  acceptFlag(flag: ContentDetailFlag): void {
    this.$.push(this.service.acceptFlag(this.content.id, flag.profileId)
      .subscribe(result => this.loadContent()));
  }

  rejectFlag(flag: ContentDetailFlag): void {
    this.$.push(this.service.rejectFlag(this.content.id, flag.profileId)
      .subscribe(result => this.loadContent()));
  }

  customUrl(): string {
    if (this.content.url) {
      const tag = `auth-hint=nextstep&contentId=${this.content.globalId}&profileId=${this.profileService.profile.globalId}`;
      return this.content.url + ((this.content.url.indexOf('?') >= 0) ? '&' : '?') + tag;
    }
    return '';
  }

  renderedDescription() {
    return this.converter.makeHtml(this.content.description);
  }

  renderedCopyright() {
    return this.converter.makeHtml(this.content.copyright);
  }

  addBookmark() {
    this.btnDisabled = true;
    this.$.push(this.service.addBookmark(this.content.id).subscribe(
      result => {
        this.content.isBookmarked = true;
        this.btnDisabled = false;
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
      result => {
        this.content.isBookmarked = false;
        this.btnDisabled = false;
        this.msgService.addSnackBar(this.content.start ? 'Stopped Watching Event' : 'Bookmark Removed');
      },
      error => {
        console.log(error.error.message);
        this.btnDisabled = false;
      }));
  }

  openFlagDialog(): void {
    const dialogRef = this.dialog.open(FlagDialog, {
      width: '400px',
      panelClass: 'flag-dialog',
      data: { id: this.content.id }
    });

    this.$.push(dialogRef.componentInstance.updateContent.subscribe((data) => {
      // display this to user to avoid reload
      this.tempFlag = data;
    }));
  }

  launchContent(): void {
    if (this.registerContentLaunchEvents === true) {
      const model: ContentEventCreate = { type: 'launch', contentId: this.content.globalId, contentName: this.content.name, contentSlug: this.content.slug, data: this.content.name };

      this.$.push(this.analyticsService.addContentEvent(model).subscribe(() => { }));
    }
  }

  viewContent(): void {
    if (this.registerContentViewEvents === true) {
      const model: ContentEventCreate = { type: 'view', contentId: this.content.globalId, contentName: this.content.name, contentSlug: this.content.slug, data: this.content.name };

      this.$.push(this.analyticsService.addContentEvent(model).subscribe(() => { }));
    }
  }

  openSelectDialog() {
    this.dialog.open(SelectDialogComponent, {
      maxHeight: '500px',
      data: {
        id: this.content.id,
        type: 'playlist'
      }
    });
  }
}

