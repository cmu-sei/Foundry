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
import { Converter } from 'showdown';
import { SettingDetail } from '../../core-api-models';
import { CustomSettingsService } from '../../settings/custom-settings.service';
import { BaseComponent } from '../../shared/components/base.component';
import { SHOWDOWN_OPTS } from '../../shared/constants/ui-params';

@Component({
  selector: 'app-licenses',
  templateUrl: './licenses.component.html',
  styleUrls: ['./licenses.component.scss']
})
export class LicensesComponent extends BaseComponent implements OnInit {

  constructor(
    private customSettingService: CustomSettingsService
  ) {
    super();
    this.converter = new Converter(SHOWDOWN_OPTS);
  }

  licenseText: string;
  converter: Converter;

  ngOnInit() {
    this.$.push(this.customSettingService.load('licensePageText')
      .subscribe((result: SettingDetail) => {
        if (result) {
          if (result.value.length) {
            this.licenseText = result.value;
          } else {
            // tslint:disable-next-line:max-line-length
            this.licenseText = 'License information appears here.';
          }
        } else {
          // tslint:disable-next-line:max-line-length
          this.licenseText = 'License information appears here.';
        }
      }));
  }

  renderedDescription() {
    return this.converter.makeHtml(this.licenseText);
  }
}

