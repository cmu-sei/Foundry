/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnChanges } from '@angular/core';
import { Converter } from 'showdown/dist/showdown';
import { Comment, Discussion } from '../../../core-api-models';
import { DiscussionService } from '../../discussion.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'comments',
  templateUrl: './browser.component.html'
})
export class DiscussionBrowserComponent extends BaseComponent implements OnChanges {
  @Input() entityId = 0;
  @Input() discussionTypeId = 0;

  comments: Comment[] = [];
  term: string;
  discussions: Discussion[] = [];
  comment: Comment;
  newCommentText = '';
  converter: Converter;
  showComments: boolean;

  constructor(
    private service: DiscussionService,
  ) {
    super();
    this.converter = new Converter();
  }

  ngOnChanges(): void {
    this.loadComments();
  }

  search(term, discussionId) {
    this.service.comments({
      term: term,
      take: 20,
      filters: [{ id: discussionId, name: 'discussion' }]
    }).subscribe(result => {
      this.comments = result.results as Comment[];

      for (let i = 0; i < this.comments.length; i++) {
        this.comments[i].text = this.converter.makeHtml(this.comments[i].text);
      }
    });
  }

  addComment(formValue): void {
    if (formValue.newCommentText && formValue.newCommentText.length > 0) {
      if (this.discussions[0]) {
        this.service.saveComment(0, formValue.newCommentText, this.discussions[0].id)
          .subscribe(result => {
            const comment = result as Comment;
            comment.text = this.converter.makeHtml(comment.text);
            this.comments.push(comment);
            this.newCommentText = '';
          });
      }
    }
  }

  sortScore(): void {
    this.comments.sort(this.commentVoteSortFunction);
  }

  commentVoteSortFunction(a, b) {
    return a.votes > b.votes ? -1 : 1;
  }

  sortDate(): void {
    this.comments.sort(this.commentDateSortFunction);
  }

  commentDateSortFunction(a, b) {
    const dateA = new Date(a.whenCreated);
    const dateB = new Date(b.whenCreated);
    return dateA > dateB ? 1 : -1;
  }

  upvote(comment) {
    this.$.push(this.service.upVote(comment.id).subscribe(result => {
      this.replaceComment(result as Comment);
    }));
  }

  downvote(comment) {
    this.$.push(this.service.downVote(comment.id).subscribe(result => {
      this.replaceComment(result as Comment);
    }));
  }

  private replaceComment(comment: Comment) {
    for (let i = 0; i < this.comments.length; i++) {
      if (this.comments[i].id === comment.id) {
        this.comments[i] = comment;
      }
    }
  }

  loadComments() {
    this.$.push(this.service.getDiscussionByType(this.discussionTypeId, this.entityId)
      .subscribe(result => {
        this.discussions = result as Discussion[];

        if (this.discussions[0]) {
          this.search('', this.discussions[0].id);
          this.showComments = true;
        } else {
          this.showComments = false;
        }
      }));
  }
}

