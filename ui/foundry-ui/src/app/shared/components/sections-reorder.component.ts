/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';
import { DragulaService } from 'ng2-dragula';
import { PlaylistDetailSection } from '../../core-api-models';

@Component({
    selector: 'sections-reorder',
    templateUrl: './sections-reorder.component.html',
    styleUrls: ['./sections-reorder.component.scss']
})

export class SectionsReorderComponent implements OnDestroy {
    @Input() sections: Array<PlaylistDetailSection> = [];
    // tslint:disable-next-line:no-output-on-prefix
    @Output() onChanged: EventEmitter<number[]> = new EventEmitter<number[]>();
    name = '';

    sectionOptions: any = {
        removeOnSpill: false,
        moves: function (el, container, target) {
            if (target.classList) {
                return target.classList.contains('handle-section');
            }
            return false;
        }
    };

    contentOptions: any = {
        removeOnSpill: false,
        moves: function (el, container, target) {
            if (target.classList) {
                return target.classList.contains('handle-content');
            }
            return false;
        }
    };

    constructor(
        private dragulaService: DragulaService,
    ) { }

    ngOnDestroy() {
        this.dragulaService.destroy('drag-section');
        this.dragulaService.destroy('drag-content');
    }

    addSection() {
        if (this.name !== '') {
            this.sections.push({ name: this.name, contents: [] });
            this.name = '';
        }
    }

    removeSection(index: number): void {
        if (confirm('Delete this section and all content from the playlist?')) {
            this.sections.splice(index, 1);
        }
    }

    removeContent(section: PlaylistDetailSection, index: number): void {
        if (confirm('Delete this content from this section?')) {
            section.contents.splice(index, 1);
        }
    }
}

