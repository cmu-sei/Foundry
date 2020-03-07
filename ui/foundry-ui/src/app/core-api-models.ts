/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

export interface MosXApiActorsActor {
  objectType?: string;
  name?: string;
  identifier?: MosXApiInverseFunctionalIdentifiersIInverseFunctionalIdentifier;
}

export interface MosXApiActorsAgent {
  objectType?: string;
  name?: string;
  identifier?: MosXApiInverseFunctionalIdentifiersIInverseFunctionalIdentifier;
}

export interface MosXApiActorsGroup {
  objectType?: string;
  name?: string;
  identifier?: MosXApiInverseFunctionalIdentifiersIInverseFunctionalIdentifier;
  member?: Array<MosXApiActorsAgent>;
}

export interface MosXApiAttachment {
  usageType?: string;
  display?: { [key: string]: string; };
  contentType?: string;
  length?: number;
  sha2?: string;
  description?: { [key: string]: string; };
  fileUrl?: string;
}

export interface ImportResult {
  messages?: Array<string>;
  errors?: Array<string>;
}

export interface MosXApiContext {
  registration?: string;
  instructor?: MosXApiActorsActor;
  team?: MosXApiActorsGroup;
  contextActivities?: MosXApiContextActivities;
  revision?: string;
  platform?: string;
  language?: string;
  statement?: MosXApiObjectsStatementReference;
  extensions?: { [key: string]: string; };
}

export interface MosXApiContextActivities {
  parent?: Array<MosXApiObjectsActivity>;
  grouping?: Array<MosXApiObjectsActivity>;
  category?: Array<MosXApiObjectsActivity>;
  other?: Array<MosXApiObjectsActivity>;
}

export interface MosXApiInverseFunctionalIdentifiersIInverseFunctionalIdentifier {
}

export interface MosXApiObjectsActivity {
  objectType?: string;
  id?: string;
  definition?: MosXApiObjectsActivityDefinition;
}

export interface MosXApiObjectsActivityDefinition {
  name?: { [key: string]: string; };
  description?: { [key: string]: string; };
  type?: string;
  moreInfo?: string;
  extensions?: { [key: string]: string; };
}

export interface MosXApiObjectsStatementObject {
}

export interface MosXApiObjectsStatementReference {
  objectType?: string;
  id?: string;
}

export interface MosXApiResult {
  score?: MosXApiScore;
  success?: boolean;
  completion?: boolean;
  response?: string;
  duration?: string;
  extensions?: { [key: string]: string; };
}

export interface MosXApiScore {
  scaled: number;
  raw?: number;
  min?: number;
  max?: number;
}

export interface MosXApiStatement {
  id?: string;
  actor: MosXApiActorsActor;
  verb: MosXApiVerb;
  object: MosXApiObjectsStatementObject;
  result?: MosXApiResult;
  context?: MosXApiContext;
  timestamp?: Date;
  stored?: Date;
  authority?: MosXApiActorsActor;
  attachments?: Array<MosXApiAttachment>;
}

export interface MosXApiVerb {
  id?: string;
  display?: { [key: string]: string; };
}

export interface Achievement {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  whenCreated?: Date;
  actorId?: string;
  actionId?: string;
  contentId?: string;
  badgeUrl?: string;
}

export interface Comment {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  whenCreated?: Date;
  parentId?: number;
  parent?: Comment;
  discussionId?: number;
  discussion?: Discussion;
  profileId?: number;
  profile?: ProfileDetail;
  text?: string;
  votes?: Array<CommentVote>;
}

export interface CommentVote {
  commentId?: number;
  profileId?: number;
  value?: number;
}

export enum ContentType {
  Challenge = <any>'Challenge',
  Course = <any>'Course',
  Curriculum = <any>'Curriculum',
  Document = <any>'Document',
  Event = <any>'Event',
  Exercise = <any>'Exercise',
  Game = <any>'Game',
  Image = <any>'Image',
  Lab = <any>'Lab',
  Quiz = <any>'Quiz',
  Simulation = <any>'Simulation',
  Video = <any>'Video',
  Webpage = <any>'Webpage'
}

export interface ContentFilterType {
  name?: string;
  displayName?: string;
}

export interface ContentTag {
  id?: number;
  contentId?: number;
  content?: ContentSummary;
  tagId?: number;
  tag?: Tag;
}

export interface Discussion {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  whenCreated?: Date;
  type?: DiscussionType;
  comments?: Array<Comment>;
  status?: DiscussionStatus;
  contentId?: number;
  content?: ContentSummary;
}

