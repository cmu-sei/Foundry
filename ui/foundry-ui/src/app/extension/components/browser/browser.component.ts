/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { ApplicationSummary } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { ExtensionService } from '../../extension.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'extension-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class ExtensionBrowserComponent extends BaseComponent implements OnInit {

  constructor(
    private svc: ExtensionService,
    private msgService: MessageService
  ) {
    super();
  }

  applications: ApplicationSummary[];
  spin: boolean;

  ngOnInit(): void {
    this.spin = true;
    this.list();
  }

  list() {
    this.$.push(this.svc.list({ skip: 0, filter: '!hidden' }).subscribe((result) => {
      this.applications = result.results;
      this.spin = false;
    }));
  }

  add(app: ApplicationSummary) {
    if (!app.working) {
      app.working = true;
      this.$.push(this.svc.add(app).subscribe((result) => {
        app.isBookmarked = true;
        app.working = false;
        this.msgService.addSnackBar(app.displayName + ' added to My Apps');
      }));
    }
  }

  remove(app: ApplicationSummary) {
    if (!app.working) {
      app.working = true;
      this.$.push(this.svc.remove(app).subscribe((result) => {
        app.isBookmarked = false;
        app.working = false;
        this.msgService.addSnackBar(app.displayName + ' removed from My Apps');
      }));
    }
  }
}

