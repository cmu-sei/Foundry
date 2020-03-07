/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
    // moduleId: module.id,
    selector: 'confirm-delete',
    template: `
        <button mat-raised-button color="warn" class="btn-sm" *ngIf="!deleteMsgVisible && !asLink" (click)="confirm()">
        <i class="material-icons">delete</i> {{ label }}
        </button>
        <button mat-raised-button color="warn" class="btn-sm" *ngIf="!deleteMsgVisible && asLink" (click)="confirm()">
        <i class="material-icons">delete</i> <span class="text text-danger"> {{ label }}</span>
        </button>
        <p *ngIf="deleteMsgVisible" class="callout callout-danger">
            {{ prompt }}
            <button (click)="delete()" mat-raised-button color="warn" class="btn-sm" >
                <i class="material-icons">delete</i> Delete</button>
            <button (click)="cancel()" mat-raised-button color="default" class="btn-sm" >
               Cancel</button>
        </p>
    `
})
export class ConfirmDeleteComponent implements OnInit {

    deleteMsgVisible: boolean;
    @Input() prompt = 'Please confirm.';
    @Input() label = '';
    @Input() asLink: boolean;
    // tslint:disable-next-line:no-output-on-prefix
    @Output() onDelete: EventEmitter<boolean> = new EventEmitter<boolean>();

    constructor() { }

    ngOnInit() {
    }

    confirm() {
        this.deleteMsgVisible = true;
    }

    cancel() {
        this.deleteMsgVisible = false;
    }

    delete() {
        this.deleteMsgVisible = false;
        this.onDelete.emit(true);
    }
}

