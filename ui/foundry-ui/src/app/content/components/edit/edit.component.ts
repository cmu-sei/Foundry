/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/


import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Location } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatChipInputEvent, MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { map, startWith, switchMap } from 'rxjs/operators';
import { Converter } from 'showdown/dist/showdown';
// tslint:disable-next-line:max-line-length
import { ContentCreate, ContentDetail, ContentSummary, ContentType, ContentUpdate, FileStorageResult, GroupSummary } from '../../../core-api-models';
import { Comment } from '../../../discussion/comment';
import { Discussion } from '../../../discussion/discussion';
import { DiscussionService } from '../../../discussion/discussion.service';
import { DocumentService } from '../../../document/document.service';
import { GroupService } from '../../../group/group.service';
import { ImageBrowserComponent } from '../../../images/components/browser/browser.component';
import { ImagesService } from '../../../images/images.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'edit-content',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})

export class ContentEditComponent extends BaseComponent implements OnInit, AfterViewChecked {
  tagControl: FormControl = new FormControl();
  niceTagControl: FormControl = new FormControl();
  showUrlAlert = false;
  DiscussionStatusOpen = 0;
  DiscussionStatusClosed = 1;
  DiscussionTypeContentReview = 0;
  contentTypes: { value: string; name: string }[] = [];
  submitted: boolean;
  submitSpin: boolean;
  isPowerUser = false;
  submitMsg: string;
  errorMsg: string;
  contentForm: NgForm;
  rendered: string;
  groups: GroupSummary[];
  renderedDescription: string;
  renderedCopyright: string;
  randomLoading = false;
  contentExists: ContentSummary[];
  @ViewChild('contentForm') currentForm: NgForm;
  @Input() content: ContentDetail;
  @Input() redirect = true;
  addEnabled = true;
  private converter: Converter;
  startDate: Date;
  endDate: Date;
  startTime: string;
  endTime: string;
  contentIsFile = false;
  id: number;
  discussions: Discussion[];
  discussion: Discussion;
  comments: Comment[];
  existingTags: any[];
  existingNiceTags: any[];
  filteredTagOptions: Observable<string[]>;
  filteredNiceTagOptions: Observable<string[]>;
  detailStepCompleted: boolean;
  visibilityStepCompleted: boolean;
  assetStepCompleted: boolean;
  subscription: any;
  selectedGroup: GroupSummary;
  contentUrl: string;

  // settings for tag input
  @ViewChild('tagInput') tagInput: ElementRef;
  @ViewChild('niceTagInput') niceTagInput: ElementRef;
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = false;
  separatorKeysCodes: number[] = [ENTER, COMMA];

  @Output()
  hideModal = new EventEmitter();

  tags = [];
  niceTags = [];
  nonTypeTags = [];


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: ContentService,
    private msgService: MessageService,
    private discussionService: DiscussionService,
    private profileService: ProfileService,
    private groupService: GroupService,
    private location: Location,
    public dialog: MatDialog,
    public settingService: SettingsService,
    public imageService: ImagesService,
    public documentService: DocumentService
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
    this.profileService.profile$.subscribe(p => {
      this.isPowerUser = p.isPowerUser;
    });

    if (this.profileService.profile) {
      this.isPowerUser = this.profileService.profile.isPowerUser;
    }

    this.loadGroups();
    this.setContentTypes();

    let contentId = 0;
    this.route.params.subscribe((params: Params) => {
      const value = params['id'];
      contentId = (value ? (+ value) : 0);
    });

