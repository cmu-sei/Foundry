/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, ElementRef, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { ContentService } from '../../../content/content.service';
// tslint:disable-next-line:max-line-length
import { ApplicationSummary, ContentSummary, ContentType, DashboardItem, DataFilter, GroupSummary, NotificationSummary, PlaylistSummary, ProfileDetail, Tag } from '../../../core-api-models';
import { ExtensionService } from '../../../extension/extension.service';
import { GroupService } from '../../../group/group.service';
import { NotificationService } from '../../../notification/notification.service';
import { PlaylistService } from '../../../playlist/playlist.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { SystemNotificationDialog } from './system.notification.component';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'browser',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})

export class DashboardBrowserComponent extends BaseComponent implements OnInit {
  items: DashboardItem[];
  removeContentTypes: any[];
  contentTypes: any[];
  featuredContent: any[] = [];
  featuredPlaylists: any[] = [];
  featuredItems: Array<any>;
  myGroups: GroupSummary[];
  systemNotifications: NotificationSummary[];
  myPlaylists: PlaylistSummary[];
  myBookmarks: ContentSummary[];
  myApps: ApplicationSummary[];
  myEvents: ContentSummary[];
  showRemoveBookmark = [];
  showRemoveApp = [];
  tags: Tag[];
  spin = false;
  canLoadMore = true;
  bookmarksDataFilter: DataFilter = { skip: 0, take: 5, sort: '-recent', filter: 'bookmarked' };
  eventsDataFilter: DataFilter = { skip: 0, take: 5, sort: '-recent', filter: 'myevents' };
  tagSkip = 0;
  tagTake = 12;
  tagTotal: number;
  hideLoadMoreTags: boolean;
  profile: ProfileDetail;
  el: ElementRef;
  viewMode = 'tile';

  constructor(
    private contentService: ContentService,
    private groupService: GroupService,
    private playlistService: PlaylistService,
    private profileService: ProfileService,
    private notificationService: NotificationService,
    private msgService: MessageService,
    private route: ActivatedRoute,
    private router: Router,
    private settings: SettingsService,
    private extensionService: ExtensionService,
    public dialog: MatDialog,
    el: ElementRef
  ) {
    super();
    this.el = el;
  }

  addItem(title: string, dataFilter: DataFilter, queryParams: any, state, type: string) {
    const item = new DashboardItem();
    item.title = title;
    item.queryParams = queryParams;
    item.dataFilter = dataFilter;
    item.state = state;
    item.type = type;

    this.items.push(item);
  }

  onScroll() {
    if (this.canLoadMore) {
      this.initMoreDashboardItems();
    }
  }

  initProfile(p: ProfileDetail) {
    if (p) {
      this.profile = p;
      this.viewMode = this.profileService.getProfileViewMode(p, 'dashboardviewmode');
      this.initDashboard();
    }
  }

  initDashboard() {
    this.getFeaturedItems();

    this.$.push(this.groupService.list({ skip: 0, take: 5, sort: '-recent', filter: 'membership' }).subscribe(data => {
      this.myGroups = data.results as GroupSummary[];
    }));

    this.loadApps();
    this.loadBookmarks();
    this.loadEvents();
    this.loadPlaylists();
    this.loadTags();
    this.initDashboardItems();
    this.loadSystemNotifications();
  };

  ngOnInit() {
    this.items = new Array<DashboardItem>();
    this.$.push(this.profileService.profile$.subscribe(p => this.initProfile(p)));
    this.initProfile(this.profileService.profile);
  }

  getFeaturedItems() {
    this.$.push(this.contentService.getFeaturedContent({ filter: 'featured' }).subscribe(data => {
      const itemArray = [];

      this.featuredContent = data[0].results.map(c => {
        const item = {
          ...c, objectType: 'content', encodedLogo: encodeURI(c.logoUrl)

        };
        return item;
      });

      this.featuredContent.forEach(item => {
        itemArray.push(item);
      });

      this.featuredPlaylists = data[1].results.map(c => {
        const item = {
          ...c, objectType: 'playlist', encodedLogo: encodeURI(c.logoUrl)
        };
        return item;
      });

      this.featuredPlaylists.forEach(item => {
        itemArray.push(item);
      });

      this.featuredItems = itemArray.sort(function (x, y) {
        return x.featuredOrder - y.featuredOrder;
      });

    }));
  }

  setProfileViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'dashboardviewmode', viewMode);
    this.viewMode = viewMode;
  };

  loadTags() {
    this.$.push(this.contentService.listTags({ skip: this.tagSkip, take: this.tagTake, sort: 'popular' }).subscribe(data => {
      this.tags = data.results;
      this.tagTotal = data.total;
    }));
  }

  loadMoreTags() {
    this.tagTake = null;
    this.hideLoadMoreTags = true;
    this.loadTags();
  }

  updateEventsAndBookmarks() {
    this.loadEvents();
    this.loadBookmarks();
  }

  updatePlaylists() {
    this.loadPlaylists();
  }

  loadBookmarks() {
    this.$.push(this.contentService.list(this.bookmarksDataFilter).subscribe(data => {
      this.myBookmarks = data.results as ContentSummary[];
    }));
  }

  loadApps() {
    this.$.push(this.extensionService.myApps().subscribe(data => {
      this.myApps = data.results;
    }));
  }

  loadEvents() {
    this.$.push(this.contentService.list(this.eventsDataFilter).subscribe(data => {
      this.myEvents = data.results.filter(c => c.type.toString() === 'Event') as ContentSummary[];
    }));
  }

  loadPlaylists() {
    this.$.push(this.playlistService.list({ skip: 0, take: 5, sort: '-recent', filter: 'following' }).subscribe(data => {
      this.myPlaylists = data.results as PlaylistSummary[];
    }));
  }

  loadSystemNotifications() {
    this.$.push(this.notificationService.list({ skip: 0, take: 5, sort: '-recent', filter: 'type=system|unread' }).subscribe(data => {
      this.systemNotifications = data.results as NotificationSummary[];
    }));
  }

  initDashboardItems() {
    this.addItem('Courses',
      { skip: 0, take: 10, sort: '-recent', filter: 'contenttype=course' },
      { filter: 'contenttype=course' }, 'lazy', 'content');
    this.addItem('Labs',
      { skip: 0, take: 10, sort: '-recent', filter: 'contenttype=lab' },
      { filter: 'contenttype=lab' }, 'lazy', 'content');
    this.addItem('Exercises',
      { skip: 0, take: 10, sort: '-recent', filter: 'contenttype=exercise' },
      { filter: 'contenttype=exercise' }, 'lazy', 'content');
    this.addItem('New Content', { skip: 0, take: 10, sort: '-recent' }, { sort: '-recent' }, 'pending', 'content');
    this.addItem('New Playlists', { skip: 0, take: 10, sort: '-recent' }, { sort: '-recent' }, 'pending', 'playlist');
    this.loadDashboadItems();
  }

  initMoreDashboardItems() {
    this.canLoadMore = false;
    this.addItem('Top Rated',
      { skip: 0, take: 10, sort: 'top', filter: 'minratingaverage=good|minratingtotal=3' }, { sort: '-top' }, 'pending', 'content');
    this.addItem('Recommended Content',
      { skip: 0, take: 10, sort: 'top', filter: 'recommended' }, { filter: 'recommended' }, 'pending', 'content');
    this.addItem('Recommended Playlists',
      { skip: 0, take: 10, sort: 'top', filter: 'recommended' }, { filter: 'recommended' }, 'pending', 'playlist');

    this.$.push(this.contentService.contentTypes().subscribe(data => {
      this.removeContentTypes = ['Course', 'Lab', 'Exercise', 'Quiz'];
      this.contentTypes = data;

      // removing the content types loaded on init
      this.removeContentTypes.forEach(removeContentType => {
        this.contentTypes = this.contentTypes.filter(ct => ct !== removeContentType);
      });

      this.contentTypes.forEach(contentType => {
        this.addItem(contentType,
          { skip: 0, take: 10, sort: '-recent', filter: 'contenttype=' + contentType.toLowerCase() },
          { filter: 'contenttype=' + contentType.toLowerCase() }, 'lazy', 'content');
      });

      this.loadDashboadItems();
    }));
  }

  loadDashboadItems() {
    this.spin = true;
    this.items.forEach(item => {
      if (item.state === 'pending' || item.state === 'lazy') {
        item.state = 'loading';
        if (item.type === 'content') {
          this.$.push(this.contentService.list(item.dataFilter).subscribe(data => {
            item.result = data;
            item.state = 'loaded';
            this.spin = false;
          }));
        }
        if (item.type === 'playlist') {
          this.$.push(this.playlistService.list(item.dataFilter).subscribe(data => {
            item.result = data;
            item.state = 'loaded';
            this.spin = false;
          }));
        }
      }
    });
  }

  removeBookmark(content) {
    this.$.push(this.contentService.removeBookmark(content.id).subscribe(
      () => {
        if (content.type === ContentType.Event) {
          this.loadEvents();
        } else {
          this.loadBookmarks();
        }
        this.msgService.addSnackBar(content.type === ContentType.Event ? 'Stopped Watching Event' : 'Bookmark Removed');
      },
      error => console.log(error.error.message)));
  }


  removeApp(app) {
    this.$.push(this.extensionService.remove(app).subscribe(
      () => {
        this.loadApps();
        this.msgService.addSnackBar('App Removed');
      },
      error => console.log(error.error.message)));
  }

  openSystemNotificationDialog(): void {
    const dialogRef = this.dialog.open(SystemNotificationDialog, {
      width: '600px',
      panelClass: 'system-notification-dialog'
    });

    dialogRef.afterClosed().subscribe(() => { });
  }

  deleteNotification(notification: NotificationSummary): void {
    this.$.push(this.notificationService.delete(notification.id).subscribe(() => {
      this.loadSystemNotifications();
    }));
  }
}

