/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/


import { Location } from '@angular/common';
import { AfterViewChecked, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { Converter } from 'showdown/dist/showdown';
import { FileStorageResult, GroupCreate, GroupDetail, GroupSummary, GroupUpdate } from '../../../core-api-models';
import { DocumentService } from '../../../document/document.service';
import { ImageBrowserComponent } from '../../../images/components/browser/browser.component';
import { ImagesService } from '../../../images/images.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { GroupService } from '../../group.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'edit-group',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})

export class GroupEditComponent extends BaseComponent implements OnInit, AfterViewChecked {
  @Input()
  group: GroupDetail;
  groups: GroupSummary[];
  childGroups: GroupSummary[];
  memberRequests: any[];
  members: any[];
  id: string;
  navId: number;
  submitted: boolean;
  submitSpin: boolean;
  submitMsg: string;
  errorMsg: string;
  groupForm: NgForm;
  private converter: Converter;
  @ViewChild('groupForm') currentForm: NgForm;
  defaultGroupImagePaths = [];
  renderedDescription: string;
  detailStepCompleted: boolean;
  assetStepCompleted: boolean;
  randomLoading: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: GroupService,
    private location: Location,
    private msgService: MessageService,
    public dialog: MatDialog,
    public settingService: SettingsService,
    public documentService: DocumentService,
    public imageService: ImagesService

  ) {
    super();
    this.converter = new Converter(
      {
        strikethrough: true,
        tables: true,
        parseImgDimensions: true,
        smoothLivePreview: true,
        tasklists: true,
        simpleLineBreaks: true
      });
  }

  ngOnInit() {
    // get groups for parent selector
    this.$.push(this.service.list('').subscribe(result => {
      this.groups = result.results;
    }));

    let groupId = '';
    this.route.params.subscribe((params: Params) => {
      groupId = params.id;
    });

    if (groupId) {
      this.route.params.pipe(
        switchMap((params: Params) => this.service.load(params.id)))
        .subscribe(result => {
          this.group = result;
        });
    } else {
      this.group = {};
    }

    this.$.push(this.imageService.fileItem$.subscribe(result => {
      this.group.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
        result.urlWithExtension;
    }));

    this.$.push(this.imageService.fileUrlItem$.subscribe(result => {
      this.group.logoUrl = result;
    }));
  }

  checkLogoUrl() {
    if (this.group.logoUrl === '' || this.group.logoUrl == null) {
      this.getRandomLogoUrl();
    }
  }

  onSubmit() {
    this.submitted = true;
    this.submitSpin = true;

    if (this.group.id) {
      const model: GroupUpdate = {
        id: this.group.id,
        name: this.group.name,
        description: this.group.description,
        summary: this.group.summary,
        logoUrl: this.group.logoUrl,
        parentId: this.group.parentId
      };
      this.$.push(this.service.update(model).subscribe(
        () => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Group Updated');
          this.goBack();
        },
        error => {
          this.errorMsg = error.error.message;
        }));
    } else {
      const model: GroupCreate = {
        name: this.group.name,
        description: this.group.description,
        summary: this.group.summary,
        logoUrl: this.group.logoUrl,
        parentId: this.group.parentId
      };
      this.$.push(this.service.add(model).subscribe(
        (result) => {
          this.submitSpin = false;
          const groupResult = result as GroupSummary;
          this.msgService.addSnackBar('Group Created');
          this.router.navigateByUrl('group/' + groupResult.id + '/_');
        },
        error => {
          this.submitSpin = false;
          this.errorMsg = error.error.message;
        }));
    }
  }

  delete() {
    this.$.push(this.service.delete(this.group.id).subscribe(result => {
      this.msgService.addSnackBar('Group Deleted');
      this.router.navigateByUrl('group');
    }));
  }

  logoUrlComplete(result: FileStorageResult) {
    this.group.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
      result.file.urlWithExtension;
  }

  ngAfterViewChecked() {
    this.formChanged();
  }

  formChanged() {
    if (this.currentForm === this.groupForm) { return; }
    this.groupForm = this.currentForm;
    if (this.groupForm) {
      this.groupForm.valueChanges
        .subscribe(data => this.onValueChanged(data));
    }
  }

  onValueChanged(data?: any) {
    this.detailStepCompleted = false;
    this.assetStepCompleted = false;

    if (!this.groupForm) { return; }
    const form = this.groupForm.form;

    const name = form.get('name');
    const desc = form.get('description');
    const logoUrl = form.get('logoUrl');

    // tslint:disable-next-line:forin
    for (const field in this.formErrors) {
      // clear previous error message (if any)
      this.formErrors[field] = '';
      const control = form.get(field);

      if (control && (control.dirty || control.touched) && !control.valid) {
        const messages = this.validationMessages[field];
        // tslint:disable-next-line:forin
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
      // if req items are valid allow stepper to proceed
      if ((name && name.valid) && (desc && desc.valid)) {
        this.detailStepCompleted = true;
      }

      if (logoUrl && logoUrl.valid) {
        this.assetStepCompleted = true;
      }
    }
  }


  renderDescription() {
    this.renderedDescription = this.converter.makeHtml(this.group.description);
  }

  // tslint:disable-next-line:member-ordering
  formErrors = {
    'name': '',
    'description': '',
    'logoUrl': '',
    'summary': ''
  };

  // tslint:disable-next-line:member-ordering
  validationMessages = {
    'name': {
      'required': 'Name is required.',
      'minlength': 'Name must be at least 4 characters long.',
      'maxlength': 'Name cannot be more than 70 characters long.',
    },
    'description': {
      'required': 'Description is required.',
      'minlength': 'Description must be at least 4 characters long.',
      'maxlength': 'Description cannot be more than 512 characters long.',
    },
    'summary': {
      'maxlength': 'Summary cannot be more than 256 characters long.',
    },
    'logoUrl': {
      'required': 'Poster Image Url is required.'
    }
  };

  getImage(event) {
    this.group.logoUrl = event;
  }

  getRandomLogoUrl() {
    this.randomLoading = true;
    this.$.push(this.documentService.getRandomFile({ filter: 'tag=group' }).subscribe((result) => {
      this.group.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
        result.urlWithExtension;
      this.randomLoading = false;
    }));
  }

  openImageBrowser() {
    const dialogRef = this.dialog.open(ImageBrowserComponent, {
      width: '850px',
      height: '600px',
      data: {
        extensions: ['.png', '.jpg', '.jpeg', '.gif'],
        currentImage: this.group.logoUrl
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('dialog closed');
    });
  }

  goBack(): void {
    this.location.back();
  }
}

