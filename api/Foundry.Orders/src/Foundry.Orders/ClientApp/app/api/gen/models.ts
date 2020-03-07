/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

export interface PagedResultAssessmentTypeAssessmentTypeSummary {
	dataFilter?: IDataFilterAssessmentType;
	total?: number;
	results?: Array<AssessmentTypeSummary>;
}

export interface IDataFilterAssessmentType {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface AssessmentTypeSummary {
	id?: number;
	name?: string;
}

export interface PagedResultAudienceAudienceSummary {
	dataFilter?: IDataFilterAudience;
	total?: number;
	results?: Array<AudienceSummary>;
}

export interface IDataFilterAudience {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface AudienceSummary {
	id?: number;
	name?: string;
}

export interface PagedResultAudienceItemAudienceItemSummary {
	dataFilter?: IDataFilterAudienceItem;
	total?: number;
	results?: Array<AudienceItemSummary>;
}

export interface IDataFilterAudienceItem {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface AudienceItemSummary {
	id?: number;
	name?: string;
	audienceId?: number;
	audienceName?: string;
}

export interface PagedResultBranchBranchSummary {
	dataFilter?: IDataFilterBranch;
	total?: number;
	results?: Array<BranchSummary>;
}

export interface IDataFilterBranch {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface BranchSummary {
	id?: number;
	name?: string;
}

export interface PagedResultClassificationClassificationSummary {
	dataFilter?: IDataFilterClassification;
	total?: number;
	results?: Array<ClassificationSummary>;
}

export interface IDataFilterClassification {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ClassificationSummary {
	id?: number;
	name?: string;
}

export interface PagedResultCommentCommentDetail {
	dataFilter?: IDataFilterComment;
	total?: number;
	results?: Array<CommentDetail>;
}

export interface IDataFilterComment {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface CommentDetail {
	id?: number;
	title?: string;
	message?: string;
	orderId?: number;
	created?: string;
	createdById?: number;
	createdByName?: string;
}

export interface CommentEdit {
	id?: number;
	title?: string;
	message?: string;
}

export interface PagedResultContentTypeContentTypeSummary {
	dataFilter?: IDataFilterContentType;
	total?: number;
	results?: Array<ContentTypeSummary>;
}

export interface IDataFilterContentType {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ContentTypeSummary {
	id?: number;
	name?: string;
}

export interface PagedResultEmbeddedTeamEmbeddedTeamSummary {
	dataFilter?: IDataFilterEmbeddedTeam;
	total?: number;
	results?: Array<EmbeddedTeamSummary>;
}

export interface IDataFilterEmbeddedTeam {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface EmbeddedTeamSummary {
	id?: number;
	name?: string;
}

export interface PagedResultEventTypeEventTypeSummary {
	dataFilter?: IDataFilterEventType;
	total?: number;
	results?: Array<EventTypeSummary>;
}

export interface IDataFilterEventType {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface EventTypeSummary {
	id?: number;
	name?: string;
}

export interface PagedResultFacilityFacilitySummary {
	dataFilter?: IDataFilterFacility;
	total?: number;
	results?: Array<FacilitySummary>;
}

export interface IDataFilterFacility {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface FacilitySummary {
	id?: number;
	name?: string;
}

export interface PagedResultFileFileSummary {
	dataFilter?: IDataFilterFile;
	total?: number;
	results?: Array<FileSummary>;
}

export interface IDataFilterFile {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface FileSummary {
	id?: number;
	name?: string;
	created?: string;
}

export interface PagedResultOperatingSystemTypeOperatingSystemTypeSummary {
	dataFilter?: IDataFilterOperatingSystemType;
	total?: number;
	results?: Array<OperatingSystemTypeSummary>;
}

export interface IDataFilterOperatingSystemType {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface OperatingSystemTypeSummary {
	id?: number;
	name?: string;
}

export interface PagedResultOrderOrderSummary {
	dataFilter?: IDataFilterOrder;
	total?: number;
	results?: Array<OrderSummary>;
}

export interface IDataFilterOrder {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface OrderSummary {
	id?: number;
	isPrivate?: boolean;
	requestor?: string;
	branchName?: string;
	branchId?: number;
	branchOther?: string;
	rankName?: string;
	rankId?: number;
	unit?: string;
	email?: string;
	phone?: string;
	producerName?: string;
	producerId?: number;
	description?: string;
	contentTypeName?: string;
	contentTypeId?: number;
	objectives?: string;
	isConfigurationNeeded?: boolean;
	toolPreparation?: string;
	environmentDetails?: string;
	networkDiagramUrl?: string;
	adversaryDetails?: string;
	scenarioCommunications?: string;
	scenarioAudienceDetails?: string;
	scenarioAudienceProcedures?: string;
	prerequisites?: string;
	onboarding?: string;
	duration?: number;
	durationType?: OrderSummaryDurationTypeEnum;
	due?: string;
	audienceName?: string;
	audienceId?: number;
	audienceItemOther?: string;
	audienceNumber?: number;
	assessmentTypeOther?: string;
	operatingSystemOther?: string;
	securityToolOther?: string;
	serviceOther?: string;
	simulatorOther?: string;
	terrainOther?: string;
	threatOther?: string;
	supportOther?: string;
	isEmbeddedTeam?: boolean;
	embeddedTeamOther?: string;
	cyberThreats?: string;
	successIndicators?: string;
	eventTypeName?: string;
	eventTypeId?: number;
	contentDescription?: string;
	trainingDescription?: string;
	eventParticipants?: string;
	theater?: string;
	storyline?: string;
	missionProcedures?: string;
	classificationName?: string;
	classificationId?: number;
	classificationOther?: string;
	facilityName?: string;
	facilityId?: number;
	eventStart?: string;
	eventEnd?: string;
	status?: OrderSummaryStatusEnum;
	commentCount?: number;
	created?: string;
	createdById?: number;
	createdByName?: string;
}

export interface OrderEdit {
	id?: number;
	isPrivate?: boolean;
	requestor?: string;
	branchName?: string;
	branchId?: number;
	branchOther?: string;
	rankName?: string;
	rankId?: number;
	unit?: string;
	email?: string;
	phone?: string;
	producerName?: string;
	producerId?: number;
	description?: string;
	contentTypeName?: string;
	contentTypeId?: number;
	objectives?: string;
	isConfigurationNeeded?: boolean;
	toolPreparation?: string;
	environmentDetails?: string;
	adversaryDetails?: string;
	scenarioCommunications?: string;
	scenarioAudienceDetails?: string;
	scenarioAudienceProcedures?: string;
	prerequisites?: string;
	onboarding?: string;
	duration?: number;
	durationType?: OrderEditDurationTypeEnum;
	due?: string;
	audienceName?: string;
	audienceId?: number;
	audienceItems?: Array<number>;
	audienceItemOther?: string;
	audienceNumber?: number;
	assessmentTypes?: Array<number>;
	assessmentTypeOther?: string;
	operatingSystemTypes?: Array<number>;
	operatingSystemOther?: string;
	securityTools?: Array<number>;
	securityToolOther?: string;
	services?: Array<number>;
	serviceOther?: string;
	simulators?: Array<number>;
	simulatorOther?: string;
	terrains?: Array<number>;
	terrainOther?: string;
	roleCrewPosition?: string;
	threats?: Array<number>;
	threatOther?: string;
	supports?: Array<number>;
	supportOther?: string;
	isEmbeddedTeam?: boolean;
	embeddedTeams?: Array<number>;
	embeddedTeamOther?: string;
	cyberThreats?: string;
	successIndicators?: string;
	files?: Array<OrderEditFile>;
	eventTypeName?: string;
	eventTypeId?: number;
	contentDescription?: string;
	trainingDescription?: string;
	eventParticipants?: string;
	theater?: string;
	storyline?: string;
	missionProcedures?: string;
	classificationName?: string;
	classificationId?: number;
	classificationOther?: string;
	facilityName?: string;
	facilityId?: number;
	eventStart?: string;
	eventEnd?: string;
	status?: OrderEditStatusEnum;
	created?: string;
	createdById?: number;
	createdByName?: string;
	networkDiagramUrl?: string;
}

export interface OrderEditFile {
	id?: number;
	name?: string;
	url?: string;
	type?: string;
	extension?: string;
}

export interface OrderDetail {
	id?: number;
	isPrivate?: boolean;
	requestor?: string;
	branchName?: string;
	branchId?: number;
	branchOther?: string;
	rankName?: string;
	rankId?: number;
	unit?: string;
	email?: string;
	phone?: string;
	producerName?: string;
	producerId?: number;
	description?: string;
	contentTypeName?: string;
	contentTypeId?: number;
	objectives?: string;
	isConfigurationNeeded?: boolean;
	toolPreparation?: string;
	environmentDetails?: string;
	adversaryDetails?: string;
	scenarioCommunications?: string;
	scenarioAudienceDetails?: string;
	scenarioAudienceProcedures?: string;
	prerequisites?: string;
	onboarding?: string;
	duration?: number;
	durationType?: OrderDetailDurationTypeEnum;
	due?: string;
	audienceName?: string;
	audienceId?: number;
	audienceItems?: Array<OrderDetailAudienceItem>;
	audienceItemOther?: string;
	audienceNumber?: number;
	assessmentTypes?: Array<OrderDetailAssessmentType>;
	assessmentTypeOther?: string;
	operatingSystemTypes?: Array<OrderDetailOperatingSystemType>;
	operatingSystemOther?: string;
	securityTools?: Array<OrderDetailSecurityTool>;
	securityToolOther?: string;
	services?: Array<OrderDetailService>;
	serviceOther?: string;
	simulators?: Array<OrderDetailSimulator>;
	simulatorOther?: string;
	terrains?: Array<OrderDetailTerrain>;
	terrainOther?: string;
	threats?: Array<OrderDetailThreat>;
	threatOther?: string;
	supports?: Array<OrderDetailSupport>;
	supportOther?: string;
	isEmbeddedTeam?: boolean;
	embeddedTeams?: Array<OrderDetailEmbeddedTeam>;
	embeddedTeamOther?: string;
	networkDiagramUrl?: string;
	cyberThreats?: string;
	successIndicators?: string;
	files?: Array<OrderDetailFile>;
	eventTypeName?: string;
	eventTypeId?: number;
	contentDescription?: string;
	trainingDescription?: string;
	eventParticipants?: string;
	theater?: string;
	storyline?: string;
	missionProcedures?: string;
	classificationName?: string;
	classificationId?: number;
	classificationOther?: string;
	facilityName?: string;
	facilityId?: number;
	eventStart?: string;
	eventEnd?: string;
	status?: OrderDetailStatusEnum;
	comments?: Array<OrderDetailComment>;
	created?: string;
	createdById?: number;
	createdByName?: string;
}

export interface OrderDetailAudienceItem {
	id?: number;
	name?: string;
}

export interface OrderDetailAssessmentType {
	id?: number;
	name?: string;
}

export interface OrderDetailOperatingSystemType {
	id?: number;
	name?: string;
}

export interface OrderDetailSecurityTool {
	id?: number;
	name?: string;
}

export interface OrderDetailService {
	id?: number;
	name?: string;
}

export interface OrderDetailSimulator {
	id?: number;
	name?: string;
}

export interface OrderDetailTerrain {
	id?: number;
	name?: string;
}

export interface OrderDetailThreat {
	id?: number;
	name?: string;
}

export interface OrderDetailSupport {
	id?: number;
	name?: string;
}

export interface OrderDetailEmbeddedTeam {
	id?: number;
	name?: string;
}

export interface OrderDetailFile {
	id?: number;
	name?: string;
	url?: string;
	created?: string;
	createdById?: number;
	createdByName?: string;
}

export interface OrderDetailComment {
	id?: number;
	title?: string;
	message?: string;
	created?: string;
	createdById?: number;
	createdByName?: string;
}

export interface PagedResultProducerProducerSummary {
	dataFilter?: IDataFilterProducer;
	total?: number;
	results?: Array<ProducerSummary>;
}

export interface IDataFilterProducer {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ProducerSummary {
	id?: number;
	name?: string;
}

export interface PagedResultProfileProfileSummary {
	dataFilter?: IDataFilterProfile;
	total?: number;
	results?: Array<ProfileSummary>;
}

export interface IDataFilterProfile {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ProfileSummary {
	id?: number;
	name?: string;
	isAdministrator?: boolean;
}

export interface PagedResultRankRankSummary {
	dataFilter?: IDataFilterRank;
	total?: number;
	results?: Array<RankSummary>;
}

export interface IDataFilterRank {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface RankSummary {
	id?: number;
	name?: string;
	grade?: string;
	branchId?: number;
}

export interface PagedResultSecurityToolSecurityToolSummary {
	dataFilter?: IDataFilterSecurityTool;
	total?: number;
	results?: Array<SecurityToolSummary>;
}

export interface IDataFilterSecurityTool {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface SecurityToolSummary {
	id?: number;
	name?: string;
}

export interface PagedResultServiceServiceSummary {
	dataFilter?: IDataFilterService;
	total?: number;
	results?: Array<ServiceSummary>;
}

export interface IDataFilterService {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ServiceSummary {
	id?: number;
	name?: string;
}

export interface PagedResultSimulatorSimulatorSummary {
	dataFilter?: IDataFilterSimulator;
	total?: number;
	results?: Array<SimulatorSummary>;
}

export interface IDataFilterSimulator {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface SimulatorSummary {
	id?: number;
	name?: string;
}

export interface PagedResultSupportSupportSummary {
	dataFilter?: IDataFilterSupport;
	total?: number;
	results?: Array<SupportSummary>;
}

export interface IDataFilterSupport {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface SupportSummary {
	id?: number;
	name?: string;
}

export interface PagedResultTerrainTerrainSummary {
	dataFilter?: IDataFilterTerrain;
	total?: number;
	results?: Array<TerrainSummary>;
}

export interface IDataFilterTerrain {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface TerrainSummary {
	id?: number;
	name?: string;
}

export interface PagedResultThreatThreatSummary {
	dataFilter?: IDataFilterThreat;
	total?: number;
	results?: Array<ThreatSummary>;
}

export interface IDataFilterThreat {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export interface ThreatSummary {
	id?: number;
	name?: string;
}

export interface DataFilter {
	term?: string;
	skip?: number;
	take?: number;
	filter?: string;
	sort?: string;
}

export enum OrderSummaryDurationTypeEnum {
	NotSet = <any>'NotSet',
	Hours = <any>'Hours',
	Days = <any>'Days',
	Months = <any>'Months',
	Weeks = <any>'Weeks'
}

export enum OrderSummaryStatusEnum {
	Draft = <any>'Draft',
	Submitted = <any>'Submitted',
	InProgress = <any>'InProgress',
	NeedsInformation = <any>'NeedsInformation',
	Complete = <any>'Complete',
	Closed = <any>'Closed'
}

export enum OrderEditDurationTypeEnum {
	NotSet = <any>'NotSet',
	Hours = <any>'Hours',
	Days = <any>'Days',
	Months = <any>'Months',
	Weeks = <any>'Weeks'
}

export enum OrderEditStatusEnum {
	Draft = <any>'Draft',
	Submitted = <any>'Submitted',
	InProgress = <any>'InProgress',
	NeedsInformation = <any>'NeedsInformation',
	Complete = <any>'Complete',
	Closed = <any>'Closed'
}

export enum OrderDetailDurationTypeEnum {
	NotSet = <any>'NotSet',
	Hours = <any>'Hours',
	Days = <any>'Days',
	Months = <any>'Months',
	Weeks = <any>'Weeks'
}

export enum OrderDetailStatusEnum {
	Draft = <any>'Draft',
	Submitted = <any>'Submitted',
	InProgress = <any>'InProgress',
	NeedsInformation = <any>'NeedsInformation',
	Complete = <any>'Complete',
	Closed = <any>'Closed'
}


