/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { GroupSummary } from 'src/app/models';
import { GroupService } from 'src/app/svc/group.service';

@Component({
  selector: 'app-tree-dialog',
  templateUrl: './tree-dialog.component.html',
  styleUrls: ['./tree-dialog.component.scss']
})
export class TreeDialogComponent implements OnInit {
  children: GroupSummary [];
  parent: GroupSummary;
  showParentTreeButton: boolean;

  constructor(
    public dialogRef: MatDialogRef<TreeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private groupSvc: GroupService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadChildren(this.data.group.id);
  }

  loadChildren(id) {
    console.log(id);
    this.groupSvc.listChildren(id).subscribe(g => {
      this.children = g.results;
    });
  }

  loadParent(id) {
    this.groupSvc.load(id).subscribe(g => {
      this.data.group = g;
      this.loadChildren(g.id);
    });
  }

  changeTree(group) {
    this.data.group = group;
    this.loadChildren(group.id);
  }

  viewParent(group) {
    this.router.navigate(['/detail', group.parentId, group.parentSlug]);
    this.dialogRef.close();
  }

  viewChild(group) {
    this.router.navigate(['/detail', group.id, group.slug]);
    this.dialogRef.close();
  }

}

