/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AnalyticsService } from '../../../analytics/analytics.service';
// tslint:disable-next-line:max-line-length
import { AnalyticsEventSummary, ContentSummary, DataFilter, GroupSummary, PagedResult, PlaylistSummary, PostDetail, ProfileDetail, ProfileInfo } from '../../../core-api-models';
import { PostsService } from '../../../posts/posts.service';
import { SelectDialogComponent } from '../../../shared/components/select-dialog/select-dialog.component';
import { ProfileService } from '../../profile.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'profile-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})
export class ProfileDetailComponent extends BaseComponent implements OnInit {
  profile: ProfileDetail;
  profileInfo: ProfileInfo;
  activities: Array<any> = [];
  public playlists: PlaylistSummary[] = [];
  public content: any[] = [];
  public groups: GroupSummary[] = [];
  public managedGroups: GroupSummary[];
  public posts: PagedResult<PostDetail>;
  public postsDataFilter: DataFilter = { skip: 0, take: 10, sort: '-recent' };
  statements: any[] = [];
  term: string;
  isSelf = true;
  profile$: Subscription;
  addUiFlag: boolean[] = [false, false, false];
  showActivity = true;
  formErrors = ['', '', ''];
  errorMsg: string;
  navId: number;
  profileUrl: string;
  selectedGroupId?: any = null;
  groupsVisible: boolean;

  totalGroups: number;
  totalContent: number;
  totalPlaylists: number;

  contentViewMode = 'tile';
  playlistViewMode = 'tile';
  groupViewMode = 'tile';

  public groupText = 'This profile has not created any groups.';
  public playlistText = 'This profile has not created any playlists.';
  public contentText = 'This profile has not created any content.';
  public postText = 'This profile has not created any posts.';

  public contentDataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '-recent',
    filter: ''
  };

  public playlistDataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '',
    filter: ''
  };

  public groupDataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '',
    filter: ''
  };

  moreGroups: boolean;
  moreContent: boolean;
  morePlaylists: boolean;
  spinGroups: boolean;
  spinContent: boolean;
  spinPlaylists: boolean;

  constructor(
    private route: ActivatedRoute,
    private profileService: ProfileService,
    private postsService: PostsService,
    private analyticsService: AnalyticsService,
    public dialog: MatDialog
  ) {
    super();
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.contentViewMode = this.profileService.getProfileViewMode(p, 'contentviewmode');
    this.playlistViewMode = this.profileService.getProfileViewMode(p, 'playlistviewmode');
    this.groupViewMode = this.profileService.getProfileViewMode(p, 'groupviewmode');

    this.isSelf = this.route.snapshot.url.length === 0
    || this.profile.id === +this.route.snapshot.url[0];
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe(p => this.initProfile(p)));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    this.navId = 0;

    if (this.isSelf) {
      this.groupText = 'You have not created any groups.';
      this.playlistText = 'You have not created any playlists.';
      this.contentText = 'You have not created any content.';
      this.postText = 'You have not created any posts.';
    }

    this.$.push(this.route.params.pipe(
      switchMap((params: Params) => this.profileService.loadById(params['id'] || 0)))
      .subscribe(result => {
        this.profile = result as ProfileDetail;
        this.profileUrl = this.profileService.getProfileUrl();
        this.loadContent();
        this.loadProfileInfo();
        this.loadPlaylists();
        this.loadGroups();
        this.loadPosts();
        this.loadActivities();
      }));
  }

  defaultImage() {
    return this.profileService.defaultImageUri;
  }

  loadPosts() {
    this.$.push(this.postsService.list(this.profile.id, this.postsDataFilter).subscribe((data: PagedResult<PostDetail>) => {
      this.posts = data;

      this.posts.results.forEach(p => {
        this.loadReplies(p);
      });
    }));
  }

  loadActivities() {
    this.$.push(this.analyticsService.getAll().subscribe((data: AnalyticsEventSummary[]) => {
      this.activities = data;
    }));
  }

  loadReplies(post: PostDetail) {
    this.$.push(this.postsService.listReplies(post.id, this.postsDataFilter).subscribe((data: PagedResult<PostDetail>) => {
      post.replies = data.results;
    }));
  }

  loadProfileInfo() {
    this.$.push(this.profileService.getProfileInfo(this.profile.globalId).subscribe((data: ProfileInfo) => {
      this.profileInfo = data;
    }));
  }

  loadPlaylists() {
    this.spinPlaylists = true;
    this.$.push(this.profileService.playlists(this.profile.id, this.playlistDataFilter).subscribe((data: PagedResult<PlaylistSummary>) => {
      const results = data.results as any[];
      for (let i = 0; i < results.length; i++) {
        this.playlists.push(results[i]);
      }
      this.totalPlaylists = data.total;
      this.morePlaylists = (this.playlistDataFilter.skip + this.playlistDataFilter.take) < this.totalPlaylists;
      this.spinPlaylists = false;
    }));
  }

  loadContent() {
    this.spinContent = true;
    this.$.push(this.profileService.content(this.profile.id, this.contentDataFilter).subscribe((data: PagedResult<ContentSummary>) => {
      const results = data.results as any[];
      for (let i = 0; i < results.length; i++) {
        this.content.push(results[i]);
      }
      this.totalContent = data.total;
      this.moreContent = (this.contentDataFilter.skip + this.contentDataFilter.take) < this.totalContent;
      this.spinContent = false;
    }));
  }

  loadGroups() {
    this.spinGroups = true;
    this.$.push(this.profileService.memberships(this.profile.globalId, this.groupDataFilter).subscribe((data: PagedResult<GroupSummary>) => {
      const results = data.results as any[];
      for (let i = 0; i < results.length; i++) {
        this.groups.push(results[i]);
      }
      this.totalGroups = data.total;
      this.moreGroups = (this.groupDataFilter.skip + this.groupDataFilter.take) < this.totalGroups;
      this.spinGroups = false;
    }));
  }

  showMoreContent() {
    this.contentDataFilter.skip += this.contentDataFilter.take;
    this.loadContent();
  }

  showMorePlaylists() {
    this.playlistDataFilter.skip += this.playlistDataFilter.take;
    this.loadPlaylists();
  }

  showMoreGroups() {
    this.groupDataFilter.skip += this.groupDataFilter.take;
    this.loadGroups();
  }

  addToGroup() {
    // this.groupService.addMember(this.selectedGroupId, this.profile.globalId).subscribe(
    //   result => {
    //     this.selectedGroupId = null;
    //     this.msgService.addSnackBar('Profile has joined group');
    //     this.loadGroups();
    //   },
    //   error => {
    //     this.errorMsg = error.error.message;
    //   }
    // );
  }

  toggleAddUi(i: number) {
    this.addUiFlag[i] = !this.addUiFlag[i];
    this.formErrors[i] = '';
  }

  openSelectDialog() {
    const dialogRef = this.dialog.open(SelectDialogComponent, {
      maxHeight: '500px',
      data: {
        id: this.profile.id,
        type: 'profile'
       }
    });
  }

  setContentViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'contentviewmode', viewMode);
    this.contentViewMode = viewMode;
  }

  setPlaylistViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'playlistviewmode', viewMode);
    this.playlistViewMode = viewMode;
  }

  setGroupViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'groupviewmode', viewMode);
    this.groupViewMode = viewMode;
  }

}

