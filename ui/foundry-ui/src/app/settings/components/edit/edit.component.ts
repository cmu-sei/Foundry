/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { AfterViewChecked, Component, OnInit, ViewChild } from '@angular/core';
import { DragulaService } from 'ng2-dragula';
import { Converter } from 'showdown/dist/showdown';
import { NgForm } from '../../../../../node_modules/@angular/forms';
import { AnalyticsService } from '../../../analytics/analytics.service';
import { ContentService } from '../../../content/content.service';
// tslint:disable-next-line:max-line-length
import { ConfigurationItem, ContentUpdate, PagedResult, PlaylistUpdate, ProfileDetail, SettingCreate, SettingDetail, SettingUpdate } from '../../../core-api-models';
import { DocumentService } from '../../../document/document.service';
import { NotificationService } from '../../../notification/notification.service';
import { PlaylistService } from '../../../playlist/playlist.service';
import { ProfileService } from '../../../profile/profile.service';
import { RequestService } from '../../../request/request.service';
import { MessageService } from '../../../root/message.service';
import { CustomSettingsService } from '../../custom-settings.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { GroupService } from '../../../group/group.service';

@Component({
  selector: 'edit-settings',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.scss']
})

export class SettingsEditComponent extends BaseComponent implements OnInit, AfterViewChecked {

  private converter: Converter;
  profile: ProfileDetail;
  renderedValue: string;
  renderedSimValue: string;
  renderedLicenseValue: string;
  renderedHelpValue: string;
  landingDesc: string;
  setting: SettingDetail;
  simSetting: SettingDetail;
  licenseSetting: SettingDetail;
  settings: PagedResult<SettingDetail>;
  syncSettings: SettingDetail[] = [];
  landingPageText: SettingDetail;
  helpSetting: SettingDetail;
  errorMsg: string;
  submitSpin: boolean;
  isAdmin = false;
  name = 'System Notification';
  description: string;
  notificationForm: NgForm;
  featuredContent: any[] = [];
  featuredPlaylists: any[] = [];
  featuredItems: any[] = [];
  dirty: boolean;
  marketConfigurationItems: Array<ConfigurationItem> = [];
  documentConfigurationItems: Array<ConfigurationItem> = [];
  groupConfigurationItems: Array<ConfigurationItem> = [];
  notificationsConfigurationItems: Array<ConfigurationItem> = [];
  analyticsConfigurationItems: Array<ConfigurationItem> = [];
  requestConfigurationItems: Array<ConfigurationItem> = [];
  apis: Array<any> = [];
  @ViewChild('notificationForm') currentForm: NgForm;

  constructor(
    private profileService: ProfileService,
    private service: CustomSettingsService,
    private notificationService: NotificationService,
    private msgService: MessageService,
    private playlistService: PlaylistService,
    private contentService: ContentService,
    private groupService: GroupService,
    public analyticsService: AnalyticsService,
    public requestService: RequestService,
    public dragulaService: DragulaService,
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

    dragulaService.drag.subscribe((value) => {
      this.onDrag(value.slice(1));
    });
    dragulaService.drop.subscribe((value) => {
      this.onDrop(value.slice(1));
    });
  }

