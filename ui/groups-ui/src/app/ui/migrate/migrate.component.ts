/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { DataFilter, GroupCreate, GroupDetail, LegacyGroupSummary, LegacyMemberDetail, MemberCreate, MemberRequestCreate, PagedResult } from '../../models';
import { AuthService } from '../../svc/auth.service';
import { MessageService } from '../../svc/message.service';
import { SettingsService } from '../../svc/settings.service';

@Component({
  selector: 'migrate',
  templateUrl: './migrate.component.html',
  styleUrls: ['./migrate.component.scss']
})
export class MigrateComponent implements OnInit {
  error: any;
  accountId: string;
  migrating: boolean;
  verified: boolean;
  pagedResult: PagedResult<LegacyGroupSummary>;
  total: number;

  groupIndex: number = 0;
  migrateGroups: LegacyGroupSummary[] = [];

  public dataFilter: DataFilter = {
    skip: 0,
    take: 5,
    term: '',
    sort: '-recent',
    filter: ''
  };

  constructor(
    private http: HttpClient,
    private authSvc: AuthService,
    private settingsSvc: SettingsService,
    private messageService: MessageService
  ) { }

  ngOnInit() {
    this.accountId = this.authSvc.currentUser.profile.sub;

    this.search();
  }

  marketUrl() {
    return this.settingsSvc.settings.urls.marketUrl;
  }

  coreUrl() {
    return this.settingsSvc.settings.urls.coreUrl;
  }

  public search() {
    this.verified = false;
    this.migrating = false;

    this.http.get<PagedResult<LegacyGroupSummary>>(this.marketUrl() + '/groups' + this.authSvc.queryStringify(this.dataFilter, '?')).subscribe(data => {
      this.pagedResult = data;
    });
  }

  reset() {
    this.search();
  }

  public verifyGroup(legacyGroup: LegacyGroupSummary) {
    this.http.get<GroupDetail>(this.coreUrl() + '/group/' + legacyGroup.globalId).subscribe(
      result => {
        legacyGroup.group = result;
        legacyGroup.verified = true;
        legacyGroup.migrated = legacyGroup.group != null;
      }, error => {
        this.messageService.addSnackBar('Verify Failed');
      });
  }

  public verifyAll() {
    var url = this.coreUrl();
    var http = this.http;
    var component = this;

    let count: number = 0;

    this.pagedResult.results.forEach(function (legacyGroup: LegacyGroupSummary) {
      http.get<GroupDetail>(url + '/group/' + legacyGroup.globalId).subscribe(
        result => {
          count++;
          legacyGroup.group = result;
          legacyGroup.verified = true;
          legacyGroup.migrated = legacyGroup.group != null;
          if (count == component.pagedResult.results.length) {
            component.verified = true;
          }
        }, error => {
          this.messageService.addSnackBar('Verify All Failed');
        });
    });
  }

  public migrateAll() {
    this.groupIndex = 0;
    this.migrateGroups = this.pagedResult.results;
    this.migrating = true;
    this.processGroup();
  }

  public migrateGroup(legacyGroup: LegacyGroupSummary) {
    this.groupIndex = 0;
    this.migrateGroups = [legacyGroup];
    this.migrating = true;
    this.processGroup();
  }

  public processGroup() {
    let legacyGroup: LegacyGroupSummary = this.migrateGroups[this.groupIndex];

    legacyGroup.migrating = true;
    if (legacyGroup.group) {
      this.processMembers(legacyGroup);
    }
    else {
      let create: GroupCreate = {
        id: legacyGroup.globalId,
        description: legacyGroup.description,
        logoUrl: legacyGroup.logoUrl,
        name: legacyGroup.name,
        summary: legacyGroup.summary
      };

      this.http.post<GroupDetail>(this.coreUrl() + '/groups', create).subscribe(
        result => {
          legacyGroup.group = result;
          this.messageService.addSnackBar('Group Created');
          this.processMembers(legacyGroup);
        },
        error => {
          this.messageService.addSnackBar('Group Create Failed');
          this.processNextGroup(legacyGroup);
        });
    }
  }

  public processMembers(legacyGroup: LegacyGroupSummary) {
    this.http.get<PagedResult<LegacyMemberDetail>>(this.marketUrl() + "/group/" + legacyGroup.id + "/members").subscribe(
      result => {
        this.processMember(legacyGroup, 0, result.results);
      });
  };

  public processMember(legacyGroup: LegacyGroupSummary, index: number, members: LegacyMemberDetail[]) {
    let legacyMember: LegacyMemberDetail = members[index];

    if (legacyMember.needsApproval) {
      let create: MemberRequestCreate = { accountId: legacyMember.profileGlobalId, groupId: legacyGroup.group.id, accountName: legacyMember.profileName };

      this.http.post<MemberRequestCreate>(this.coreUrl() + '/group/' + legacyGroup.group.id + '/member-requests', create).subscribe(
        result => {
          index++
          this.messageService.addSnackBar('Member Request Created');
          if (index < members.length) { this.processMember(legacyGroup, index, members); }
          else { this.processNextGroup(legacyGroup); }
        },
        error => {
          index++
          this.messageService.addSnackBar('Member Request Create Failed');
          if (index < members.length) { this.processMember(legacyGroup, index, members); }
          else { this.processNextGroup(legacyGroup); }
        });
    }
    else {
      let create: MemberCreate = { accountId: legacyMember.profileGlobalId, accountName: legacyMember.profileName, groupId: legacyGroup.group.id, isOwner: legacyMember.isOwner };

      this.http.post<MemberCreate>(this.coreUrl() + '/group/' + legacyGroup.group.id + '/members', create).subscribe(
        result => {
          index++
          this.messageService.addSnackBar('Member Created');
          if (index < members.length) { this.processMember(legacyGroup, index, members); }
          else { this.processNextGroup(legacyGroup); }
        },
        error => {
          index++
          this.messageService.addSnackBar('Member Create Failed');
          if (index < members.length) { this.processMember(legacyGroup, index, members); }
          else { this.processNextGroup(legacyGroup); }
        });
    }
  }

  public processNextGroup(legacyGroup: LegacyGroupSummary) {
    legacyGroup.migrated = legacyGroup.group != null;
    legacyGroup.migrating = false;

    this.updateListeners();

    this.groupIndex++;
    if (this.groupIndex < this.migrateGroups.length) {
      this.processGroup();
    }
    else {
      this.migrating = false;
    }
  }

  updateListeners() {
    this.messageService.notify('group-update');
  }
}

