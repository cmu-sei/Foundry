/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Router } from '@angular/router';
import { DragulaService } from 'ng2-dragula';
import { ContentSummary } from '../../core-api-models';

@Component({
    selector: 'content-reorder',
    templateUrl: './content-reorder.component.html',
    styles: [`
        .content-list-item {
            position: relative;
            display: block;
            padding: 10px 15px;
            margin-bottom: -1px;
            border-radius: 4px;
        }
        .content-list-item:hover {
            cursor: pointer;
        }
        .drag-on {
            background-color: lightgray;
        }
        button {
            z-index: 2;
        }
    `]
})

export class ContentReorderComponent {
    @Input() contents: Array<ContentSummary> = [];
      // tslint:disable-next-line:no-output-on-prefix
    @Output() onChanged: EventEmitter<number[]> = new EventEmitter<number[]>();
      // tslint:disable-next-line:no-output-on-prefix
    @Output() onRemoved: EventEmitter<number> = new EventEmitter<number>();

    constructor(
        private router: Router,
        private dragulaService: DragulaService
    ) {

        dragulaService.drag.subscribe((value) => {
            this.onDrag(value.slice(1));
        });
        dragulaService.drop.subscribe((value) => {
            console.log(value);
            this.onDrop(value.slice(1));
        });
    }

    private hasClass(el: any, name: string) {
        return new RegExp('(?:^|\\s+)' + name + '(?:\\s+|$)').test(el.className);
    }

    private addClass(el: any, name: string) {
        if (!this.hasClass(el, name)) {
            el.className = el.className ? [el.className, name].join(' ') : name;
        }
    }

    private removeClass(el: any, name: string) {
        if (this.hasClass(el, name)) {
            el.className = el.className.replace(new RegExp('(?:^|\\s+)' + name + '(?:\\s+|$)', 'g'), ' ');
        }
    }

    private onDrag(args) {
        const [e, el] = args;
        this.addClass(e, 'drag-on');
    }

    private onDrop(args) {
        const [e, el] = args;
        this.removeClass(e, 'drag-on');
        el.innerText.split(/\r|\n|\r\n/).forEach(
            (v, i, a) => {
                const t = this.contents.filter((c) => {
                    console.log(`[${c.name}] [${v.trim()}]`);
                    return c.name === v.trim();
                }).pop();
                if (t) {
                    t.order = i;
                    console.log(t.name + ' ' + t.order);
                }
            });
        console.log(el.innerText);
    }

    remove(id: number): void {
        this.onRemoved.emit(id);
    }
}

