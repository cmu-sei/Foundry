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
import { GroupSummary } from 'src/app/models';
import { GroupService } from 'src/app/svc/group.service';

@Component({
  selector: 'app-sidenav-children',
  templateUrl: './sidenav-children.component.html',
  styleUrls: ['./sidenav-children.component.scss']
})
export class SidenavChildrenComponent implements OnInit {
  @Input() groupId: string;
  groups: GroupSummary[];
  isVisible: boolean;
  constructor(
    private groupSvc: GroupService
  ) { }

  ngOnInit() {
    this.groupSvc.listChildren(this.groupId).subscribe(result => {
      this.groups = result.results;
    });
  }

}

