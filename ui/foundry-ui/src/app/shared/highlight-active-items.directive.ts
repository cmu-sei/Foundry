/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Directive, ElementRef, AfterViewInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import * as $ from 'jquery';

@Directive({ selector: '[myHighlightActiveItems]' })

export class HighlightActiveItemsDirective implements AfterViewInit {
  constructor(private el: ElementRef, private location: Location, private router: Router) {}

  ngAfterViewInit() {
    const $el = $(this.el.nativeElement);
    const $links = $el.find('a');

    function highlightActive(links) {
      const path = location.hash;

      links.each( (i, link) => {
        const $link = $(link);
        const $li = $link.parent('li');
        const href = $link.attr('href');

        if ($li.hasClass('active')) {
          $li.removeClass('active');
        }
        if ($link.hasClass('active')) {
          $li.addClass('active');
        }
      } );
    }

    highlightActive($links);

    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      highlightActive($links);
    });
  }
}