export enum DiscussionType {
  ContentReview = <any>'ContentReview'
}

export enum DiscussionStatus {
  Open = <any>'Open',
  Closed = <any>'Closed'
}

export interface ProfileFollower {
  profileId?: number;
  profileName?: string;
}

export interface GroupFollower {
  groupId?: number;
  groupName?: string;
}

export interface Membership {
  id?: number;
  order?: number;
  groupId?: number;
  permissions?: MembershipPermissions;
  profileId?: number;
  group?: GroupDetail;
  profile?: ProfileDetail;
}

export interface MemberRequestDetail {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  created?: string;
  updated?: string;
  access?: GroupAccess;
  status?: MemberRequestStatus;
}

export interface MemberRequestCreate {
  accountId?: string;
  groupId?: string;
  accountName?: string;
}

export interface MemberRequestUpdate {
  accountId?: string;
  groupId?: string;
  status?: MemberRequestStatus;
}

export enum MemberRequestStatus {
  Pending = <any>'Pending',
  Approved = <any>'Approved',
  Denied = <any>'Denied'
}

export enum MembershipPermissions {
  None = <any>'None',
  Member = <any>'Member',
  ManageMembers = <any>'Manage_Members',
  RunReports = <any>'Run_Reports',
  Edit = <any>'Edit',
  Owner = <any>'Owner'
}

export interface PlaylistDetail {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  summary?: string;
  created?: Date;
  isDefault?: boolean;
  canFollow?: boolean;
  isPublic?: boolean;
  isFeatured?: boolean;
  isRecommended?: boolean;
  isOwner?: boolean;
  canEdit?: boolean;
  isFollowing?: boolean;
  logoUrl?: string;
  trailerUrl?: string;
  contentCount?: number;
  sectionCount?: number;
  profileFollowerCount?: number;
  groupFollowerCount?: number;
  sections?: Array<PlaylistDetailSection>;
  profileFollowers?: Array<ProfileFollower>;
  groupFollowers?: Array<GroupFollower>;
  userRating?: Rating;
  rating?: PlaylistRating;
  keyValues?: Array<KeyValue>;
  tags?: Array<PlaylistSummaryTag>;
  featuredOrder?: number;
  copyright?: string;
  profileId?: number;
  profileName?: string;
  profileSlug?: string;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  publisherThumbnailUrl?: string;
}

export interface PlaylistDetailSection {
  order?: number;
  name?: string;
  contents?: Array<PlaylistDetailSectionContent>;
}

export interface PlaylistDetailSectionContent {
  id?: number;
  name?: string;
  slug?: string;
  description?: string;
  url?: string;
  logoUrl?: string;
  order?: number;
  type?: ContentType;
  publisherId?: string;
  publisherName?: string;
  tags?: Array<PlaylistDetailSectionContentTag>;
  created?: Date;
}

export interface PlaylistDetailSectionContentTag {
  name?: string;
  slug?: string;
}

export interface PlaylistSectionUpdate {
  name?: string;
  contentIds?: Array<number>;
}

export interface ProfileDetailGroup {
  id?: number;
  name?: string;
  slug?: string;
}

export interface ProfileDetailPlaylist {
  id?: number;
  name?: string;
  slug?: string;
}

export interface ProfileDetail {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  whenCreated?: Date;
  organization?: string;
  permissions?: ProfilePermissions;
  groups?: Array<ProfileDetailGroup>;
  playlists?: Array<ProfileDetailPlaylist>;
  keyValues?: Array<KeyValue>;
  isAdministrator?: boolean;
  isPowerUser?: boolean;
  isDisabled?: boolean;
  canManage?: boolean;
}

export interface ProfileInfo {
  name?: string;
  biography?: string;
  avatar?: string;
  organizationLogo?: string;
  organizationUnitLogo?: string;
  updatedAt?: string;
}

export interface ProfileSummary {
  id?: number;
  globalId?: string;
  name?: string;
  slug?: string;
  description?: string;
  organization?: string;
  membershipCount?: number;
  playlistCount?: number;
  authoredCount?: number;
  contributionCount?: number;
  keyValues?: Array<KeyValue>;
  isAdministrator?: boolean;
  isPowerUser?: boolean;
  isDisabled?: boolean;
  canManage?: boolean;
}

export enum ProfilePermissions {
  None = <any>'None',
  PowerUser = <any>'PowerUser',
  Administrator = <any>'Administrator'
}

