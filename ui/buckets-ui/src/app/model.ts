/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

export interface DataFilter {
    term?: string;
    skip?: number;
    take?: number;
    sort?: string;
    filter?: string;
}

export class PagedResult<T> {
    total?: number;
    results: T[];
    dataFilter?: DataFilter;
}

export class BucketDetail {
    id?: number;
    globalId?: string;
    name?: string;
    slug?: string;
    isDefault?: boolean;
    bucketSharingType?: BucketSharingType;
    access?: Array<string>;
}

export enum BucketSharingType {
    Public = <any>'Public',
    Private = <any>'Private',
    Restricted = <any>'Restricted'
}

export enum BucketAccessType {
    Manager = <any>'Manager',
    Owner = <any>'Owner',
    Member = <any>'Member'
}

export class BucketSummary {
    id?: number;
    globalId?: string;
    name?: string;
    slug?: string;
    isDefault?: boolean;
    bucketSharingType?: BucketSharingType;
    access?: Array<string>;
}

export class BucketUpdate {
    id?: number;
    name?: string;
    isDefault?: boolean;
    bucketSharingType?: BucketSharingType;
}

export class FileDetail {
    id: number;
    name: string;
    slug: string;
    globalId: string;
    extension: string;
    length: number;
    url: string;
    urlWithExtension: string;
    contentType: string;
    bucketName: string;
    bucketId: number;
    versionNumber: number;
    tags: Array<string>;
    access?: Array<string>;
    createdByName?: string;
    created: Date;
}

export class FileSummary {
    id: number;
    name: string;
    slug: string;
    globalId: string;
    extension: string;
    length: number;
    url: string;
    urlWithExtension: string;
    contentType: string;
    bucketName: string;
    bucketId: number;
    versionNumber: number;
    tags: Array<string>;
    access?: Array<string>;
    createdByName?: string;
    created: Date;
}

export class ImportFileSummary {
    name: string;
    extension: string;
    bucketName: string;
    bucketId: number;
    path: string;
    globalId: string;
    isImported: boolean;
}

export class ImportFileUpdate {
    name: string;
    path: string;
    bucketId: number;
    globalId: string;
}

export class FileUpdate {
    id: number;
    name: string;
    bucketId: number;
    tags: Array<string>;
}

export class BucketCreate {
    name?: string;
    isDefault?: boolean;
    bucketSharingType?: BucketSharingType;
    globalId?: string;
}

export class AccountSummary {
    globalId?: string;
    name?: string;
    isUploadOwner?: boolean;
    isAdministrator?: boolean;
    isApplication?: boolean;
    bucketCount?: number;
}

export class AccountDetail {
    globalId?: string;
    name?: string;
    isUploadOwner?: boolean;
    isAdministrator?: boolean;
    isApplication?: boolean;
    buckets?: Array<AccountDetailBucket>;
}

export class AccountDetailBucket {
    id?: number;
    name?: string;
    slug?: string;
    isDefault?: boolean;
    bucketSharingType?: BucketSharingType;
    bucketAccessType?: BucketAccessType;
    access?: Array<string>
}

export class AccountCreate {
    globalId?: string;
    name?: string;
    isUploadOwner?: boolean;
    isAdministrator?: boolean;
    isApplication?: boolean;
}

export class AccountUpdate {
    globalId?: string;
    name?: string;
    isUploadOwner?: boolean;
    isAdministrator?: boolean;
    isApplication?: boolean;
    buckets?: Array<AccountUpdateBucket>;
}

export class AccountUpdateBucket {
    id?: number;
    name?: string;
    isDefault?: boolean;
    bucketAccessType?: BucketAccessType;
}

export class BucketAccountCreate {
    bucketId?: number;
    accountId?: string;
    isDefault?: boolean;
    bucketAccessType?: BucketAccessType;
}
