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
import { BucketSummary, PagedResult } from '../../model';
import { BucketService } from '../../svc/bucket.service';
import { BucketManageComponent } from '../bucket-manage/bucket-manage.component';
import { DefaultBucketSelectComponent } from '../default-bucket-select/default-bucket-select.component';
import { EditComponent } from '../edit/edit.component';

@Component({
  selector: 'app-bucket-browser',
  templateUrl: './bucket-browser.component.html',
  styleUrls: ['./bucket-browser.component.scss']
})
export class BucketBrowserComponent implements OnInit {
  pagedResult: PagedResult<BucketSummary>;

  constructor(
    public dialog: MatDialog,
    private bucketService: BucketService
  ) { }

  ngOnInit() {

    this.bucketService.list(null).subscribe((result) => {
      this.pagedResult = result;
    });
  }

  openEditDialog() {
    const dialogRef = this.dialog.open(EditComponent, {
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openManageDialog() {
    const dialogRef = this.dialog.open(BucketManageComponent, {
      width: '800px'
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }

  openDefaultSelectDialog() {
    const dialogRef = this.dialog.open(DefaultBucketSelectComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}

