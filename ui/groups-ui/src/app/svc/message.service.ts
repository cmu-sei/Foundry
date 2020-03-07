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
import { MatSnackBar } from '@angular/material';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class MessageService {
    message: string;
    detailMessage: string;

    constructor(
        public snackBar: MatSnackBar
    ) { }

    private listeners = new Subject<any>();

    listen(): Observable<any> {
        return this.listeners.asObservable();
    }

    notify(m: any) {
        this.listeners.next(m);
    }

    add(message: string) {
        this.message = message;
        if (message.toString().includes('Network Error', 0)) {
            // tslint:disable-next-line:max-line-length
            this.detailMessage = 'Make sure the site\'s certificate is trusted and confirm you have network connectivity to the identity provider';
        }
    }

    addSnackBar(message: string) {
        this.snackBar.open(message, null,
            { duration: 3000 }
        );
    }
}

