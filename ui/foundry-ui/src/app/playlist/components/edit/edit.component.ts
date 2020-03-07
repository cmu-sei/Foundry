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
import { AfterViewChecked, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatChipInputEvent, MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, startWith, switchMap } from 'rxjs/operators';
import { Converter } from 'showdown/dist/showdown';
import { FileStorageResult, GroupSummary, PlaylistCreate, PlaylistDetail, PlaylistSectionUpdate, PlaylistUpdate } from '../../../core-api-models';
import { DocumentService } from '../../../document/document.service';
import { GroupService } from '../../../group/group.service';
import { ImageBrowserComponent } from '../../../images/components/browser/browser.component';
import { ImagesService } from '../../../images/images.service';
import { ProfileService } from '../../../profile/profile.service';
import { MessageService } from '../../../root/message.service';
import { SettingsService } from '../../../root/settings.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { TagService } from '../../../tag/tag.service';
import { PlaylistService } from '../../playlist.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'edit-playlist',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})

export class PlaylistEditComponent extends BaseComponent implements OnInit, AfterViewChecked {
  tagControl: FormControl = new FormControl();
  @Input() playlist: PlaylistDetail;
  submitted: boolean;
  isPublic: boolean;
  submitSpin: boolean;
  submitMsg: string;
  isPowerUser = false;
  errorMsg: string;
  id: number;
  navId: number;
  randomLoading: boolean;
  playlistForm: NgForm;
  @ViewChild('playlistForm') currentForm: NgForm;
  existingTags: any[];
  tags = [];
  filteredTagOptions: Observable<string[]>;
  renderedDescription: string;
  renderedCopyright: string;
  private converter: Converter;
  groups: GroupSummary[];
  selectedGroup: GroupSummary;

  // settings for tag input
  @ViewChild('tagInput') tagInput: ElementRef;
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = false;
  separatorKeysCodes: number[] = [ENTER, COMMA];
  detailStepCompleted: boolean;
  assetStepCompleted: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: PlaylistService,
    private location: Location,
    private msgService: MessageService,
    private tagService: TagService,
    private profileService: ProfileService,
    public dialog: MatDialog,
    public settingService: SettingsService,
    public documentService: DocumentService,
    private groupService: GroupService,
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
    this.$.push(this.profileService.profile$.subscribe(p => {
      this.isPowerUser = p.isPowerUser;
    }));

    if (this.profileService.profile) {
      this.isPowerUser = this.profileService.profile.isPowerUser;
    }

    let playlistId = 0;
    this.route.params.subscribe((params: Params) => {
      const value = params['id'];
      playlistId = (value ? (+ value) : 0);
    });

    this.loadGroups();

    if (playlistId > 0) {
      this.$.push(this.route.params.pipe(
        switchMap((params: Params) => this.service.load(params['id'])))
        .subscribe(result => {
          this.playlist = result as PlaylistDetail;
          this.checkLogoUrl();
          this.isPublic = result.isPublic;
          if (this.playlist.tags) {
            this.playlist.tags.forEach(element => {
              this.tags.push(element.name);
            });
          }

          this.renderCopyright();
          this.renderDescription();
        }));
    } else {
      this.playlist = {} as PlaylistDetail;
      this.checkLogoUrl();
    }

    this.tagService.list(null).subscribe(result => {
      this.existingTags = result.results.map(t => t.name);
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

    this.$.push(this.imageService.fileItem$.subscribe(result => {
      this.playlist.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
        result.urlWithExtension;
    }));

    this.$.push(this.imageService.fileUrlItem$.subscribe(result => {
      this.playlist.logoUrl = result;
    }));
  }

  loadGroups() {
    this.$.push(this.groupService.list({ filter: 'managed+' }).subscribe(result => {
      this.groups = result.results as GroupSummary[];
    }));
  }

  changeGroup(e) {
    if (e.value) {
      this.$.push(this.groupService.load(e.value).subscribe(result => {
        this.playlist.publisherName = result.name;
        this.playlist.publisherSlug = result.slug;
      }));
    } else {
      this.playlist.publisherName = '';
      this.playlist.publisherSlug = '';
    }
  }

  checkLogoUrl() {
    if (this.playlist.logoUrl === '' || this.playlist.logoUrl == null) {
      this.getRandomLogoUrl();
    }
  }

  ngAfterViewChecked() {
    this.formChanged();
  }

  submit(redirect: boolean) {
    this.submitted = true;
    this.submitSpin = true;

    if (this.playlist.id > 0) {
      this.$.push(this.saveSections().subscribe(() => {
        this.$.push(this.saveUpdate().subscribe(
          () => {
            this.submitSpin = false;
            this.msgService.addSnackBar('Playlist Updated.');
            if (redirect) {
              this.goBack();
            }
          },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }));
      }));
    } else {
      this.$.push(this.saveCreate().subscribe(
        result => {
          this.playlist.id = result.id;
          this.submitSpin = false;
          this.openDialog();
          this.tags = [];
          this.playlistForm.resetForm();
        },
        error => {
          this.submitSpin = false;
          this.errorMsg = error.error.message;
        }));
    }
  }