export interface RatingMetricDetail {
  poor?: number,
  fair?: number,
  good?: number,
  great?: number,
  total?: number
}

export interface ContentDifficulty {
  basic?: number,
  intermediate?: number,
  advanced?: number,
  total?: number
}

export interface PlaylistRating {
  poor?: number,
  fair?: number,
  good?: number,
  great?: number,
  total?: number
}

export interface Tag {
  id?: number;
  name?: string;
  slug?: string;
  description?: string;
  tagType?: string;
  tagSubType?: string;
}

export interface TagUpdate {
  id?: number;
  name?: string;
  description?: string;
  tagType?: string;
  tagSubType?: string;
}

export interface AchievementCreate {
  name?: string;
  description?: string;
  actorId?: string;
  actionId?: string;
  contentId?: string;
  badgeUrl?: string;
}

export interface AchievementDetail {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  actorId?: string;
  actionId?: string;
  contentId?: string;
  badgeUrl?: string;
}

export interface AchievementSummary {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  actorId?: string;
  actionId?: string;
  contentId?: string;
  badgeUrl?: string;
}

export interface AchievementUpdate {
  id?: number;
  name?: string;
  description?: string;
  actorId?: string;
  actionId?: string;
  contentId?: string;
  badgeUrl?: string;
}

export interface NotificationSummary {
  id?: number;
  subject?: string;
  body?: string;
  url?: string;
  values?: NotificationSummaryValue[];
  globalId?: string;
  created?: Date;
  read?: Date;
  logoUrl?: string;
  source?: string;
}

export interface NotificationSummaryValue {
  key?: string;
  value?: string;
}
export interface CommentCreate {
  text?: string;
}

export interface CommentUpdate {
  id?: number;
  text?: string;
}

export interface ContentCreate {
  id?: number;
  name?: string;
  description?: string;
  summary?: string;
  copyright?: string;
  globalId?: string;
  type?: ContentType;
  tags?: Array<string>;
  url?: string;
  logoUrl?: string;
  hoverUrl?: string;
  thumbnailUrl?: string;
  trailerUrl?: string;
  settings?: string;
  order?: number;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  isDisabled?: boolean;
  isRecommended?: boolean;
  isFeatured?: boolean;
  startDate?: string;
  endDate?: string;
  startTime?: string;
  endTime?: string;
}

export interface ContentDetail {
  id?: number;
  globalId?: string;
  name?: string;
  slug?: string;
  description?: string;
  summary?: string;
  copyright?: string;
  created?: Date;
  createdBy?: string;
  tags?: Array<ContentDetailTag>;
  url?: string;
  logoUrl?: string;
  hoverUrl?: string;
  thumbnailUrl?: string;
  launchUrl?: string;
  trailerUrl?: string;
  settings?: string;
  order?: number;
  type?: ContentType;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  publisherThumbnailUrl?: string;
  provider?: ContentDetailProvider;
  instances?: Array<ContentDetailContentInstance>;
  discussions?: Array<ContentDetailDiscussion>;
  isDisabled?: boolean;
  isRecommended?: boolean;
  isFeatured?: boolean;
  isBookmarked?: boolean;
  isFlagged?: boolean;
  levelBytes?: string;
  canAccess?: boolean;
  canEdit?: boolean;
  achievementCount?: number;
  userRating?: Rating;
  userDifficulty?: Difficulty;
  rating?: RatingMetricDetail;
  difficulty?: ContentDifficulty;
  keyValues?: Array<KeyValue>;
  flagCount?: number;
  flags: Array<ContentDetailFlag>;
  start?: Date;
  end?: Date;
  startDate?: string;
  endDate?: string;
  startTime?: string;
  endTime?: string;
  featuredOrder?: number;
  authorId?: number;
  authorName?: string;
}

export enum ContentDetailUserRating {
  Unrated = <any>'Unrated',
  Up = <any>'Up',
  Down = <any>'Down'
}

export enum ContentDetailUserLevel {
  Unrated = <any>'Unrated',
  Basic = <any>'Basic',
  Intermediate = <any>'Intermediate',
  Advanced = <any>'Advanced'
}

export interface ContentDetailContentInstance {
  id?: number;
  name?: string;
}


export interface ContentDetailDiscussion {
  id?: number;
  name?: string;
}


export interface ContentDetailProvider {
  id?: number;
  name?: string;
}


export interface ContentDetailTag {
  name?: string;
  slug?: string;
  tagType?: string;
}

