/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatTreeFlatDataSource, MatTreeFlattener } from '@angular/material';
import * as cloneDeep from 'lodash/cloneDeep';
import { AccountDetail, DataFilter, GroupSummary, TreeGroupSummary } from 'src/app/models';
import { AccountService } from 'src/app/svc/account.service';
import { GroupService } from 'src/app/svc/group.service';
import { MessageService } from 'src/app/svc/message.service';

interface FlatNode {
  expandable: boolean;
  name: string;
  level: number;
  id: string;
  key: string;
  slug: string;
}

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
  @ViewChild('term')
  term: ElementRef;
  account: AccountDetail;
  groupList: GroupSummary [];
  public dataFilter: DataFilter = {
    sort: '-recent',
    filter: ''
  };


  private transformer = (node: TreeGroupSummary, level: number) => {
    return {
      expandable: !!node.children && node.children.length > 0,
      name: node.name,
      level: level,
      id: node.id,
      slug: node.slug,
      key: node.key
    };
  }

  // tslint:disable-next-line:member-ordering
  treeControl = new FlatTreeControl<FlatNode>(
    node => node.level, node => node.expandable);

  // tslint:disable-next-line:member-ordering
  treeFlattener = new MatTreeFlattener(
    this.transformer, node => node.level, node => node.expandable, node => node.children);

  // tslint:disable-next-line:member-ordering
  dataSource = new MatTreeFlatDataSource(this.treeControl, this.treeFlattener);

  constructor(
    private groupSvc: GroupService,
    private accountSvc: AccountService,
    private msgSvc: MessageService
  ) {
  }

  hasChild = (_: number, node: FlatNode) => node.expandable;

  initAccount(account: AccountDetail) {
    if (account) {
      this.account = account;
      this.msgSvc.listen().subscribe((m: any) => {
        if (m === 'group-update') {
          this.loadTree();
        }
      });
      this.loadTree();
    }
  }

  ngOnInit() {
    this.accountSvc.account$.subscribe(account => { this.initAccount(account); });
    this.initAccount(this.accountSvc.account);
  }

  loadTree() {
    this.groupList = [];
    this.groupSvc.tree().subscribe(data => {
      this.dataSource.data = data as any;
    });
  }

  loadList() {
    this.groupSvc.list(this.dataFilter).subscribe(data => {
      const results = data.results as GroupSummary[];
      for (let i = 0; i < results.length; i++) {
        this.groupList.push(results[i]);
      }
    });
  }

  listFilter(filter) {
    this.dataFilter.filter = filter;
    this.dataFilter.term = '';
    this.term.nativeElement.value = '';
    this.groupList = [];
    this.loadList();
  }

  filter(term) {
    this.groupList = [];
    this.dataFilter.filter = '';
    if (term) {
      const clonedTreeLocal = cloneDeep(this.dataSource.data);
      this.recursiveTreeFilter(clonedTreeLocal, term);
      this.dataSource.data = clonedTreeLocal;
      this.treeControl.expandAll();
    } else {
      // if term is blank reload tree
      this.loadTree();
    }
  }

  recursiveTreeFilter(tree: Array<TreeGroupSummary>, term): boolean {
    for (let index = tree.length - 1; index >= 0; index--) {
      const node = tree[index];
      if (node.children) {
        const parentCanBeEliminated = this.recursiveTreeFilter(node.children, term);
        if (parentCanBeEliminated) {
          if (node.name.toLocaleLowerCase().indexOf(term.toLocaleLowerCase()) === -1) {
            tree.splice(index, 1);
          }
        }
      } else {
        if (node.name.toLocaleLowerCase().indexOf(term.toLocaleLowerCase()) === -1) {
          tree.splice(index, 1);
        }
      }
    }
    return tree.length === 0;
  }
}

