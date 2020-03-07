/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { CalendarDateFormatter, CalendarEvent, CalendarEventTitleFormatter } from 'angular-calendar';
import { addDays, isSameDay, isSameMonth, subDays } from 'date-fns';
import * as moment from 'moment';
import { Subject } from 'rxjs';
import { ContentService } from '../../../content/content.service';
import { DataFilter } from '../../../core-api-models';
import { CustomDateFormatter } from '../../custom-date-formatter.provider';
import { CustomEventTitleFormatter } from '../../custom-event-title-formatter.provider';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'calendar-browser',
  templateUrl: './browser.component.html',
  styleUrls: ['./browser.component.scss'],
  providers: [
    { provide: CalendarEventTitleFormatter, useClass: CustomEventTitleFormatter },
    { provide: CalendarDateFormatter, useClass: CustomDateFormatter }
  ]
})
export class CalendarBrowserComponent extends BaseComponent implements OnInit {
  @ViewChild('modalContent') modalContent: TemplateRef<any>;
  events: CalendarEvent[];
  displayColor: any;
  view = 'month';
  viewDate: Date = new Date();
  refresh: Subject<any> = new Subject();
  activeDayIsOpen = false;

  public dataFilter: DataFilter = {
    filter: 'contenttype=event'
  };

  colors: any = [
    { primary: '#ad2121', secondary: '#FAE3E3' }, // red
    { primary: '#1e90ff', secondary: '#D1E8FF' }, // yellow
    { primary: '#e3bc08', secondary: '#FDF1BA' }, // blue
    { primary: '#00d200', secondary: '#99FF99' }  // green
  ];

  constructor(
    private service: ContentService,
    private router: Router
  ) {
    super();
  }

  ngOnInit() {
    this.reset();
  }

  filter(filter) {
    this.dataFilter.filter = filter;
    this.reset();
  }

  reset() {
    this.$.push(this.service.list(this.dataFilter).subscribe(data => {
      // to increment the colors array
      let i = 0;

      this.events = data.results.map(c => {
        const eventObject = {
          title: c.name,
          start: subDays(moment(c.startDate + ' ' + c.startTime, 'MM/DD/YYYY HH:mm').toString(), 0),
          end: addDays(moment(c.endDate + ' ' + c.endTime, 'MM/DD/YYYY h:mm a').toString(), 0),
          color: this.colors[i],
          id: c.id,
          slug: c.slug
        };
        i++;

        if (i >= this.colors.length) {
          i = 0;
        }
        return eventObject;
      });
    }));
  }

  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
        this.viewDate = date;
      }
    }
  }

  eventClicked(event) {
    this.router.navigateByUrl('content/' + event.event.id + '/' + event.event.slug);
  }

}

