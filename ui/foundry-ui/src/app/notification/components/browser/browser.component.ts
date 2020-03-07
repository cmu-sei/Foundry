/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input } from '@angular/core';
import { DataFilter, NotificationSummary, NotificationSummaryValue, PagedResult } from '../../../core-api-models';
import { NotificationService } from '../../notification.service';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'notification-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss']
})
export class NotificationBrowserComponent extends BaseComponent {
  @Input()
  result: PagedResult<NotificationSummary> = null;
  spin: boolean = false;
  take: number = 10;
  skip: number = 0;
  text: string = '';
  takes: Array<number> = [10, 25, 50, 100, 200];
  working: boolean = false;
  selected: NotificationSummary[] = [];
  all = false;
  selectedCount: number;

  public dataFilter: DataFilter = {
    skip: this.skip,
    term: '',
    take: this.take,
    sort: '-recent',
    filter: 'unread'
  };
  constructor(
    private notificationService: NotificationService,
    private messageService: MessageService
  ) {
    super();
  }

  ngOnInit(): void {
    this.search();
  };

  notificationIcon(values: NotificationSummaryValue[]) {
    return this.notificationService.notificationIcon(values);
  }

  navigate(notification: NotificationSummary) {
    this.markAsRead(notification);
    this.notificationService.navigate(notification);
  }

  updateListeners() {
    this.messageService.notify('notification-update');
  }

  delete(notification: NotificationSummary) {
    if (!this.working) {
      this.working = true;
      this.$.push(this.notificationService.delete(notification.id).subscribe(() => {
        this.working = false;
        this.search();
        this.updateListeners();
        this.messageService.addSnackBar('Notification was Deleted');
      }));
    }
  }

  deleteSelected() {
    if (!this.working) {
      this.working = true;
      var ids = this.selected.map(function (n) { return n.id; });

      this.$.push(this.notificationService.deleteAll(ids).subscribe(() => {
        this.working = false;
        this.search();
        this.clearSelects();
        this.updateListeners();
        this.messageService.addSnackBar('Notifications were Deleted');
      }));
    }
  }

  markAsRead(notification: NotificationSummary) {
    if (!this.working) {
      this.working = true;
      this.$.push(this.notificationService.markAsRead(notification.id).subscribe(() => {
        this.working = false;
        this.search();
        this.updateListeners();
        this.messageService.addSnackBar('Notification marked as Read');
      }));
    }
  }

  markAsUnread(notification: NotificationSummary) {
    if (!this.working) {
      this.working = true;
      this.$.push(this.notificationService.markAsUnread(notification.id).subscribe(() => {
        this.working = false;
        this.search();
        this.updateListeners();
        this.messageService.addSnackBar('Notification marked as Unread');
      }));
    }
  }

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  };

  sort(sort) {
    this.dataFilter.sort = sort;
    this.reset();
  };

  reset() {
    this.dataFilter.skip = 0;
    this.result = null;
    this.search();
  };

  search() {
    if (!this.spin) {
      this.spin = true;

      this.$.push(this.notificationService.list(this.dataFilter).subscribe(result => {
        this.result = result;
        this.spin = false;
        var df = this.result.dataFilter;
        var st = df.skip + 1;
        var en = st + df.take - 1;
        if (en > result.total) en = result.total;
        this.text = st + " to " + en + " of " + result.total;
      }));
    }
  }

  onChange(notification: NotificationSummary, event) {
    this.all = false;
    if (event.target.checked) {
      this.selectedCount++;
      this.selected.push(notification);
    } else {
      for (let i = 0; i < this.selected.length; i++) {
        if (this.selected[i].id === notification.id) {
          this.selected.splice(i, 1);
          this.selectedCount--;
        }
      }
    }
  }

  onAllChange(value, event) {
    this.clearSelects();

    if (event.target.checked) {
      this.all = true;
      for (let i = 0; i < this.result.results.length; i++) {
        var notification = this.result.results[i];
        this.selectedCount++;
        this.selected.push(notification);
      }
    }
  }

  clearSelects() {
    this.selected = [];
    this.all = false;
    this.selectedCount = 0;
  }

  isDisabled(verb) {
    if (verb == 'start') {
      return this.dataFilter.skip == 0;
    }

    if (verb == 'previous') {
      return this.dataFilter.skip == 0;
    }

    if (verb == 'next') {
      return (this.dataFilter.skip + this.dataFilter.take) > this.result.total;
    }

    if (verb == 'end') {
      return (this.dataFilter.skip + this.dataFilter.take) > this.result.total;
    }
  }

  action(verb) {
    if (this.isDisabled(verb)) {
      return false;
    }

    if (verb == 'start') {
      this.dataFilter.skip = 0;
    }

    if (verb == 'previous') {
      this.dataFilter.skip = this.dataFilter.skip - this.dataFilter.take;
    }

    if (verb == 'next') {
      this.dataFilter.skip = this.dataFilter.skip + this.dataFilter.take;
    }

    if (verb == 'end') {
      this.dataFilter.skip = Math.floor(this.result.total / this.dataFilter.take) * this.dataFilter.take;
    }

    this.search();
  }

}

