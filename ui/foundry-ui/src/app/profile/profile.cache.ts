/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Injectable } from '@angular/core';

@Injectable()
export class EntityCache {
  _cache = new Array<CacheItem>();
  cacheTimeout = 300000; // 5 mins in milliseconds
  constructor() {}

  public get(id: string): any {
    const item = this._cache.find(i => i.id === id);
    return (item && (Date.now() - item.ts) < this.cacheTimeout)
      ? item.obj
      : null;
  }

  public set(id: string, item: any): void {
    const index = this._cache.findIndex(i => i.id === id);
    if (index < 0) {
      this._cache.push({ id: id, ts: Date.now(), obj: item});
    } else {
      this._cache.splice(index, 1, item);
    }
  }

  public clear(id: string): void {
    const index = this._cache.findIndex(i => i.id === id);
    if (index >= 0) {
      this._cache.slice(index, 1);
    }
  }
}

export interface CacheItem {
  id?: string;
  ts?: number;
  obj?: any;
}