export interface KeyValue {
  key?: string;
  value?: string;
}

export interface ContentSummary {
  id?: number;
  name?: string;
  slug?: string;
  description?: string;
  type?: ContentType;
  order?: number;
  ratingAverage?: number;
  ratingCount?: number;
  levelCounts?: Array<number>;
  ratingCounts?: Array<number>;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  publisherThumbnailUrl?: string;
  whenCreated?: string;
  canAccess?: boolean;
  canEdit?: boolean;
  isDisabled?: boolean;
  isRecommended?: boolean;
  isFeatured?: boolean;
  isBookmarked?: boolean;
  isFlagged?: boolean;
  achievementCount?: number;
  logoUrl?: string;
  hoverUrl?: string;
  thumbnailUrl?: string;
  trailerUrl?: string;
  url?: string;
  tags?: Array<ContentSummaryTag>;
  rating?: RatingMetricDetail;
  difficulty?: ContentDifficulty;
  keyValues?: Array<KeyValue>;
  startDate?: string;
  endDate?: string;
  startTime?: string;
  endTime?: string;
  featuredOrder?: number;
  authorId?: number;
  authorName?: string;
}

export interface LaunchedContents {
  contents: Array<LaunchedContent>;
}

export interface LaunchedContent {
  contentGlobalId: string;
  name: string;
  count: number;
}

export interface LeaderboardSummary {
  AssessmentName?: string;
  AssessmentGuid?: string;
  LeaderValues: Array<LeaderValue>;
}

export interface LeaderValue {
  Id?: number;
  AssessmentGuid?: string;
  AssessmentId?: number;
  AssessmentName?: string;
  UserId?: number;
  UserName?: string;
  OAuthId?: string;
  CommunityId?: number;
  CommunityName?: string;
  ElapsedSeconds?: number;
  TeamId?: number;
  UserScore?: number;
  DateCreated?: string;
  UserIdCreated?: number;
  DateUpdated?: string;
  UserIdUpdated?: number;
}
export interface ContentSummaryTag {
  name?: string;
  slug?: string;
  tagType?: string;
}

export interface PlaylistSummaryTag {
  name?: string;
  slug?: string;
  tagType?: string;
}

export interface ContentUpdate {
  id?: number;
  name?: string;
  slug?: string;
  description?: string;
  summary?: string;
  copyright?: string;
  type?: any;
  tags?: Array<string>;
  url?: string;
  logoUrl?: string;
  hoverUrl?: string;
  thumbnailUrl?: string;
  trailerUrl?: string;
  settings?: string;
  order?: number;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  isDisabled?: boolean;
  isRecommended?: boolean;
  isFeatured?: boolean;
  featuredOrder?: number;
  startDate?: string;
  endDate?: string;
  startTime?: string;
  endTime?: string;
}

export enum Difficulty {
  Unrated = <any>0,
  Basic = <any>1,
  Intermediate = <any>2,
  Advanced = <any>3
}

export enum Rating {
  Unrated = <any>0,
  Poor = <any>1,
  Fair = <any>2,
  Good = <any>3,
  Great = <any>4
}

export interface DiscussionDetailComment {
  id?: number;
  discussionId?: number;
  text?: string;
  votes?: number;
  author?: string;
  authorVote?: number;
  canVote?: boolean;
  whenCreated?: Date;
}

export interface ContentDetailFlag {
  profileId?: number;
  comment?: string;
  profileName?: string;
  flagStatus?: string;
}

export interface GroupCreate {
  id?: string;
  name?: string;
  logoUrl?: string;
  description?: string;
  summary?: string;
  parentId?: string;
}

