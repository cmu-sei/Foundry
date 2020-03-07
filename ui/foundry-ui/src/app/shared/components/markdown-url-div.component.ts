/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Input, OnChanges } from '@angular/core';
import { Http } from '@angular/http';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Converter } from 'showdown/dist/showdown';
import { SHOWDOWN_OPTS } from '../constants/ui-params';

@Component({
    selector: 'markdown-url-div',
    template: `
        <span *ngIf="!renderedHtml" class="fa fa-circle-notched-o fa-2x"></span>
        <div [innerHtml]="renderedHtml"></div>
        <p *ngIf="message" class="alert alert-warning">{{url}} &mdash; {{ message }}</p>
    `
})
export class MarkdownUrlDivComponent implements OnChanges {

    constructor(
        private sanitizer: DomSanitizer,
        private http: Http
    ) { }
    @Input() url: string;
    @Input() safe: boolean;
    renderedHtml: SafeHtml;
    message: string;

    ngOnChanges() {
        this.http.get(this.url).subscribe(
            (response) => {
                const converter = new Converter(SHOWDOWN_OPTS);
                 const html = converter.makeHtml(response.text());
                this.renderedHtml = (this.safe)
                    ? this.sanitizer.bypassSecurityTrustHtml(html)
                    : html;
            },
            (err) => {
                this.message = err.statusText;
            }
        );
    }

}

