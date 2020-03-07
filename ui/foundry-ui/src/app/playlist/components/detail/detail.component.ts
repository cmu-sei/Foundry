/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { BreadcrumbService } from 'angular-crumbs';
import { Observable, Subject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { Converter } from 'showdown/dist/showdown';
import { PlaylistDetail } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { SHOWDOWN_OPTS } from '../../../shared/constants/ui-params';
import { PlaylistService } from '../../playlist.service';
import { PlaylistRatingsComponent } from '../rating/ratings.component';
import { BaseComponent } from '../../../shared/components/base.component';
import { SelectDialogComponent } from '../../../shared/components/select-dialog/select-dialog.component';

@Component({
  selector: 'playlist',
  templateUrl: './detail.component.html',
  styleUrls: ['./detail.component.scss'],
  // tslint:disable-next-line:use-host-property-decorator
  host: { '(window:scroll)': 'moreOnScroll($event)' }
})
export class PlaylistDetailComponent extends BaseComponent implements OnInit {
  @Input() selectedIndex: number;
  public playlist: PlaylistDetail;
  converter: Converter;
  selectedGroupId?: number = null;
  more: boolean;
  spin: boolean;
  error: any;
  submitSpin: boolean;
  showGameLaunch: boolean;
  errorMsg: string;
  filter: string;
  entity: string;
  expanded = true;
  showTrailer: boolean;
  private moreSource: Subject<boolean> = new Subject<boolean>();
  private more$: Observable<boolean> = this.moreSource.asObservable();
  private idSource: Subject<string> = new Subject<string>();
  private id$: Observable<string> = this.idSource.asObservable();
  private hasOnlyBaseRoute: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: PlaylistService,
    private msgService: MessageService,
    private breadcrumbService: BreadcrumbService,
    public dialog: MatDialog,
  ) {
    super();
    this.converter = new Converter(SHOWDOWN_OPTS);
  }

  ngOnInit() {
    this.$.push(
      this.route.params.pipe(switchMap((params: Params) => this.service.load(params['id'])))
        .subscribe(result => {
          if (result == null) {
            return;
          }

          this.playlist = result as PlaylistDetail;

          if (this.playlist.trailerUrl != null) {
            const ext = this.playlist.trailerUrl.substr(this.playlist.trailerUrl.lastIndexOf('.') + 1);
            if (ext === 'mp4' || ext === 'm4v') {
              this.showTrailer = true;
            }
          }

          // if game tag exists, show game launch btn
          if (this.checkTags(this.playlist.tags, 'game')) {
            this.showGameLaunch = true;
          }

          this.breadcrumbService.changeBreadcrumb(this.route.snapshot, this.playlist.name);

          this.loadFirstContent();
        },
          error => {
            this.error = error.error.message;
          }));

    this.route.params.subscribe(p => {
      this.hasOnlyBaseRoute = this.router.url.split('/').length < 5;
      this.idSource.next(p['id']);
    });
  }

  loadFirstContent() {
    let found = false;
    let content = null;

    this.playlist.sections.forEach(s => {
      if (!found) {
        s.contents.forEach(c => {
          if (!found) {
            content = c;
            found = true;
          }
        });
      }
    });

    if (this.hasOnlyBaseRoute && content) {
      this.router.navigate(['content', content.id, content.slug], { relativeTo: this.route });
    }
  }

  follow() {
    this.submitSpin = true;
    this.$.push(this.service.follow(this.playlist.id).subscribe(
      result => {
        this.submitSpin = false;
        this.playlist.isFollowing = result.isFollowing;
        this.playlist.canFollow = result.canFollow;
        this.msgService.addSnackBar('Subscription Added');
      },
      error => {
        this.submitSpin = false;
        this.errorMsg = error.error.message;
      }
    ));
  }

  unFollow() {
    this.submitSpin = true;
    this.$.push(this.service.unfollow(this.playlist.id).subscribe(
      result => {
        this.submitSpin = false;
        this.playlist.isFollowing = result.isFollowing;
        this.playlist.canFollow = result.canFollow;
        this.msgService.addSnackBar('Subscription Removed');
      },
      error => {
        this.submitSpin = false;
        this.errorMsg = error.error.message;
      }
    ));
  }

  renderedDescription() {
    if (this.playlist.description) {
      return this.converter.makeHtml(this.playlist.description);
    }

    return '';
  }

  renderedCopyright() {
    if (this.playlist.copyright) {
      return this.converter.makeHtml(this.playlist.copyright);
    }

    return '';
  }

  moreOnScroll(e) {
    // TODO: this needs refinement -- must consider document.offsetHeight
    this.moreSource.next(window.innerHeight - window.pageYOffset < 20);
  }

  openSelectDialog() {
    const dialogRef = this.dialog.open(SelectDialogComponent, {
      maxHeight: '500px',
      data: {
        id: this.playlist.id,
        type: 'group'
      }
    });
  }

  openRatingDialog() {
    const dialogRef = this.dialog.open(PlaylistRatingsComponent, {
      maxHeight: '400px',
      data: {
        playlist: this.playlist,
      }
    });
  }

  checkTags(arr, val) {
    return arr.some(arrVal => val === arrVal.slug);
  }
}

