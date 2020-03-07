/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { PlaylistService } from '../../playlist.service';
import { PlaylistDetail, PlaylistDetailSectionContent } from '../../../core-api-models';
import { Params, ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { BaseComponent } from '../../../shared/components/base.component';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.scss']
})
export class GameBoardComponent extends BaseComponent implements OnInit {
  selected: Question;
  error: string;
  categories: Array<Category> = [];
  rows: Array<Array<Question>> = [];
  showNav: boolean;
  playlist: PlaylistDetail;

  constructor(
    private route: ActivatedRoute,
    private location: Location,
    private playlistService: PlaylistService
  ) {
    super();
  }

  ngOnInit() {
    this.$.push(
      this.route.params.pipe(
        switchMap((params: Params) => this.playlistService.load(params['id'])))
        .subscribe(playlist => {
          this.playlist = playlist;
          this.wire();
        }, error => { }));
  }

  wire() {
    this.playlist.sections.forEach((section) => {

      let rowIndex: number = 0;

      // add no more than 5 categories
      if (this.categories.length < 5) {
        var value = 100;

        let category: Category = { name: section.name, questions: [] };

        section.contents.forEach((content) => {
          // add no more than 5 questions per category
          if (category.questions.length < 5) {
            this.addQuestion(category, content, rowIndex, value);
            value = value + 100;
            rowIndex++;
          }
        });

        // fill in blank question for category
        while (category.questions.length < 5) {
          this.addQuestion(category, null, category.questions.length, 0);
        }

        this.categories.push(category);
      }
    });

    // fill in blank categories
    while (this.categories.length < 5) {
      let category: Category = { name: '-', questions: [] };

      // fill in blank questions for category
      while (category.questions.length < 5) {
        this.addQuestion(category, null, category.questions.length, 0);
      }

      this.categories.push(category);
    }
  }

  addQuestion(category: Category, content: PlaylistDetailSectionContent, rowIndex: number, value: number) {
    let question: Question = { value: value, content: content };

    category.questions.push(question);

    if (this.rows.length < rowIndex + 1) {
      this.rows.push([]);
    }

    this.rows[rowIndex].push(question);
  }

  select(question: Question) {
    this.selected = this.selected == question ? null : question;
  }

  launch(content: PlaylistDetailSectionContent) {

    if (content) {
      window.open(content.url, '_blank');
    }
  }

  back() {
    // if viewing content navigate back to board if not go back one page
    if (this.selected) {
      this.selected = null;
    } else {
      this.location.back();
    }
  }
}

export class Category {
  name: string;
  questions: Array<Question>;
}

export class Question {
  value?: number;
  answer?: string;
  hint?: string;
  content?: PlaylistDetailSectionContent
}