  saveUpdate() {
    const model: PlaylistUpdate = {
      id: this.playlist.id,
      name: this.playlist.name,
      isDefault: this.playlist.isDefault,
      isPublic: this.playlist.isPublic,
      trailerUrl: this.playlist.trailerUrl,
      logoUrl: this.playlist.logoUrl,
      isFeatured: this.playlist.isFeatured,
      isRecommended: this.playlist.isRecommended,
      tags: this.tags,
      copyright: this.playlist.copyright,
      description: this.playlist.description,
      summary: this.playlist.summary,
      publisherId: this.playlist.publisherId,
      publisherName: this.playlist.publisherName,
      publisherSlug: this.playlist.publisherSlug
    };

    return this.service.update(model);
  }

  saveCreate() {
    const model: PlaylistCreate = {
      name: this.playlist.name,
      isDefault: this.playlist.isDefault,
      isPublic: this.playlist.isPublic,
      trailerUrl: this.playlist.trailerUrl,
      logoUrl: this.playlist.logoUrl,
      isFeatured: this.playlist.isFeatured,
      isRecommended: this.playlist.isRecommended,
      tags: this.tags,
      copyright: this.playlist.copyright,
      description: this.playlist.description,
      summary: this.playlist.summary,
      publisherId: this.playlist.publisherId,
      publisherName: this.playlist.publisherName,
      publisherSlug: this.playlist.publisherSlug
    };

    return this.service.add(model);
  }

  openDialog(): void {
    const component = this;
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: this.playlist.name + ' has been added',
        message: 'What would you like to do next?',
        yesText: 'View ' + this.playlist.name,
        yesCallback: this.goToPlaylist,
        noText: 'Add Another Playlist',
        noCallback: this.newPlaylist,
        parent: this
      }
    });
    dialogRef.afterClosed().subscribe(result => { });
  }

  goToPlaylist(edit) {
    edit.router.navigateByUrl('playlist/' + edit.playlist.id + '/_');
  }

  newPlaylist(edit) {
    edit.isRestricted = false;
    edit.playlist = { id: 0 };
    window.location.reload();
  }

  delete() {
    this.$.push(this.service.delete(this.playlist.id).subscribe(() => {
      this.router.navigateByUrl('playlist');
      this.msgService.addSnackBar('Playlist Deleted');
    }));
  }

  saveSections() {
    const sections: Array<PlaylistSectionUpdate> = [];

    // construct model to post
    this.playlist.sections.forEach(section => {
      const contentIds: number[] = [];
      section.contents.forEach(content => {
        contentIds.push(content.id);
      });
      sections.push({ name: section.name, contentIds: contentIds });
    });

    return this.service.organize(this.playlist.id, sections);
  }

  logoUrlComplete(result: FileStorageResult) {
    this.playlist.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
      result.file.urlWithExtension;
  }

  trailerUrlComplete(result: FileStorageResult) {
    this.playlist.trailerUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
      result.file.urlWithExtension;
  }

  formChanged() {
    if (this.currentForm === this.playlistForm) { return; }
    this.playlistForm = this.currentForm;
    if (this.playlistForm) {
      this.playlistForm.valueChanges
        .subscribe(data => this.onValueChanged(data));
    }
  }

  onValueChanged(data?: any) {
    this.detailStepCompleted = false;
    this.assetStepCompleted = false;

    if (!this.playlistForm) { return; }
    const form = this.playlistForm.form;

    // Get required form items for step completed check
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

  // tslint:disable-next-line:member-ordering
  formErrors = {
    'name': '',
    'description': '',
    'logoUrl': ''
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
      'maxlength': 'Description cannot be more than 1024 characters long.',
    },
    'logoUrl': {
      'required': 'Poster Image Url is required.'
    }
  };

  getImage(event) {
    this.playlist.logoUrl = event;
  }

  // tag and autocomplete functions
  filterTagList(val: string): string[] {
    return this.existingTags.filter(option =>
      option.toLowerCase().indexOf(val.toLowerCase()) === 0);
  }

  addTag(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    if ((value || '').trim()) {
      this.tags.push(value.trim());
    }
    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  removeTag(tag: any): void {
    const index = this.tags.indexOf(tag);

    if (index >= 0) {
      this.tags.splice(index, 1);
    }
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    this.tags.push(event.option.viewValue);
    this.tagInput.nativeElement.value = '';
  }

  renderDescription() {
    this.renderedDescription = this.converter.makeHtml(this.playlist.description);
  }

  renderCopyright() {
    this.renderedCopyright = this.converter.makeHtml(this.playlist.copyright);
  }

  getRandomLogoUrl() {
    this.randomLoading = true;
    this.$.push(this.documentService.getRandomFile({ filter: 'tag=playlist' }).subscribe((result) => {
      this.playlist.logoUrl = this.settingService.settings.clientSettings.urls.uploadUrl +
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
        currentImage: this.playlist.logoUrl
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

