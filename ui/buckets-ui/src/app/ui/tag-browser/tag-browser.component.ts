/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent, MatChipInputEvent, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { FileService } from 'src/app/svc/file.service';
import { MessageService } from 'src/app/svc/message.service';

@Component({
    selector: 'app-tag-browser',
    templateUrl: './tag-browser.component.html',
    styleUrls: ['./tag-browser.component.scss']
})
export class TagBrowserComponent implements OnInit {
    visible = true;
    selectable = true;
    removable = true;
    addOnBlur = false;
    separatorKeysCodes: number[] = [ENTER, COMMA];
    tagControl = new FormControl();
    existingTags: any[];
    tags = [];
    filteredTagOptions: Observable<string[]>;
    allTags: string[] = ['Security', 'Lock', 'Desktop', 'Exercise', 'Lab'];

    @ViewChild('tagInput') tagInput: ElementRef;

    constructor(
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fileService: FileService,
        private msgService: MessageService,
        public dialogRef: MatDialogRef<TagBrowserComponent>,
    ) {
    }

    ngOnInit() {
        if (this.data.file.tags) {
            this.data.file.tags.forEach(element => {
                this.tags.push(element);
            });
        }
        // TODO: Add terms for auto complete
        this.existingTags = [];

        if (this.existingTags) {
            this.filteredTagOptions = this.tagControl.valueChanges.pipe(
                startWith(null),
                map(val => val ? this.filterTagList(val) : this.existingTags.slice()));
        } else {
            this.filteredTagOptions = this.tagControl.valueChanges.pipe(
                startWith(null),
                map(val => val ? this.filterTagList(val) : null));
        }

    }

    filterTagList(val: string): string[] {
        return this.existingTags.filter(option =>
            option.toLowerCase().indexOf(val.toLowerCase()) === 0);
    }

    addTag(event: MatChipInputEvent): void {
        const input = event.input;
        const value = event.value;


        if ((value || '').trim()) {
            this.tags.push(value.trim());
        }

        if (input) {
            input.value = '';
        }

        this.tagControl.setValue(null);
    }

    removeTag(tag: string): void {
        const index = this.tags.indexOf(tag);

        if (index >= 0) {
            this.tags.splice(index, 1);
        }
    }

    selected(event: MatAutocompleteSelectedEvent): void {
        this.tags.push(event.option.viewValue);
        this.tagInput.nativeElement.value = '';
    }

    private _filter(value: string): string[] {
        const filterValue = value.toLowerCase();

        return this.allTags.filter(tag => tag.toLowerCase().indexOf(filterValue) === 0);
    }

    tagClicked() {
        console.log('clicked');
    }

    submit() {
        this.fileService.setTags(this.data.file.id, this.tags).subscribe(
            result => {
                this.fileService.updateFiles();
                this.dialogRef.close();
                this.msgService.addSnackBar('Tags Updated');
            },
            error => {
                console.log(error);
            });
    }
}

