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
import { AfterViewChecked, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, NgForm } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { GroupCreate, GroupDetail, GroupRequestCreate, GroupSummary, GroupUpdate } from 'src/app/models';
import { AuthService } from 'src/app/svc/auth.service';
import { GroupRequestService } from 'src/app/svc/group-request.service';
import { GroupService } from 'src/app/svc/group.service';
import { ImagesService } from 'src/app/svc/images.service';
import { MemberService } from 'src/app/svc/member.service';
import { MessageService } from 'src/app/svc/message.service';
import { SettingsService } from 'src/app/svc/settings.service';
import { ImageBrowserComponent } from '../image-browser/image-browser.component';

@Component({
  selector: 'app-group-edit',
  templateUrl: './group-edit.component.html',
  styleUrls: ['./group-edit.component.scss']
})
export class GroupEditComponent implements OnInit, AfterViewChecked {
  group: GroupDetail;
  groups: GroupSummary[];
  childGroups: GroupSummary[];
  id: string;
  submitSpin: boolean;
  submitMsg: string;
  error: string;
  groupForm: NgForm;
  parentRequestMsg: string;
  parentRequestId: string;
  currentParentId: string;
  @ViewChild('groupForm') currentForm: NgForm;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    public dialog: MatDialog,
    private groupSvc: GroupService,
    private groupReqSvc: GroupRequestService,
    private memberSvc: MemberService,
    private imgSvc: ImagesService,
    private settingSvc: SettingsService,
    private authSvc: AuthService,
    private msgSvc: MessageService,
    private location: Location
  ) { }

  ngOnInit() {
    // get groups for parent selector
    this.groupSvc.list({sort: 'alphabetic'}).subscribe(result => {
      this.groups = result.results.filter(g => g.id !== this.group.id);
    });

    let groupId = '';
    this.route.params.subscribe((params: Params) => {
      groupId = params.id;
    });

    if (groupId) {
      this.route.params.pipe(
        switchMap((params: Params) => this.groupSvc.load(params.id)))
        .subscribe(result => {
          this.group = result;
          this.currentParentId = result.parentId;
        });
    } else {
      this.group = {};
    }

    this.imgSvc.fileItem$.subscribe(result => {
      this.group.logoUrl = this.settingSvc.settings.urls.uploadUrl +
        result.urlWithExtension;
    });

    this.imgSvc.fileUrlItem$.subscribe(result => {
      this.group.logoUrl = result;
    });
  }

  onSubmit() {
    if (this.group.id) {
      const model: GroupUpdate = {
        id: this.group.id,
        name: this.group.name,
        description: this.group.description,
        summary: this.group.summary,
        logoUrl: this.group.logoUrl,
        parentId: this.currentParentId
      };

      this.groupSvc.update(model).subscribe(
        result => {
          this.msgSvc.addSnackBar('Group Updated');
          this.router.navigateByUrl('detail/' + result.id + '/' + result.slug);
          this.updateListeners();
          this.submitParentRequest(result.id);
        },
        error => {
          this.error = error.error.message;
        });
    } else {
      const model: GroupCreate = {
        name: this.group.name,
        description: this.group.description,
        summary: this.group.summary,
        logoUrl: this.group.logoUrl,
        parentId: this.currentParentId
      };

      this.groupSvc.add(model).subscribe(
        result => {
          this.msgSvc.addSnackBar('Group Created');
          this.router.navigateByUrl('detail/' + result.id + '/' + result.slug);
          this.updateListeners();
          this.submitParentRequest(result.id);
        },
        error => {
          this.error = error.error.message;
        });
    }
  }

  loadChildren(id: string) {
    this.groupSvc.listChildren(id).subscribe(result => {
      this.childGroups = result.results;
    });
  }

  updateListeners() {
    this.msgSvc.notify('group-update');
  }

  openImageBrowser() {
    const dialogRef = this.dialog.open(ImageBrowserComponent, {
      width: '850px',
      height: '700px',
      data: {
        extensions: ['.png', '.jpg', '.jpeg', '.gif'],
        currentImage: this.group.logoUrl
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('dialog closed');
    });
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
    }
  }

  submitParentRequest(id) {
    if (this.groupForm.form.valid === true) {
      if (this.parentRequestId) {
        const model: GroupRequestCreate = {
          parentGroupId: this.parentRequestId,
          childGroupId: id
        };
        this.groupReqSvc.addRequest(this.parentRequestId, model).subscribe();
      }
    }
  }
  // if parent selection changes check if user has edit permissions
  onParentChange(e) {
    this.parentRequestMsg = '';
    this.parentRequestId = '';
    // check if model change has a value and is not current parent group
    if (e && e !== this.currentParentId) {
      this.groupSvc.load(e).subscribe(result => {
          if (result.actions.edit) {
            // no request is needed
            this.currentParentId = e;
            this.parentRequestMsg = `This group will be nested within ${result.name}`;
          } else {
            // request will be sent when group edit form is submitted
            this.parentRequestId = e;
            this.parentRequestMsg = `A request will be sent to nest this group within ${result.name}`;
          }
      });
    }
  }

  delete() {
    this.groupSvc.delete(this.group.id).subscribe(result => {
      this.msgSvc.addSnackBar('Group Deleted');
      this.updateListeners();
      this.router.navigateByUrl('/');
    });
  }

  goBack() {
    this.location.back();
  }

  // tslint:disable-next-line:member-ordering
  formErrors = {
    name: '',
    description: '',
    logoUrl: '',
    summary: ''
  };

  // tslint:disable-next-line:member-ordering
  validationMessages = {
    name: {
      required: 'Name is required.',
      minlength: 'Name must be at least 4 characters long.',
      maxlength: 'Name cannot be more than 70 characters long.',
    },
    description: {
      required: 'Description is required.',
      minlength: 'Description must be at least 4 characters long.',
      maxlength: 'Description cannot be more than 512 characters long.',
    },
    summary: {
      maxlength: 'Summary cannot be more than 256 characters long.',
    },
    logoUrl: {
      required: 'Poster Image Url is required.'
    }
  };

}

