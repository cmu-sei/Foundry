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
import { ContentDetail, Difficulty, Rating } from '../../../core-api-models';
import { MessageService } from '../../../root/message.service';
import { ContentService } from '../../content.service';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'content-ratings',
  templateUrl: './ratings.component.html',
  styleUrls: ['./ratings.component.scss']
})
export class ContentRatingsComponent extends BaseComponent {
  @Input()
  content: ContentDetail;
  difficultySpin: boolean;
  ratingSpin: boolean;
  ratings: Array<string> = ['Poor', 'Fair', 'Good', 'Great'];
  difficulties: Array<string> = ['Basic', 'Intermediate', 'Advanced'];

  constructor(
    private service: ContentService,
    private msgService: MessageService
  ) {
    super();
  }

  ngOnChanges(): void {
    this.content = this.content[0];
  };

  setDifficulty(difficulty: Difficulty): void {
    this.difficultySpin = true;

    this.$.push(this.service.saveDifficulty(this.content.id, difficulty).
      subscribe(() => {
        var previous = this.content.userDifficulty;
        var unrated: string = Difficulty[Difficulty.Unrated];

        if (difficulty == previous) {
          this.content.difficulty.total--;
          this.content.userDifficulty = Difficulty.Unrated;
          this.msgService.addSnackBar('You set difficulty rating to Unrated');
        }
        else {
          if ((previous.toString() == unrated) || (previous == Difficulty.Unrated)) {
            this.content.difficulty.total++;
          }

          this.content.userDifficulty = difficulty;
          this.msgService.addSnackBar('You set difficulty rating to ' + this.content.userDifficulty);
        }
        this.difficultySpin = false;
      }));
  }

  setRating(rating: Rating): void {
    this.ratingSpin = true;
    this.$.push(this.service.saveRating(this.content.id, rating).
      subscribe(() => {
        var previous = this.content.userRating;
        var unrated: string = Rating[Rating.Unrated];

        if (rating == previous) {
          this.content.rating.total--;
          this.content.userRating = Rating.Unrated;
          this.msgService.addSnackBar('You set rating to Unrated');
        }
        else {
          if ((previous.toString() == unrated) || (previous == Rating.Unrated)) {
            this.content.rating.total++;
          }

          this.content.userRating = rating;
          this.msgService.addSnackBar('You set rating to ' + this.content.userRating);
        }
        this.ratingSpin = false;
      }));
  }
}

