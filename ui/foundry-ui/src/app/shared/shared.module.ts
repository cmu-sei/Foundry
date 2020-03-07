/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// tslint:disable-next-line:max-line-length
import { MatAutocompleteModule, MatButtonModule, MatCardModule, MatCheckboxModule, MatChipsModule, MatDatepickerModule, MatDialogModule, MatExpansionModule, MatIconModule, MatInputModule, MatListModule, MatMenuModule, MatNativeDateModule, MatPaginatorModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule, MatSelectModule, MatSliderModule, MatSlideToggleModule, MatSnackBarModule, MatStepperModule, MatTableModule, MatTabsModule, MatToolbarModule, MatTooltipModule } from '@angular/material';
import { RouterModule } from '@angular/router';
import { DragulaModule } from 'ng2-dragula';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { CommentEditDialog, ContentCommentsComponent } from '../content/components/comments/comments.component';
import { ContentDetailComponent } from '../content/components/detail/detail.component';
import { FlagDialog } from '../content/components/detail/flag-dialog.component';
import { PlaylistSelectComponent } from '../content/components/detail/playlist-select-content.component';
import { ExportDialog } from '../content/components/manage/export.dialog';
import { ContentManageComponent } from '../content/components/manage/manage.component';
import { ContentRatingsComponent } from '../content/components/rating/ratings.component';
import { ContentSidebarListComponent } from '../content/components/sidebar-list/sidebar.component';
import { ContentTileComponent } from '../content/components/tile/tile.component';
import { FileUploadComponent } from '../document/file-upload.component';
import { GroupTileComponent } from '../group/components/tile/tile.component';
import { GroupPlaylistSelectComponent } from '../playlist/components/detail/group-select.component';
import { PlaylistTileComponent } from '../playlist/components/tile/tile.component';
import { GroupProfileSelectComponent } from '../profile/components/detail/group-select.component';
import { AccordionNavDirective } from './accordion-nav.directive';
import { FilterPipe, LocaleDatePipe, SlugifyPipe, TruncatePipe } from './app.pipes';
import { AppendSubmenuIconDirective } from './append-submenu-icon.directive';
import { AutoCloseMobileNavDirective } from './auto-close-mobile-nav.directive';
import { ClickOutsideDirective } from './click-outside.directive';
import { AdvancedSelectDialogComponent } from './components/advanced-select-dialog/advanced-select-dialog.component';
import { BrowserScrollComponent } from './components/browser-scroll/browser-scroll.component';
import { ConfirmDeleteComponent } from './components/confirm-delete.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { ContentReorderComponent } from './components/content-reorder.component';
import { DifficultyDisplayComponent } from './components/difficulty-display/difficulty-display.component';
import { EntityCreatorComponent } from './components/entity-creator.component';
import { ErrorMessageComponent } from './components/error-message/error-message.component';
import { ImageCropComponent } from './components/image-crop/image-crop.component';
import { ImageTileComponent } from './components/imageTile/imageTile.component';
import { MarkdownUrlDivComponent } from './components/markdown-url-div.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { PagerComponent } from './components/pager/pager';
import { PostCreateComponent } from './components/post-create/post-create.component';
import { PostTileComponent } from './components/post-tile/post-tile.component';
import { RatingsDisplayComponent } from './components/ratings-display/ratings-display.component';
import { SearchComponent } from './components/search/search.component';
import { SectionsReorderComponent } from './components/sections-reorder.component';
import { SelectDialogComponent } from './components/select-dialog/select-dialog.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { VideoPlayerComponent } from './components/video-player/video-player.component';
import { HighlightActiveItemsDirective } from './highlight-active-items.directive';
import { HorizontalScrollDirective } from './horizontal-scroll.directive';
import { SlimScrollDirective } from './slim-scroll.directive';
import { ToggleOffcanvasNavDirective } from './toggle-offcanvas-nav.directive';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';

const shared = [
  BrowserScrollComponent,
  SearchComponent,
  SpinnerComponent,
  PageNotFoundComponent,
  VideoPlayerComponent,
  TruncatePipe,
  FilterPipe,
  LocaleDatePipe,
  SlugifyPipe,
  EntityCreatorComponent,
  ConfirmDeleteComponent,
  RatingsDisplayComponent,
  DifficultyDisplayComponent,
  ImageCropComponent,
  MarkdownUrlDivComponent,
  ContentReorderComponent,
  SectionsReorderComponent,
  HighlightActiveItemsDirective,
  ClickOutsideDirective,
  HorizontalScrollDirective,
  AccordionNavDirective,
  AppendSubmenuIconDirective,
  ToggleOffcanvasNavDirective,
  SlimScrollDirective,
  AutoCloseMobileNavDirective,
  ImageTileComponent,
  PagerComponent,
  PostTileComponent,
  PostCreateComponent,
  ConfirmDialogComponent,
  ErrorMessageComponent,
  SelectDialogComponent,
  PlaylistSelectComponent,
  GroupPlaylistSelectComponent,
  GroupProfileSelectComponent,
  ContentSidebarListComponent,
  ExportDialog,
  FlagDialog,
  CommentEditDialog,
  ContentRatingsComponent,
  ContentDetailComponent,
  ContentManageComponent,
  FileUploadComponent,
  ContentCommentsComponent,
  ContentTileComponent,
  PlaylistTileComponent,
  GroupTileComponent,
  UnauthorizedComponent,
  AdvancedSelectDialogComponent
];

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    DragulaModule,
    MatPaginatorModule,
    MatTooltipModule, MatStepperModule, MatSnackBarModule, MatDialogModule, MatMenuModule, MatDatepickerModule,
    MatNativeDateModule, MatTabsModule, MatProgressSpinnerModule, MatIconModule, MatTableModule, MatToolbarModule,
    MatSelectModule, MatChipsModule, MatProgressBarModule, MatCardModule,
    MatSlideToggleModule, MatAutocompleteModule, MatButtonModule, MatListModule,
    MatCheckboxModule, MatRadioModule, MatInputModule, MatSliderModule, MatExpansionModule, InfiniteScrollModule
  ],
  declarations: shared,
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    shared,
    DragulaModule,
    MatPaginatorModule,
    MatTooltipModule, MatStepperModule, MatSnackBarModule, MatDialogModule, MatMenuModule, MatDatepickerModule,
    MatNativeDateModule, MatTabsModule, MatProgressSpinnerModule, MatIconModule, MatTableModule, MatToolbarModule,
    MatSelectModule, MatChipsModule, MatProgressBarModule, MatCardModule,
    MatSlideToggleModule, MatAutocompleteModule, MatButtonModule, MatListModule,
    MatCheckboxModule, MatRadioModule, MatInputModule, MatSliderModule, MatExpansionModule, InfiniteScrollModule
  ],
  entryComponents: [
    SelectDialogComponent,
    GroupPlaylistSelectComponent,
    GroupProfileSelectComponent
  ]
})
export class SharedModule { }