    if (contentId > 0) {
      this.route.params.pipe(
        switchMap((params: Params) => this.service.load(contentId)))
        .subscribe((result: ContentDetail) => {
          this.content = result;
          if (this.content.tags) {
            this.content.tags.forEach(element => {
              if (element.tagType !== null && element.tagType.toLowerCase() === 'nice') {
                this.niceTags.push(element.name);
              } else {
                this.nonTypeTags.push(element.name);
              }
            });
          }

          // if start or end exists convert for edit display
          if (this.content.start) {
            this.startDate = new Date(this.content.start.toString());
            this.startTime = this.convertTimeForDisplay(this.content.startTime);
          }

          if (this.content.end) {
            this.endDate = new Date(this.content.end.toString());
            this.endTime = this.convertTimeForDisplay(this.content.endTime);
          }

          this.checkContentType(this.content.type.toString());
          this.renderCopyright();
          this.renderDescription();
        });

    } else {
      this.content = { id: 0 } as ContentDetail;
    }

    this.service.listTags(' ').subscribe(result => {
      this.existingTags = result.results.map(t => t.name);
    });

    this.service.listTags({ filter: 'type=nice' }).subscribe(result => {
      this.existingNiceTags = result.results;
    });

    if (this.existingTags) {
      this.filteredTagOptions = this.tagControl.valueChanges.pipe(
        startWith(null),
        map(val => val ? this.filterTagList(val) : this.existingTags.slice()));
    } else {
      this.filteredTagOptions = this.tagControl.valueChanges.pipe(
        startWith(null),
        map(val => val ? this.filterTagList(val) : null));
    }

    if (this.existingNiceTags) {
      this.filteredNiceTagOptions = this.niceTagControl.valueChanges.pipe(
        startWith(null),
        map(val => val ? this.filterNiceTagList(val) : this.existingTags.slice()));
    } else {
      this.filteredNiceTagOptions = this.niceTagControl.valueChanges.pipe(
        startWith(null),
        map(val => val ? this.filterNiceTagList(val) : null));
    }

    this.imageService.fileItem$.subscribe(result => {
      this.content.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
        result.urlWithExtension;
    });

