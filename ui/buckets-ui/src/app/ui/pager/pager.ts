/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PagedResult, DataFilter } from '../../model';

@Component({
    selector: 'pager',
    templateUrl: './pager.html',
    styleUrls: ['./pager.scss']
})

export class PagerComponent<T> {
    @Input() public dataFilter: DataFilter = { skip: 0, term: '', take: 25, sort: '-recent', filter: '' };

    private _pagedResult: PagedResult<T> = { results: [], total: 0, dataFilter: this.dataFilter };

    get pagedResult(): PagedResult<T> {
        return this._pagedResult;
    }

    @Input() set pagedResult(pagedResult: PagedResult<T>) {
        this._pagedResult = pagedResult;

        if (pagedResult == null) {
            this.pageText = 'null';
        }
        else {
            const df = pagedResult.dataFilter;
            const st = pagedResult.total == 0 ? 0 : df.skip + 1;
            let en = st + df.take - 1;
            if (en > pagedResult.total) {
                en = pagedResult.total;
            }
            this.pageText = st + ' to ' + en + ' of ' + pagedResult.total;
        }
    }

    @Output() public onPageChanged: EventEmitter<any> = new EventEmitter();
    @Output() public onPageSizeChanged: EventEmitter<any> = new EventEmitter();
    @Output() public onSearch: EventEmitter<any> = new EventEmitter();

    public pageText: string = '';
    public takes: Array<number> = [10, 25, 50, 100, 200];
    constructor() { }

    isDisabled(verb) {
        if (verb === 'start') {
            return this.dataFilter.skip === 0;
        }

        if (verb === 'previous') {
            return this.dataFilter.skip === 0;
        }

        if (verb === 'next') {
            return (this.dataFilter.skip + this.dataFilter.take) > this.pagedResult.total;
        }

        if (verb === 'end') {
            return (this.dataFilter.skip + this.dataFilter.take) > this.pagedResult.total;
        }
    }

    search() {
        console.log('search');
        this.onSearch.emit(this.dataFilter);
    }

    pageSizeChanged() {
        this.onPageSizeChanged.emit(this.dataFilter);
    }

    pageChanged(verb) {
        if (this.isDisabled(verb)) {
            return false;
        }

        if (verb === 'start') {
            this.dataFilter.skip = 0;
        }

        if (verb === 'previous') {
            this.dataFilter.skip = this.dataFilter.skip - this.dataFilter.take;
        }

        if (verb === 'next') {
            this.dataFilter.skip = this.dataFilter.skip + this.dataFilter.take;
        }

        if (verb === 'end') {
            this.dataFilter.skip = Math.floor(this.pagedResult.total / this.dataFilter.take) * this.dataFilter.take;
        }

        this.onPageChanged.emit(this.dataFilter);
    }
}

