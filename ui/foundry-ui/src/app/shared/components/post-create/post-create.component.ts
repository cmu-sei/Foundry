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
import { ImageBrowserComponent } from 'src/app/images/components/browser/browser.component';
import { ImagesService } from 'src/app/images/images.service';
import { SettingsService } from 'src/app/root/settings.service';
import { PostCreate, PostDetail } from '../../../core-api-models';
import { PostsService } from '../../../posts/posts.service';
import { MessageService } from '../../../root/message.service';

@Component({
  selector: 'post-create',
  templateUrl: './post-create.component.html',
  styleUrls: ['./post-create.component.scss'],
})

export class PostCreateComponent implements OnInit {
  @Input() public posts: Array<PostDetail>;
  @Input() public isHidden: boolean = false;
  @Input() public parentId: number = null;
  @Input() public placeholderMsg: string;
  logoRootUrl: string;
  public postsSubmitSpin: boolean = false;
  post: PostCreate = { text: '', attachments: [] };

  constructor(
    private postsService: PostsService,
    private messageService: MessageService,
    public imageService: ImagesService,
    private settingService: SettingsService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {
    this.logoRootUrl = this.settingService.settings.clientSettings.urls.uploadUrl;

    this.imageService.fileItem$.subscribe(result => {
      const url = this.settingService.settings.clientSettings.urls.uploadUrl +
        result.urlWithExtension;
      this.post.attachments.push(url);
    });

    this.imageService.fileUrlItem$.subscribe(result => {
      this.post.attachments.push(result);
    });
  }

  createPost() {
    this.post.parentId = this.parentId;
    this.postsService.add(this.post).subscribe((data: PostDetail) => {
      this.post.text = '';
      this.post.attachments = [];
      this.posts.splice(0, 0, data);
      this.messageService.addSnackBar('Post created successfully');
    });
  }

  openImageBrowser() {
    const dialogRef = this.dialog.open(ImageBrowserComponent, {
      width: '860px',
      height: '600px',
      data: { extensions: ['.png', '.jpg', '.jpeg', '.gif'] },
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('dialog closed');
    });
  }

  remove(url) {
    this.post.attachments.splice(url, 1);
  }
}

