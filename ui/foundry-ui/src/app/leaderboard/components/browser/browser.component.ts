/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component } from '@angular/core';
import { AnalyticsService } from '../../../analytics/analytics.service';
import { ContentService } from '../../../content/content.service';
import { ContentEventSummary, ContentSummary, DataFilter, GroupSummary, LeaderboardSummary, LeaderValue, PagedResult, PlaylistSummary, ProfileInfo, ProfileSummary } from '../../../core-api-models';
import { GroupService } from '../../../group/group.service';
import { PlaylistService } from '../../../playlist/playlist.service';
import { ProfileService } from '../../../profile/profile.service';
import { SettingsService } from '../../../root/settings.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class LeaderboardBrowserComponent extends BaseComponent {
  sponsors: PagedResult<GroupSummary>;
  authors: PagedResult<ProfileSummary>;
  contributors: PagedResult<ProfileSummary>;
  contents: PagedResult<ContentSummary>;
  playlists: PagedResult<PlaylistSummary>;
  exercises: Array<LeaderboardSummary> = new Array<LeaderboardSummary>();
  exerciseMessage: string = "";
  exerciseResultTotal: number = 0;
  displaySTEPLeaderboardData: boolean = false;
  displayLRSData: boolean = false;
  take: number = 5;
  skip: number = 0;
  minimumRating = 'Good';
  minimumTotal = 1;
  contentLaunches: Array<ContentEventSummary>;
  contentLaunchesCount: number = 0;
  profiles = new Array<ProfileCache>();

  constructor(
    private contentService: ContentService,
    private profileService: ProfileService,
    private groupService: GroupService,
    private playlistService: PlaylistService,
    private analyticsService: AnalyticsService,
    private settingsService: SettingsService
  ) {
    super();
  }

  ngOnInit(): void {
    this.displaySTEPLeaderboardData = this.settingsService.settings.reporting.displaySTEPLeaderboardData;
    this.displayLRSData = this.settingsService.settings.reporting.displayLRSData;
    this.list();
  }

  hasProfile(guid: string) {
    return this.profiles.find(p => p.id === guid);
  }

  getProfile(guid: string) {
    return this.hasProfile(guid).info;
  }

  list() {
    var topRatedDataFilter = { skip: this.skip, take: this.take, sort: 'top', filter: 'minratingaverage=' + this.minimumRating + '|minratingtotal=' + this.minimumTotal };

    this.contentService.list(topRatedDataFilter).subscribe(result => {
      this.contents = result;
    });

    this.groupService.list(topRatedDataFilter).subscribe(result => {
      this.sponsors = result;
    });

    this.playlistService.list(topRatedDataFilter).subscribe(result => {
      this.playlists = result;
    });

    this.profileService.list(topRatedDataFilter).subscribe(result => {
      this.authors = result;
    });

    this.profileService.list({ skip: this.skip, take: this.take, sort: 'contributions' }).subscribe(result => {
      this.contributors = result;
    });

    if (this.displaySTEPLeaderboardData) {
      var reportIds: Array<string> = this.settingsService.settings.reporting.exerciseLeaderboardIds;

      if (reportIds != null) {
        for (var i = 0; i < reportIds.length; i++) {
          this.$.push(this.analyticsService.exerciseLeaderboard(reportIds[i], this.settingsService.settings.reporting.exerciseLeaderboardMaxResultsCount).subscribe(result => {
            if (result.Message != null) {
              this.exerciseMessage = result.Message;
            }
            else {
              const list: LeaderboardSummary = result;
              list.LeaderValues.forEach(
                (leader: LeaderValue) => {
                  if (leader.OAuthId) {
                    this.profileService.getProfileInfo(leader.OAuthId).subscribe(
                      (info: ProfileInfo) => {
                        this.profiles.push({ id: leader.OAuthId, info: info });
                      }
                    );
                  }
                }
              );
              this.exercises.push(list);
              this.exerciseResultTotal += list.LeaderValues.length;
            }
          }));
        }
      }
    }

    if (this.displayLRSData) {
      const model: DataFilter = {
        sort: 'created',
        skip: 0,
        take: 10,
        filter: 'type=launch'
      };
      this.$.push(this.analyticsService.getAllContentEvents(model).subscribe(result => {
        this.contentLaunches = result.results;
        this.contentLaunchesCount = this.contentLaunches.length;
      }));
    }
  }
}

interface ProfileCache {
  id?: string;
  info?: ProfileInfo;
}

