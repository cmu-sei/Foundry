/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input } from '@angular/core';

@Component({
    selector: 'difficulty-display',
    templateUrl: './difficulty-display.component.html',
    styleUrls: ['./difficulty-display.component.scss']
})

export class DifficultyDisplayComponent {
    @Input()
    average: any;
    @Input()
    total: any;

    constructor(
    ) { }

    getDifficultyText(average) {
        if (average > 0 && average <= 1.5) { return 'Difficulty Level: Beginner'; }
        if (average > 1.5 && average <= 2.5) { return 'Difficulty Level: Intermediate'; }
        if (average > 2.5) { return 'Difficulty Level: Advanced'; }
        return 'Difficulty Level: Not Set';
    }
}

