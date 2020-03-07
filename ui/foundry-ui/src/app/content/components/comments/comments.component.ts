/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Inject, Input, OnChanges } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Converter } from 'showdown/dist/showdown';
import { CommentUpdate, ContentSummary, DataFilter, DiscussionDetailComment, PagedResult, ProfileInfo } from '../../../core-api-models';
import { DiscussionService } from '../../../discussion/discussion.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'content-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['comments.component.scss']
})
export class ContentCommentsComponent extends BaseComponent implements OnChanges {

  @Input() content: ContentSummary;
  comments: DiscussionDetailComment[] = [];
  comment: any;
  newCommentText = '';
  converter: Converter;
  showComments: boolean;
  showMore: boolean;
  searchArgs: DataFilter;
  btnDisabled = false;
  profileInfo: ProfileInfo;

  constructor(
    private contentSvc: ContentService,
    private commentSvc: DiscussionService,
    private profileSvc: ProfileService,
    public dialog: MatDialog,
    private messageSvc: MessageService

  ) {
    super();
    this.converter = new Converter();
  }

  ngOnChanges(): void {
    this.showComments = true; // this.content.allowComments;

    this.searchNew();
  }

  searchMore(): void {
    this.searchArgs.skip += this.searchArgs.take;
    this.search();
  }

  searchNew(): void {
    this.comments = [];
    this.searchArgs = {
      skip: 0,
      take: 10,
      // filters: [{ id: this.content.id, name: "content" }]
    };
    this.search();
  }

  search(): void {
    this.$.push(this.contentSvc.comments(this.content.id, this.searchArgs).subscribe((data: PagedResult<DiscussionDetailComment>) => {
      this.comments = data.results;
      this.showMore = data.total > 0 && this.searchArgs.skip + this.searchArgs.take < data.total;
    }));
  }

  toHtml(text: string): string {
    return this.converter.makeHtml(text);
  }

  addComment(): void {
    this.btnDisabled = true;
    if (this.newCommentText && this.newCommentText.length > 0) {
      this.contentSvc
        .comment(this.content.id, this.newCommentText)
        .subscribe((result: DiscussionDetailComment) => {
          // result.text = this.converter.makeHtml(result.text);
          this.comments.push(result);
          this.btnDisabled = false;
          this.newCommentText = '';
        });
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
    this.commentSvc.upVote(comment.id).subscribe((result: DiscussionDetailComment) => {
      this.replaceComment(result);
      this.messageSvc.addSnackBar('Comment was up-voted');
    });
  }

  downvote(comment) {
    this.commentSvc.downVote(comment.id).subscribe((result: DiscussionDetailComment) => {
      this.replaceComment(result);
      this.messageSvc.addSnackBar('Comment was down-voted');
    });
  }

  private replaceComment(comment: DiscussionDetailComment) {
    for (let i = 0; i < this.comments.length; i++) {
      if (this.comments[i].id === comment.id) {
        this.comments[i] = comment;
        break;
      }
    }
  }

  openEdit(comment: CommentUpdate) {
    const dialogRef = this.dialog.open(CommentEditDialog, {
      width: '600px',
      data: { id: comment.id, text: comment.text }
    });

    dialogRef.componentInstance.updateText.subscribe(() => {
      this.comments = [];
      this.search();
    });
  }

  openDelete(comment: DiscussionDetailComment, index: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Confirm Delete',
        message: 'Are you sure you want to delete this comment?',
        yesText: 'Delete',
        yesCallback: () => {
          this.commentSvc.deleteComment(comment.id).subscribe(result => {
            dialogRef.close();
            this.comments.splice(index, 1);
              this.messageSvc.addSnackBar('Comment Removed');
            },
            error => {
              this.messageSvc.addSnackBar(error.error.message);
            }
          );
        },
        noText: 'Cancel',
        noCallback: () => { dialogRef.close(); },
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }
}


@Component({
  selector: 'comment-edit-dialog',
  templateUrl: 'comment-edit-dialog.component.html',
})
export class CommentEditDialog {
  updateText = new EventEmitter();
  constructor(
    public dialogRef: MatDialogRef<CommentEditDialog>,
    public commentSvc: DiscussionService,
    public messageSvc: MessageService,
    @Inject(MAT_DIALOG_DATA) public data: CommentUpdate) { }

  updateComment() {
    this.commentSvc.updateComment(this.data).subscribe(
      () => {
        this.updateText.emit();
        this.dialogRef.close();
        this.messageSvc.addSnackBar('Comment Updated');
      },
      error => {
        this.messageSvc.addSnackBar(error.error.message);
      }
    );
  }

  cancelEdit() {
    this.dialogRef.close();
  }

}

