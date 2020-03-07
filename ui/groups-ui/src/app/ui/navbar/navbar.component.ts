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
import { MatDialog } from '@angular/material';
import { Subscription } from 'rxjs';
import { AccountDetail } from 'src/app/models';
import { AccountService } from 'src/app/svc/account.service';
import { GroupService } from 'src/app/svc/group.service';
import { MessageService } from 'src/app/svc/message.service';
import { AuthService } from '../../svc/auth.service';
import { SettingsService } from '../../svc/settings.service';
import { InviteInputComponent } from '../invite-input/invite-input.component';


@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  title = '';
  account$: Subscription;
  account: AccountDetail;
  migrateInidicator: boolean;
  constructor(
    private settingsSvc: SettingsService,
    private authService: AuthService,
    private accountService: AccountService,
    private groupService: GroupService,
    private messageService: MessageService,
    public inviteDialog: MatDialog
  ) { }

  initAccount(account: AccountDetail) {
    this.account = account;
  }

  ngOnInit() {
    this.accountService.account$.subscribe(account => { this.initAccount(account); });
    this.initAccount(this.accountService.account);

    this.title = this.settingsSvc.settings.branding.applicationName;
  }

  startSigninMainWindow() {
    this.authService.initiateLogin('/');
  }

  logout() {
    this.authService.logout();
  }

  migrateGroups() {
    this.migrateInidicator = true;
    this.groupService.migrate().subscribe(
      result => {
        if (result) {
          this.messageService.addSnackBar('Migation Complete');
          this.messageService.notify('group-update');
          this.migrateInidicator = false;
        }
      },
      error => {
        this.messageService.addSnackBar(error.message);
        this.migrateInidicator = false;
      });
  }


  openInviteInputDialog() {
    const dialogRef = this.inviteDialog.open(InviteInputComponent, {
      width: '600px'
    });
  }

}

