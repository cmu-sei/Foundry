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
import { BreadcrumbService } from 'angular-crumbs';
import { switchMap } from 'rxjs/operators';
import { Converter } from 'showdown';
import { ProfileService } from 'src/app/profile/profile.service';
import { ContentService } from '../../../content/content.service';
import { ContentSummary, DataFilter, GroupSummary, MemberRequestCreate, PlaylistSummary, ProfileDetail } from '../../../core-api-models';
import { ExtensionService } from '../../../extension/extension.service';
import { PlaylistService } from '../../../playlist/playlist.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { SHOWDOWN_OPTS } from '../../../shared/constants/ui-params';
import { GroupService } from '../../group.service';
import { InviteInputComponent } from '../invite-input/invite-input.component';
import { InviteComponent } from '../invite/invite.component';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'group-detail',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss']
})

export class GroupDetailComponent extends BaseComponent implements OnInit {
  profile: ProfileDetail;
  public group: GroupSummary;
  public contents: any[] = [];
  public playlists: PlaylistSummary[] = [];
  converter: Converter;
  lastTerm: string;

  public contentDataFilter: DataFilter = {
    skip: 0,
    term: '',
    take: 20,
    sort: '-recent',
    filter: ''
  };

  totalContent: number;
  totalPlaylists: number;

  moreContent: boolean;
  spinContent: boolean;

  contentViewMode = 'tile';
  playlistViewMode = 'tile';

  error: any;
  warning: boolean;
  btnDisabled: boolean = false;

  public forumUrl: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: GroupService,
    private contentService: ContentService,
    private playlistService: PlaylistService,
    private settingService: SettingsService,
    private profileService: ProfileService,
    private extensionService: ExtensionService,
    private msgService: MessageService,
    private breadcrumbService: BreadcrumbService,
    public dialog: MatDialog,
    public inviteDialog: MatDialog
  ) {
    super();
    this.converter = new Converter(SHOWDOWN_OPTS);
  }

  initProfile(p: ProfileDetail) {
    this.profile = p;
    this.contentViewMode = this.profileService.getProfileViewMode(p, 'contentviewmode');
    this.playlistViewMode = this.profileService.getProfileViewMode(p, 'playlistviewmode');
  }

  ngOnInit() {
    this.forumUrl = this.settingService.settings.clientSettings.urls.forumUrl;
    this.$.push(this.profileService.profile$.subscribe(p => this.initProfile(p)));

    if (this.profileService.profile) {
      this.initProfile(this.profileService.profile);
    }

    this.route.params.pipe(
      switchMap((params: Params) => this.service.load(params['id'])))
      .subscribe(result => {
        this.group = result as GroupSummary;
        this.breadcrumbService.changeBreadcrumb(this.route.snapshot, this.group.name);
        this.warning = false;
        this.contentDataFilter.filter = 'group=' + this.group.id;
        this.loadContent();
        this.loadPlaylists();
      },
        error => { this.error = error.error.message; }
      );

    this.$.push(this.msgService.listen().subscribe((m: any) => {
      if (m === 'group-update') {
        this.loadPlaylists();
      }
      if (m === 'update-status') {
        this.group.roles.member = true;
      }
    }));
  }

  loadContent() {
    this.spinContent = true;
    this.contentService.list(this.contentDataFilter)
      .subscribe(result => {
        const results = result.results as ContentSummary[];
        for (let i = 0; i < results.length; i++) {
          this.contents.push(results[i]);
        }
        this.totalContent = result.total;
        this.moreContent = (this.contentDataFilter.skip + this.contentDataFilter.take) < this.totalContent;
        this.spinContent = false;
      },
        (err) => {
          this.error = err.error.message;
          this.warning = true;
        },
        () => { this.spinContent = false; });
  }

  loadPlaylists() {
    this.playlists = [];
    this.playlistService.getGroupPlaylists(this.group.id)
      .subscribe(result => {
        for (let i = 0; i < result.results.length; i++) {
          this.playlists.push(result.results[i]);
        }
        this.totalPlaylists = result.total;
      });
  }

  showMoreContent() {
    this.contentDataFilter.skip += this.contentDataFilter.take;
    this.loadContent();
  }

  removePlaylist(id: number) {
    this.playlistService.groupUnfollow(id, this.group.id).subscribe(
      () => {
        let p = this.playlists.filter(
          (v) => {
            return v.id == id;
          }
        ).pop();
        this.playlists.splice(this.playlists.indexOf(p), 1);
      }
    )
  }

  join() {
    this.btnDisabled = true;
    const model: MemberRequestCreate = {
      accountId: this.profileService.profile.globalId,
      groupId: this.group.id,
      accountName: this.profileService.profile.name
    };
    this.service.addRequest(this.group.id, model).subscribe(result => {
      if (result) {
        this.msgService.addSnackBar('Membership Request Submitted');
        this.group.actions.join = false;
        this.btnDisabled = false;
      }
    }, error => {
      this.msgService.addSnackBar(error.error.message);
      this.btnDisabled = false;
    });
  }

  checkForRequest() {
    if (!this.group.actions.join && !this.group.roles.member) {
      return true;
    } else {
      return false;
    }
  }

  leave() {
    this.btnDisabled = true;
    this.$.push(this.service.removeMember(this.group.id, this.profileService.profile.globalId).subscribe(result => {
      if (result) {
        this.group.actions.leave = false;
        this.group.actions.join = true;
        this.btnDisabled = false;
        this.msgService.addSnackBar('Membership Removed');
      }
    }, error => {
      this.btnDisabled = false;
      this.msgService.addSnackBar(error.error.message);
    }));
  }

  renderedDescription() {
    return this.converter.makeHtml(this.group.description);
  }

  setContentViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'contentviewmode', viewMode);
    this.contentViewMode = viewMode;
  }

  setPlaylistViewMode(viewMode: string) {
    this.profileService.setProfileViewMode(this.profile, 'playlistviewmode', viewMode);
    this.playlistViewMode = viewMode;
  }

  openInviteDialog() {
    const dialogRef = this.inviteDialog.open(InviteComponent, {
      width: '600px',
      data: {
        group: this.group
      }
    });
  }

  openInviteInputDialog() {
    const dialogRef = this.inviteDialog.open(InviteInputComponent, {
      width: '600px',
      data: {
        group: this.group
      }
    });
  }
}

