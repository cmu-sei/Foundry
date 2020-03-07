/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, Inject, OnChanges, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Rating } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { BaseComponent } from '../../../shared/components/base.component';
import { PlaylistService } from '../../playlist.service';

@Component({
    selector: 'playlist-ratings',
    templateUrl: './ratings.component.html',
    styleUrls: ['./ratings.component.scss']
})
export class PlaylistRatingsComponent extends BaseComponent implements OnInit, OnChanges {

    constructor(
        public dialogRef: MatDialogRef<PlaylistRatingsComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private service: PlaylistService,
        private msgService: MessageService
    ) {
      super();
    }

    ngOnInit() {
    }

    ngOnChanges(): void {
        this.data.playlist = this.data.playlist[0];
    }

    setRating(rating): void {
        this.service.saveRating(this.data.playlist.id, rating).
            subscribe(response => {
                const previous = this.data.playlist.userRating;
                const unrated: string = Rating[Rating.Unrated];
                this.data.playlist.rating = response;
                if (rating === previous) {
                    this.data.playlist.rating.total--;
                    this.data.playlist.userRating = Rating.Unrated;
                    this.msgService.addSnackBar('You set rating to Unrated');
                } else {
                    if ((previous.toString() === unrated) || (previous === Rating.Unrated)) {
                        this.data.playlist.rating.total++;
                    }
                    this.data.playlist.userRating = rating;
                    this.msgService.addSnackBar('You set rating to ' + this.data.playlist.userRating);
                }
            }, error => {
                this.msgService.addSnackBar(error.error.message);
            });
        this.dialogRef.close();
    }
}

