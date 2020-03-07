/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { PostDetail, PostVoteMetric } from '../../../core-api-models';
import { PostsService } from '../../../posts/posts.service';
import { MessageService } from '../../../root/message.service';
import { ConfirmDialogComponent } from '../confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'post-tile',
  templateUrl: './post-tile.component.html',
  styleUrls: ['./post-tile.component.scss'],
})

export class PostTileComponent implements OnInit {
  @Input() public posts: Array<PostDetail>;
  @Input() public post: PostDetail;
  @Input() public index: number = -1;
  @Input() public canDelete: boolean = false;
  public deletePost: PostDetail = null;

  constructor(
    private postsService: PostsService,
    private messageService: MessageService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {

  }

  openDeletePostDialog(post: PostDetail): void {
    this.deletePost = post;
    const component = this;
    let msg = '';
    let title = '';

    if (post.replies) {
      title = 'Delete Thread?';
      msg = 'Are you sure you want to delete this thread?';
    } else {
      title = 'Delete Post?';
      msg = 'Are you sure you want to remove this post?';
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: title,
        message: msg,
        yesText: 'Delete',
        yesCallback: this.confirmDeletePost,
        noText: 'Cancel',
        noCallback: this.cancelDeletePost,
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }

  cancelDeletePost(context: PostTileComponent) {
    context.deletePost = null;
  }

  confirmDeletePost(context: PostTileComponent) {
    context.postsService.delete(context.deletePost.id).subscribe((data: boolean) => {
      let index = context.posts.indexOf(context.deletePost);
      context.posts.splice(index, 1);
      context.messageService.addSnackBar('Post was removed');
    });
  }

  upVote(post: PostDetail) {
    this.postsService.upVote(post.id).subscribe((data: PostVoteMetric) => {
      this.messageService.addSnackBar(data.userVote === 0 ? 'Vote was removed' : 'Post was up-voted');
      post.voteMetric = data;
    });
  }

  downVote(post: PostDetail) {
    this.postsService.downVote(post.id).subscribe((data: PostVoteMetric) => {
      this.messageService.addSnackBar(data.userVote === 0 ? 'Vote was removed' : 'Post was down-voted');
      post.voteMetric = data;
    });
  }
}

