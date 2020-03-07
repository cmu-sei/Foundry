/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

export class PagedResult<T> {
  total?: number;
  results: T[];
  dataFilter?: DataFilter;
}

export interface DataFilter {
  term?: string;
  skip?: number;
  take?: number;
  sort?: string;
  filter?: string;
}

export class FileDetail {
  id: number;
  name: string;
  slug: string;
  globalId: string;
  extension: string;
  length: number;
  url: string;
  contentType: string;
  bucketName: string;
  bucketId: number;
  versionNumber: number;
  tags: Array<string>;
  access?: Array<string>;
  createdByName?: string;
  created: Date;
}


/** Groups **/
export interface GroupDetail {
  id?: string;
  name?: string;
  logoUrl?: string;
  slug?: string;
  description?: string;
  summary?: string;
  parentId?: string;
  parentName?: string;
  parentSlug?: string;
  actions?: GroupActions;
  roles?: GroupRoles;
  counts?: GroupCounts;
  updated?: string;
  created?: string;
}

export interface GroupSummary {
  id?: string;
  name?: string;
  logoUrl?: string;
  slug?: string;
  description?: string;
  summary?: string;
  parentId?: string;
  parentName?: string;
  parentSlug?: string;
  actions?: GroupActions;
  roles?: GroupRoles;
  counts?: GroupCounts;
  updated?: string;
  created?: string;
}

export interface GroupActions {
  edit?: boolean;
  delete?: boolean;
  join?: boolean;
  leave?: boolean;
}

export interface GroupCounts {
  members?: number;
  memberRequests?: number;
  groupRequests?: number;
  children?: number;
}

export interface GroupRoles {
  member?: boolean;
  manager?: boolean;
  owner?: boolean;
}

export interface GroupUpdate {
  id?: string;
  name?: string;
  logoUrl?: string;
  description?: string;
  summary?: string;
  parentId?: string;
}

export interface GroupCreate {
  id?: string;
  name?: string;
  logoUrl?: string;
  description?: string;
  summary?: string;
  parentId?: string;
}

export interface TreeGroupSummary {
  id?: string;
  name?: string;
  slug?: string;
  key?: string;
  children?: Array<TreeGroupSummary>;
}

/** Member Requests **/
export interface MemberRequestCreate {
  accountId?: string;
  accountName?: string;
  groupId?: string;
}

export interface MemberRequestUpdate {
  accountId?: string;
  groupId?: string;
  status?: MemberRequestStatus;
}

export interface MemberRequestDetail {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  created?: string;
  updated?: string;
  actions?: MemberRequestActions;
  status?: MemberRequestStatus;
}

export interface MemberRequestActions {
  edit?: boolean;
  delete?: boolean;
}

export enum MemberRequestStatus {
  Pending = <any>'Pending',
  Approved = <any>'Approved',
  Denied = <any>'Denied'
}

/** Group Requests **/
export interface GroupRequestCreate {
  parentGroupId?: string;
  childGroupId?: string;
}

export interface GroupRequestUpdate {
  parentGroupId?: string;
  childGroupId?: string;
  status?: GroupRequestStatus;
}

export interface GroupRequestDetail {
  parentGroupId?: string;
  parentGroupName?: string;
  parentGroupSlug?: string;
  childGroupId?: string;
  childGroupName?: string;
  childGroupSlug?: string;
  created?: string;
  updated?: string;
  actions?: GroupRequestActions;
  status?: GroupRequestStatus;
}

export enum GroupRequestStatus {
  Pending = <any>'Pending',
  Approved = <any>'Approved',
  Denied = <any>'Denied'
}

export interface GroupRequestActions {
  edit?: boolean;
  delete?: boolean;
}

/** Members **/
export interface MemberDetail {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  isOwner?: boolean;
  isManager?: boolean;
  actions?: MemberActions;
}

export interface MemberSummary {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  isOwner?: boolean;
  isManager?: boolean;
  created: Date;
  actions?: MemberActions;
}

export interface MemberActions {
  edit?: boolean;
  delete?: boolean;
}

export interface MemberCreate {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  isOwner?: boolean;
  isManager?: boolean;
}

export interface MemberUpdate {
  accountId?: string;
  groupId?: string;
  isOwner?: boolean;
  isManager?: boolean;
}

/** Accounts **/

export interface AccountDetail {
  id?: string;
  name?: string;
  slug?: string;
  isAdministrator?: boolean;
  created?: Date;
  updated?: Date;
  actions?: AccountActions;
}

export interface AccountSummary {
  id?: string;
  name?: string;
  slug?: string;
  isAdministrator?: boolean;
  created?: Date;
  updated?: Date;
  actions?: AccountActions;
}

export interface AccountActions {
  edit?: boolean;
  delete?: boolean;
}

/** Migrate **/

export interface LegacyGroupSummary {
  id?: number;
  globalId?: string;
  name?: string;
  slug?: string;
  summary?: string;
  description?: string;
  logoUrl?: string;
  thumbnailUrl?: string;
  created?: Date;
  updated?: Date;
  memberCount?: number;
  group?: GroupDetail;
  verified?: boolean;
  migrating?: boolean;
  migrated?: boolean;
}

export interface LegacyMemberDetail {
  id?: number;
  permissions?: LegacyGroupPermission;
  groupId?: number;
  groupGlobalId?: string;
  groupName?: string;
  profileId?: number;
  profileName?: string;
  profileGlobalId?: string;
  needsApproval?: boolean;
  canManage?: boolean;
  hasRunReports?: boolean;
  hasEditGroups?: boolean;
  hasManageMembers?: boolean;
  isOwner?: boolean;
}

export enum LegacyGroupPermission {
  None = <any>'None',
  Member = <any>'Member',
  ManageMembers = <any>'Manage_Members',
  RunReports = <any>'Run_Reports',
  Edit = <any>'Edit',
  Owner = <any>'Owner'
}