    this.imageService.fileUrlItem$.subscribe(result => {
      this.content.logoUrl = result;
    });
  }

  setContentTypes(): void {
    // tslint:disable-next-line:forin
    for (const n in ContentType) {
      this.contentTypes.push({ value: <any>ContentType[n], name: n });
    }
  }

  loadGroups() {
    this.$.push(this.groupService.list({ filter: 'managed+' }).subscribe(result => {
      this.groups = result.results as GroupSummary[];
    }));
  }

  changeGroup(e) {
    if (e.value) {
      this.$.push(this.groupService.load(e.value).subscribe(result => {
        this.content.publisherName = result.name;
        this.content.publisherSlug = result.slug;
      }));
    } else {
      this.content.publisherName = '';
      this.content.publisherSlug = '';
    }
  }

  checkUrl(url) {
    if (this.content.id > 0) {
      this.$.push(this.service.list({ filter: `url=${url}` }).subscribe(result => {
        this.contentExists = result.results.filter(x => x.id !== this.content.id);
        if (this.contentExists.length > 0) {
          this.showUrlAlert = true;
        } else {
          this.showUrlAlert = false;
        }
      }));
    } else {
      this.$.push(this.service.list({ filter: `url=${url}` }).subscribe(result => {
        this.contentExists = result.results;
        if (this.contentExists.length > 0) {
          this.showUrlAlert = true;
        } else {
          this.showUrlAlert = false;
        }
      }));
    }
  }

  ngAfterViewChecked() {
    this.formChanged();
  }

  // convert date from picker to proper format for storage
  convertDate(date) {
    if (date) {
      return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
    } else {
      return '';
    }
  }

  // convert time input for storage
  convertTime(date) {
    if (date) {
      return moment(date, ['hh:mm']).format('h:mmA');
    } else {
      return '';
    }
  }

  // convert time for time input display
  convertTimeForDisplay(date) {
    if (date) {
      return moment(date, ['h:mm A']).format('HH:mm');
    } else {
      return '';
    }
  }

  onSubmit() {
    this.submitted = true;
    this.submitSpin = true;


    this.tags = this.nonTypeTags.concat(this.niceTags);

    if (!this.tags.length) {
      this.submitSpin = false;
      this.errorMsg = 'Add at least one content tag before saving.';
    } else {
      if (this.content.id > 0) {
        const model: ContentUpdate = {
          id: this.content.id,
          name: this.content.name,
          description: this.content.description,
          copyright: this.content.copyright,
          settings: this.content.settings,
          summary: this.content.summary,
          type: this.content.type,
          url: this.content.url,
          logoUrl: this.content.logoUrl,
          trailerUrl: this.content.trailerUrl,
          publisherId: this.content.publisherId,
          publisherName: this.content.publisherName,
          publisherSlug: this.content.publisherSlug,
          isRecommended: this.content.isRecommended,
          isDisabled: this.content.isDisabled,
          isFeatured: this.content.isFeatured,
          tags: this.tags,
          startDate: this.convertDate(this.startDate),
          endDate: this.convertDate(this.endDate),
          startTime: this.startTime,
          endTime: this.endTime
        };

        this.$.push(this.service.update(model).subscribe(
          result => {
            this.submitSpin = false;
            this.submitMsg = 'Content updated';
            this.msgService.addSnackBar('Content Updated');
            this.goBack();
          },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }));
      } else {
        const model: ContentCreate = {
          name: this.content.name,
          description: this.content.description,
          copyright: this.content.copyright,
          settings: this.content.settings,
          summary: this.content.summary,
          type: this.content.type,
          url: this.content.url,
          logoUrl: this.content.logoUrl,
          trailerUrl: this.content.trailerUrl,
          publisherId: this.content.publisherId,
          publisherName: this.content.publisherName,
          publisherSlug: this.content.publisherSlug,
          isRecommended: this.content.isRecommended,
          isDisabled: this.content.isDisabled,
          isFeatured: this.content.isFeatured,
          tags: this.tags,
          startDate: this.convertDate(this.startDate),
          endDate: this.convertDate(this.endDate),
          startTime: this.startTime,
          endTime: this.endTime
        };
        this.$.push(this.service.add(model)
          .subscribe((result: ContentDetail) => {
            this.content.id = result.id;
            this.openDialog();
            this.tags = [];
            this.contentForm.resetForm();
          },
            error => {
              this.submitSpin = false;
              this.errorMsg = error.error.message;
            }));
      }
    }
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.content.name + ' has been added',
        message: 'What would you like to do next?',
        yesText: 'View ' + this.content.name,
        yesCallback: this.goToContent,
        noText: 'Add More Content',
        noCallback: this.newContent,
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(() => { });
  }

  goToContent(edit) {
    edit.router.navigateByUrl('content/' + edit.content.id + '/_');
  }

  newContent(edit) {
    edit.content = { id: 0, tags: [] };
    window.location.reload();
  }

  toggleComments() {
    if (this.discussion.status === this.DiscussionStatusOpen) {
      this.discussion.status = this.DiscussionStatusClosed;
    } else {
      this.discussion.status = this.DiscussionStatusOpen;
    }

    this.discussionService.save(this.discussion).subscribe(() => { });
  }

  removeImg() {
    this.content.logoUrl = null;
  }

  logoUrlComplete(result: FileStorageResult) {
    this.content.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
      result.file.urlWithExtension;
  }

  trailerUrlComplete(result: FileStorageResult) {
    this.content.trailerUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
      result.file.urlWithExtension;
  }

  contentUrlComplete(result: FileStorageResult) {
    this.content.url = this.settingService.settings.clientSettings.urls.uploadUrl +
    result.file.urlWithExtension;
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
    this.detailStepCompleted = false;
    this.visibilityStepCompleted = false;
    this.assetStepCompleted = false;

    if (!this.contentForm) { return; }

    const form = this.contentForm.form;

    // Get required form items for step completed check
    const name = form.get('name');
    const desc = form.get('description');
    const type = form.get('contentType');
    const logoUrl = form.get('logoUrl');

    // tslint:disable-next-line:forin
    for (const field in this.formErrors) {
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
      if (type && type.valid) {
        this.visibilityStepCompleted = true;
      }
      if (logoUrl && logoUrl.valid) {
        this.assetStepCompleted = true;
      }
    }
  }

  renderDescription() {
    this.renderedDescription = this.converter.makeHtml(this.content.description);
  }

  renderCopyright() {
    this.renderedCopyright = this.converter.makeHtml(this.content.copyright);
  }

  delete() {
    this.service.delete(this.content.id).subscribe(result => {
      this.router.navigateByUrl('content');
      this.msgService.addSnackBar('Content Deleted');
    });
  }

  // tslint:disable-next-line:member-ordering
  formErrors = {
    'name': '',
    'description': '',
    'copyright': '',
    'summary': '',
    'logoUrl': '',
    'contentType': ''
  };

  // tslint:disable-next-line:member-ordering
  validationMessages = {
    'name': {
      'required': 'Name is required.',
      'minlength': 'Name must be at least 4 characters long.',
      'maxlength': 'Name cannot be more than 50 characters long.',
    },
    'description': {
      'required': 'Description is required.',
      'minlength': 'Description must be at least 4 characters long.',
      'maxlength': 'Description cannot be more than 1024 characters long.'
    },
    'copyright': {
      'minlength': 'Copyright must be at least 4 characters long.',
      'maxlength': 'Copyright cannot be more than 512 characters long.'
    },
    'summary': {
      'maxlength': 'Summary cannot be more than 256 characters long.'
    },
    'logoUrl': {
      'required': 'Poster Image Url is required.'
    },
    'contentType': {
      'required': 'Indicate a content type.'
    }
  };

  getImage(event) {
    this.content.logoUrl = event;
  }

  // tslint:disable-next-line:member-ordering
  localHeight = '100px';

  getHeight() {
    return this.localHeight;
  }

  setHeight(height) {
    this.localHeight = height;
  }

  filterTagList(val: string): string[] {
    return this.existingTags.filter(option =>
      option.toLowerCase().indexOf(val.toLowerCase()) === 0);
  }

  filterNiceTagList(val: string): string[] {
    return this.existingNiceTags.filter(option =>
      option.description.toLowerCase().indexOf(val.toLowerCase()) === 0 ||
      option.name.toLowerCase().indexOf(val.toLowerCase()) === 0);
  }

  addTag(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.nonTypeTags.push(value.trim());
    }
    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  addNiceTag(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.niceTags.push(value.trim());
    }
    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  removeTag(tag: any): void {
    const index = this.nonTypeTags.indexOf(tag);
    if (index >= 0) {
      this.nonTypeTags.splice(index, 1);
    }
  }

  removeNiceTag(tag: any): void {
    const index = this.niceTags.indexOf(tag);
    if (index >= 0) {
      this.niceTags.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.nonTypeTags.push(event.option.viewValue);
    this.tagInput.nativeElement.value = '';
  }

  niceSelected(event: MatAutocompleteSelectedEvent): void {
    this.niceTags.push(event.option.value);
    this.niceTagInput.nativeElement.value = '';
  }

  checkContentType(contentType: string) {
    if (contentType === 'Document' || contentType === 'Image' || contentType === 'Video') {
      this.contentIsFile = true;
    } else {
      this.contentIsFile = false;
    }

    this.checkLogoUrl();
  }

  checkLogoUrl() {
    if (this.content.logoUrl === '' || this.content.logoUrl == null) {
      this.getRandomLogoUrl();
    }
  }

  getRandomLogoUrl() {
    this.randomLoading = true;
    this.$.push(this.documentService.getRandomFile({ filter: 'tag=' + this.content.type }).subscribe((result) => {
      this.content.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
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
        currentImage: this.content.logoUrl
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

