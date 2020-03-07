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
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { CommentUpdate, DiscussionDetailComment } from '../core-api-models';
import { SettingsService } from '../root/settings.service';

@Injectable()
export class DiscussionService {

    constructor(
        private http: HttpClient,
        private auth: AuthService,
        private settingsSvc: SettingsService
    ) { }

    url() {
        return this.settingsSvc.settings.clientSettings.urls.coreUrl;
    }

    public load(id) {
        return this.http.get(this.url() + '/discussion/' + id);
    }

    public getDiscussionByType(type, id) {
        return this.http.get(this.url() + '/content/' + id + '/discussion/' + type);
    }

    public save(discussion) {
        if (discussion.id === 0) {
            return this.http.post(this.url() + '/discussions', discussion);
        } else {
            return this.http.put(this.url() + '/discussion/' + discussion.id, discussion);
        }
    }

    public comments(search): Observable<any> {
        return this.http.get<any>(this.url() + '/discussion/comments?' + this.auth.queryStringify(search, '?'));
    }

    public saveComment(id, text, discussionId) {
        if (id === 0) {
            return this.http.post(this.url() + '/discussion/' + discussionId + '/comments', { text: text, discussionId: discussionId });
        } else {
            return this.http.put(this.url() + '/comment/' + id, { text: text, discussionId: discussionId });
        }
    }

    public updateComment(comment: CommentUpdate): Observable<DiscussionDetailComment> {
        return this.http.put(this.url() + '/comment/' + comment.id, comment);
    }

    public deleteComment(id: number) {
        return this.http.delete(this.url() + '/comment/' + id);
   }

    public upVote(commentId) {
        return this.http.post(this.url() + '/comment/' + commentId + '/up', null);
    }

    public downVote(commentId) {
        return this.http.post(this.url() + '/comment/' + commentId + '/down', null);
    }
}