export interface GroupDetail {
  id?: string;
  name?: string;
  logoUrl?: string;
  slug?: string;
  memberCount?: number;
  childCount?: number;
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

export interface GroupDetailMembership {
  id?: number;
  profileId?: number;
}

export interface GroupDetailProvider {
  id?: number;
}

export interface GroupSummary {
  id?: string;
  name?: string;
  logoUrl?: string;
  slug?: string;
  memberCount?: number;
  childCount?: number;
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

export interface GroupMemberDetail {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  isOwner?: boolean;
  isManager?: boolean;
}

export interface GroupMemberSummary {
  accountId?: string;
  accountName?: string;
  groupId?: string;
  groupName?: string;
  groupSlug?: string;
  isOwner?: boolean;
  isManager?: boolean;
  created: Date;
}


export interface GroupMemberCreate {
  accountId?: string;
  groupId?: string;
  isOwner?: boolean;
  isManager?: boolean;
}

export interface GroupMemberUpdate {
  accountId?: string;
  groupId?: string;
  isOwner?: boolean;
  isManager?: boolean;
}

export interface GroupMemberDelete {
  accountId?: string;
  groupId?: string;
}

export enum GroupMemberDetailPermissions {
  None = <any>'None',
  Member = <any>'Member',
  ManageMembers = <any>'Manage_Members',
  RunReports = <any>'Run_Reports',
  Edit = <any>'Edit',
  Owner = <any>'Owner'
}

export interface GroupAccess {
  edit?: boolean;
  delete?: boolean;
  member?: boolean;
  join?: boolean;
}

export interface PlaylistSummary {
  id?: number;
  globalId?: string;
  name?: string;
  description?: string;
  summary?: string;
  whenCreated?: Date;
  isDefault?: boolean;
  isFeatured?: boolean;
  isRecommended?: boolean;
  logoUrl?: string;
  trailerUrl?: string;
  contentCount?: number;
  sectionCount?: number;
  profileFollowerCount?: number;
  groupFollowerCount?: number;
  canFollow?: boolean;
  isFollowing?: boolean;
  canEdit?: boolean;
  keyValues?: Array<KeyValue>;
  tags?: Array<PlaylistSummaryTag>;
  featuredOrder?: number;
  rating?: RatingMetricDetail;
  slug?: string;
  copyright?: string;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
  publisherThumbnailUrl?: string;
}

export interface PlaylistUpdate {
  id?: number;
  name?: string;
  isPublic?: boolean;
  isDefault?: boolean;
  isFeatured?: boolean;
  isRecommended?: boolean;
  logoUrl?: string;
  trailerUrl?: string;
  featuredOrder?: number;
  tags?: Array<PlaylistSummaryTag>;
  copyright?: string;
  description?: string;
  summary?: string;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
}

export interface PlaylistCreate {
  id?: number;
  name?: string;
  isPublic?: boolean;
  isDefault?: boolean;
  isFeatured?: boolean;
  isRecommended?: boolean;
  logoUrl?: string;
  trailerUrl?: string;
  tags?: Array<PlaylistSummaryTag>;
  copyright?: string;
  description?: string;
  summary?: string;
  publisherId?: string;
  publisherName?: string;
  publisherSlug?: string;
}

export interface ProviderAgentSummary {
  id?: number;
  account?: string;
  password?: string;
}

export interface ProviderCreate {
  name?: string;
  description?: string;
  capability?: ProviderCreateCapability;
  logo?: string;
  groupId?: number;
}

export enum ProviderCreateCapability {
  TradecraftConsulting = <any>'Tradecraft_consulting',
  ScenarioDevelopment = <any>'Scenario_development',
  ExerciseManagement = <any>'Exercise_Management',
  InfrastructureManagement = <any>'Infrastructure_Management',
  TopologyHosting = <any>'Topology_hosting',
  TrainingDevelopment = <any>'Training_development'
}

export interface ProviderDetail {
  id?: number;
  name?: string;
  description?: string;
  capability?: ProviderDetailCapability;
  logo?: string;
  groupId?: number;
  contentCount?: number;
}

export enum ProviderDetailCapability {
  TradecraftConsulting = <any>'Tradecraft_consulting',
  ScenarioDevelopment = <any>'Scenario_development',
  ExerciseManagement = <any>'Exercise_Management',
  InfrastructureManagement = <any>'Infrastructure_Management',
  TopologyHosting = <any>'Topology_hosting',
  TrainingDevelopment = <any>'Training_development'
}

export interface ProviderSummary {
  id?: number;
  name?: string;
  description?: string;
  capabilities?: string;
  logo?: string;
  contentCount?: number;
}

export interface ProviderUpdate {
  id?: number;
  name?: string;
  description?: string;
  capability?: ProviderUpdateCapability;
  logo?: string;
  groupId?: number;
}

export enum ProviderUpdateCapability {
  TradecraftConsulting = <any>'Tradecraft_consulting',
  ScenarioDevelopment = <any>'Scenario_development',
  ExerciseManagement = <any>'Exercise_Management',
  InfrastructureManagement = <any>'Infrastructure_Management',
  TopologyHosting = <any>'Topology_hosting',
  TrainingDevelopment = <any>'Training_development'
}

export class DashboardItem {
  title: string;
  result: any;
  dataFilter: DataFilter;
  state: string = 'pending';
  queryParams: any;
  type: string;
}

export interface DataFilter {
  term?: string;
  skip?: number;
  take?: number;
  sort?: string;
  filter?: string;
}

export class PagedResult<T> {
  total?: number;
  results: Array<T> = [];
  dataFilter?: DataFilter;
}

export interface AnalyticsEventSummary {
  id?: number;
  type?: string;
  data?: string;
  created?: Date;
  createdBy?: string;
  clientId?: string;
}

export interface UserEventCreate {
  type?: string;
  data?: string;
}

export interface UserEventSummary {
  id?: number;
  type?: string;
  data?: string;
  created?: Date;
  createdBy?: string;
  clientId?: string;
}

export interface ContentEventCreate {
  contentId?: string;
  contentName?: string;
  contentSlug?: string;
  type?: string;
  data?: string;
  progress?: number;
}

export interface ContentEventSummary {
  id?: number;
  contentId?: string;
  contentName?: string;
  contentSlug?: string;
  type?: string;
  data?: string;
  progress?: number;
  created?: Date;
  createdBy?: string;
  clientId?: string;
}

export interface ClientEventCreate {
  url?: string;
  lastUrl?: string;
  type?: string;
  data?: string;
}

export interface ClientEventSummary {
  id?: number;
  url?: string;
  lastUrl?: string;
  type?: string;
  data?: string;
  created?: Date;
  createdBy?: string;
  clientId?: string;
}

export interface PageViewMetric {
  url?: string;
  total?: number;
  totalUnique?: number;
  history?: PageViewMetricHistory[];
}

export interface PageViewMetricHistory {
  url?: string;
  total?: number;
}

export interface SettingDetail {
  key?: string;
  value?: string;
}

export interface SettingUpdate {
  key?: string;
  value?: string;
}

export interface SettingCreate {
  key?: string;
  value?: string;
}

export interface DataSetResult {
  total?: number;
  dataFilter?: DataFilter;
  dataSet?: DataSet;
  reportName?: string;
  fileName?: string;
}

export interface DataSet {
  columns?: DataColumn[];
  rows?: DataRow[];
}

export interface DataColumn {
  name?: string;
  sort?: string;
  isSortedBy?: boolean;
  sortDirection?: string;
}

export interface DataRow {
  values?: DataValue[];
}

export interface DataValue {
  value?: string;
}

export class Filter {
  value: string;
  text: string;
}

export class Report {
  name: string;
  description: string;
  slug: string;
  defaultSort: string;
  filters: Array<Filter> = [];
}

export interface ApplicationSummary {
  id: number;
  name: string,
  displayName: string,
  description: string
  slug: string,
  clientUri: string,
  eventReferenceUri: string,
  logoUri: string,
  enabled: boolean,
  isBookmarked: boolean,
  isHidden: boolean,
  isPinned: boolean,
  working: boolean
}

export interface ApplicationUpdate {
  id: number,
  isHidden: boolean,
  isPinned: boolean
}

export interface ContentLaunch {
  name: string,
  globalId: string
}

export interface ContentView {
  name: string,
  globalId: string
}

export interface Search {
  term: string
}

export interface FileStorageResult {
  exception: any,
  file: FileDetail,
  fileName: string,
  globalId: string,
  type: string
}

export interface FileDetail {
  bucketId: number
  bucketName: string,
  contentType: string,
  extension: string,
  globalId: string,
  id: number,
  length: number,
  name: string,
  slug: string,
  tags: Array<string>,
  url: string,
  urlWithExtension: string,
  versionNumber: number
}

export interface ConfigurationItem {
  name: string;
  settings: Array<ConfigurationItem>;
}

export interface ConfigurationItemSetting {
  key: string;
  value: string;
}

export interface PostDetail {
  id: number;
  text: string;
  parentId?: number;
  attachments: Array<string>;
  replies: Array<PostDetail>;
  voteMetric: PostVoteMetric;
  profileId: number;
  profileName: string;
  profileSlug: string;
  created?: Date;
}

export interface PostVoteMetric {
  up: number;
  down: number;
  userVote: number;
}

export interface PostCreate {
  text: string;
  parentId?: number;
  attachments: Array<string>;
}

export interface PostUpdate {
  id: number;
  text: string;
  parentId?: number;
  attachments: Array<string>;
}

export interface FormError
{
  name: string;
  messages: Array<string>;
}

export interface Statement {
  id?: string;
  name?: string;
  description?: string;
  verb?: string;
  count?: number;
  agentName?: string;
}

