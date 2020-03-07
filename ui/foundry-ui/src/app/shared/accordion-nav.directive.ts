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
import * as $ from 'jquery';

@Directive({ selector: '[myAccordionNav]' })

export class AccordionNavDirective implements AfterViewInit {
  el: ElementRef;
  constructor(el: ElementRef) {
    this.el = el;
  }

  ngAfterViewInit() {
    // on click, open it's own ul, close sibling li opened ul & sub ul
    // on click, close it's own ul & sub ul

    const $nav = $(this.el.nativeElement);
    const slideTime = 250;
    const $lists = $nav.find('ul').parent('li');
    $lists.append('<i class="material-icons icon-has-ul">arrow_drop_down</i>');
    const $As = $lists.children('a');

    // Disable a link that has ul
    $As.on('click', (event) => {
      event.preventDefault();
    });

    // Accordion nav
    $nav.on('click', (e) => {

      const target = e.target;
      const $parentLi = $(target).closest('li'); // closest, insead of parent, so it still works when click on i icons
      if (!$parentLi.length) { return; } // return if doesn't click on li
      const $subUl = $parentLi.children('ul');

      // let depth = $subUl.parents().length; // but some li has no sub ul, so...
      const depth = $parentLi.parents().length + 1;

      // filter out all elements (except target) at current depth or greater
      const allAtDepth = $nav.find('ul').filter(function() {
        if ($(this).parents().length >= depth && this !== $subUl.get(0)) {
          return true;
        }
      });
      allAtDepth.slideUp(slideTime).closest('li').removeClass('open');

      // Toggle target
      if ( $parentLi.has('ul').length ) {
        $parentLi.toggleClass('open');
      }
      $subUl.stop().slideToggle(slideTime);

    });
  }
}

