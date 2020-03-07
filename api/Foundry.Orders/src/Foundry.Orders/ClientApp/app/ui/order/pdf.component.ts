/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, Input } from '@angular/core';
import { OrderDetail } from '../../api/gen/models';
import * as jsPDF from 'jspdf'

@Component({
    selector: 'order-create-pdf',
    template: '<p class="text-right pdf-link" (click)="downloadPdf()"><span matTooltip="Save as pdf" [matTooltipPosition]="position"><i class="fas fa-file-pdf fa-lg"></i></span></p>',
    styleUrls: ['./pdf.component.css'],
    providers: [
        { provide: 'Window',  useValue: window }
      ]
    
})
export class PdfComponent {
    position = 'before';
    constructor(
        @Inject('Window') private window: Window,
    ) {
    }

    @Input() order: OrderDetail;

    downloadPdf() {
        var doc = new jsPDF();
        
        doc.setFontSize(22)
        doc.text(20, 20, this.order.id);
      
        doc.setFontSize(14)
        doc.setFontType('bold')
        doc.text(20, 30, "Objectives");

        doc.setFontType('normal')
        doc.text(20, 38, this.order.objectives);

        doc.addPage();
        doc.save(this.order.id +'_details.pdf');
    }
}