  ngOnInit() {
    this.$.push(this.profileService.profile$.subscribe((p) => this.isAdmin = p.isAdministrator));

    if (this.profileService.profile) { this.isAdmin = this.profileService.profile.isAdministrator; }

    this.apis = this.service.apis;

    this.$.push(this.service.status().subscribe((result) => { this.apis[0].status = result; }));

    this.$.push(this.analyticsService.status().subscribe((result) => { this.apis[2].status = result; }));

    this.$.push(this.notificationService.status().subscribe((result) => { this.apis[1].status = result; }));

    this.$.push(this.requestService.status().subscribe((result) => { this.apis[3].status = result; }));

    this.$.push(this.documentService.status().subscribe((result) => { this.apis[4].status = result; }));

    this.$.push(this.groupService.status().subscribe((result) => { this.apis[5].status = result; console.log(result); }));

    this.$.push(this.service.load('landingPageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          this.setting = result as SettingDetail;
          this.render();
        } else {
          this.setting = {} as SettingDetail;
        }
      }));

    this.$.push(this.service.load('licensePageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          this.licenseSetting = result as SettingDetail;
          this.renderLicense();
        } else {
          this.licenseSetting = {} as SettingDetail;
        }
      }));

    this.$.push(this.service.load('simsPageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          this.simSetting = result as SettingDetail;
          this.renderSim();
        } else {
          this.simSetting = {} as SettingDetail;
        }
      }));

    this.$.push(this.service.load('helpPageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          this.helpSetting = result as SettingDetail;
          this.renderHelp();
        } else {
          this.helpSetting = {} as SettingDetail;
        }
      }));

    this.$.push(this.service.list(null)
      .subscribe((result: PagedResult<SettingDetail>) => {
        if (result) {

          this.settings = result as PagedResult<SettingDetail>;
          this.syncSettings = [];
          result.results.forEach(setting => {
            if (setting.key.match(/Synchronize/)) {
              this.syncSettings.push(setting);
            }
          });
        }
      }));

    this.$.push(this.service.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.marketConfigurationItems = result;
        }
      }));

    this.$.push(this.analyticsService.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.analyticsConfigurationItems = result;
        }
      }));

    this.$.push(this.notificationService.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.notificationsConfigurationItems = result;
        }
      }));

    this.$.push(this.groupService.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.groupConfigurationItems = result;
        }
      }));

    this.$.push(this.documentService.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.documentConfigurationItems = result;
        }
      }));

    this.$.push(this.requestService.configuration()
      .subscribe((result: Array<ConfigurationItem>) => {
        if (result) {
          this.requestConfigurationItems = result;
        }
      }));

    this.getFeaturedItems();
  }

  submitLandingText() {
    this.submitSpin = true;
    if (this.setting.key) {
      const model: SettingUpdate = {
        key: this.setting.key,
        value: this.setting.value
      };

      this.$.push(this.service.update(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Landing page text updated.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    } else {
      const model: SettingCreate = {
        key: 'landingPageText',
        value: this.setting.value
      };

      this.$.push(this.service.add(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Landing page text created.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    }
  }

  submitSimsText() {
    this.submitSpin = true;
    if (this.simSetting.key) {
      const model: SettingUpdate = {
        key: this.simSetting.key,
        value: this.simSetting.value
      };

      this.$.push(this.service.update(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Sims page text updated.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    } else {
      const model: SettingCreate = {
        key: 'simsPageText',
        value: this.simSetting.value
      };

      this.$.push(this.service.add(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Sims page text created.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    }
  }

  submitLicenseText() {
    this.submitSpin = true;
    if (this.licenseSetting.key) {
      const model: SettingUpdate = {
        key: this.licenseSetting.key,
        value: this.licenseSetting.value
      };

      this.$.push(this.service.update(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Licenses page text updated.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    } else {
      const model: SettingCreate = {
        key: 'licensePageText',
        value: this.simSetting.value
      };

      this.$.push(this.service.add(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Licenses page text created.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    }
  }

  submitHelpText() {
    this.submitSpin = true;
    if (this.helpSetting.key) {
      const model: SettingUpdate = {
        key: this.helpSetting.key,
        value: this.helpSetting.value
      };

      this.$.push(this.service.update(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Help page text updated.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    } else {
      const model: SettingCreate = {
        key: 'helpPageText',
        value: this.helpSetting.value
      };

      this.$.push(this.service.add(model)
        .subscribe((result: SettingDetail) => {
          this.submitSpin = false;
          this.msgService.addSnackBar('Help page text created.');
        },
          error => {
            this.submitSpin = false;
            this.errorMsg = error.error.message;
          }
        ));
    }
  }

  render() {
    this.renderedValue = this.converter.makeHtml(this.setting.value);
  }

  renderSim() {
    this.renderedSimValue = this.converter.makeHtml(this.simSetting.value);
  }

  renderLicense() {
    this.renderedLicenseValue = this.converter.makeHtml(this.licenseSetting.value);
  }

  renderHelp() {
    this.renderedHelpValue = this.converter.makeHtml(this.helpSetting.value);
  }


  submitSystemNotification(form: NgForm) {
    this.submitSpin = true;
    if (this.name && this.description) {

      const model = {
        subject: this.name,
        body: this.description
      };

      this.$.push(this.notificationService.add(model).subscribe((result) => {
        this.msgService.addSnackBar('System Notification Added');
        form.resetForm();
      }));
    }
    this.submitSpin = false;
  }

  ngAfterViewChecked() {
    this.formChanged();
  }

  formChanged() {
    if (this.currentForm === this.notificationForm) { return; }
    this.notificationForm = this.currentForm;
    if (this.notificationForm) {
      this.notificationForm.valueChanges
        .subscribe(data => this.onValueChanged(data));
    }
  }

  onValueChanged(data?: any) {
    if (!this.notificationForm) { return; }
    const form = this.notificationForm.form;

    // tslint:disable-next-line:forin
    for (const field in this.formErrors) {
      // clear previous error message (if any)
      this.formErrors[field] = '';
      const control = form.get(field);

      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        // tslint:disable-next-line:forin
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  getFeaturedItems() {
    this.contentService.getFeaturedContent({ filter: 'featured' }).subscribe(data => {
      const itemArray = [];

      this.featuredContent = data[0].results.map(c => {
        const item = {
          ...c, objectType: 'content'

        };
        return item;
      });

      this.featuredContent.forEach(item => {
        itemArray.push(item);
      });

      this.featuredPlaylists = data[1].results.map(c => {
        const item = {
          ...c, objectType: 'playlist'
        };
        return item;
      });

      this.featuredPlaylists.forEach(item => {
        itemArray.push(item);
      });

      this.featuredItems = itemArray.sort(function (x, y) {
        return x.featuredOrder - y.featuredOrder;
      });

    });
  }

  private hasClass(el: any, name: string) {
    return new RegExp('(?:^|\\s+)' + name + '(?:\\s+|$)').test(el.className);
  }

  private addClass(el: any, name: string) {
    if (!this.hasClass(el, name)) {
      el.className = el.className ? [el.className, name].join(' ') : name;
    }
  }

  private removeClass(el: any, name: string) {
    if (this.hasClass(el, name)) {
      el.className = el.className.replace(new RegExp('(?:^|\\s+)' + name + '(?:\\s+|$)', 'g'), ' ');
    }
  }

  private onDrag(args) {
    const [e, el] = args;
    this.addClass(e, 'drag-on');
  }

  private onDrop(args) {
    const [e, el] = args;
    this.removeClass(e, 'drag-on');
    this.dirty = true;
  }

  organize() {
    const items = this.featuredItems.map((c, i) => {
      c.featuredOrder = i;
      return c;
    });

    items.forEach(item => {
      if (item.objectType === 'playlist') {
        const itemTags = [];
        if (item.tags) {
          item.tags.forEach(element => {
            itemTags.push(element.name);
          });
        }

        const model: PlaylistUpdate = {
          ...item,
          tags: itemTags
        };

        this.$.push(this.playlistService.update(model).subscribe(result => {
          item.featuredOrder = result.featuredOrder;
        }));
      }

      if (item.objectType === 'content') {
        const itemTags = [];
        if (item.tags) {
          item.tags.forEach(element => {
            itemTags.push(element.name);
          });
        }

        const model: ContentUpdate = {
          ...item, tags: itemTags
        };

        this.$.push(this.contentService.update(model).subscribe(result => {
          item.featuredOrder = result.featuredOrder;
        }));
      }
    });
    this.msgService.addSnackBar('Order Updated');
  }


  // tslint:disable-next-line:member-ordering
  formErrors = {
    'name': '',
    'description': '',
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
    }
  };

}

