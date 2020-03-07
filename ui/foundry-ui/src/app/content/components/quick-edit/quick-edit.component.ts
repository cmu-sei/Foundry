/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { AfterViewChecked, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { ContentDetail, ContentUpdate } from '../../../core-api-models';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'quick-edit-content',
  templateUrl: './quick-edit.component.html',
  styleUrls: ['./quick-edit.component.scss']
})

export class ContentQuickEditComponent extends BaseComponent implements OnInit, AfterViewChecked {
  submitted: boolean;
  submitSpin: boolean;
  isPowerUser: boolean = false;
  submitMsg: string;
  errorMsg: string;
  contentForm: NgForm;
  @ViewChild('contentForm') currentForm: NgForm;
  @Input() content: ContentDetail;
  tags = [];

  constructor(
    private router: Router,
    private service: ContentService,
    private msgService: MessageService,
    private profileService: ProfileService,
    public dialog: MatDialog,
    public settingService: SettingsService,
    public dialogRef: MatDialogRef<ContentQuickEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe(p => {
      this.isPowerUser = p.isPowerUser;
    }));

    if (this.profileService.profile) {
      this.isPowerUser = this.profileService.profile.isPowerUser;
    }

    this.$.push(this.service.load(this.data.id)
      .subscribe((result: ContentDetail) => {
        this.content = result;

        if (this.content.tags) {
          this.content.tags.forEach(element => {
            this.tags.push(element.name);
          });
        }
      }));
  }

  ngAfterViewChecked() {
    this.formChanged();
  }

  onSubmit() {
    this.submitted = true;
    this.submitSpin = true;

    if (!this.tags.length) {
      this.submitSpin = false;
      this.errorMsg = 'Add at least one content tag before saving.'
    }
    else {
      let model: ContentUpdate = {
        id: this.content.id,
        name: this.content.name,
        description: this.content.description,
        settings: this.content.settings,
        type: this.content.type,
        url: this.content.url,
        logoUrl: this.content.logoUrl,
        publisherId: this.content.publisherId,
        isRecommended: this.content.isRecommended,
        isDisabled: this.content.isDisabled,
        isFeatured: this.content.isFeatured,
        tags: this.tags,
        startDate: this.content.startDate,
        endDate: this.content.endDate,
        startTime: this.content.startTime,
        endTime: this.content.endTime
      }

      this.service.update(model).subscribe(
        () => {
          this.submitSpin = false;
          this.submitMsg = "Content updated";
          this.dialogRef.close();
          this.msgService.addSnackBar("Content Updated");
        },
        error => {
          this.submitSpin = false;
          this.errorMsg = error.error.message;
        });
    }
  }
  removeImg() {
    this.content.logoUrl = null;
  }

  logoUrlComplete(url: string) {
    this.content.logoUrl = url;
  }

  contentUrlComplete(url: string) {
    this.content.url = url;
  }

  formChanged() {
    if (this.currentForm === this.contentForm) { return; }
    this.contentForm = this.currentForm;
    if (this.contentForm) {
      this.contentForm.valueChanges
        .subscribe(data => this.onValueChanged(data));
    }
  }

  onValueChanged(data?: any) {
    if (!this.contentForm) { return; }
    const form = this.contentForm.form;

    for (const field in this.formErrors) {
      // clear previous error message (if any)
      this.formErrors[field] = '';
      const control = form.get(field);

      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  formErrors = {
    'name': '',
    'logoUrl': '',
  };

  validationMessages = {
    'name': {
      'required': 'Name is required.',
      'minlength': 'Name must be at least 4 characters long.',
      'maxlength': 'Name cannot be more than 50 characters long.',
    },
    'logoUrl': {
      'required': 'Poster Image Url is required.'
    }
  };

  getImage(event) {
    this.content.logoUrl = event;
  }

  localHeight: string = "100px";

  getHeight() {
    return this.localHeight;
  }

  setHeight(height) {
    this.localHeight = height;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  navigateToEdit(content: any): void {
    this.dialogRef.close();
    this.router.navigateByUrl("content/edit/" + content.id);
  }

}

