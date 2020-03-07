/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { formatDate } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';

// tslint:disable-next-line:use-pipe-transform-interface
@Pipe({ name: 'limitTo'})
export class TruncatePipe {
  transform(value: string = '', args: string): string {
    const limit = args ? parseInt(args, 10) : 10;
    const trail = '...';
    return value.length > limit ? value.substring(0, limit) + trail : value;
  }
}

@Pipe({ name: 'filter', pure: false})
export class FilterPipe implements PipeTransform {

    transform(items: Array<any>, conditions: {[field: string]: any}): Array<any> {
        return items.filter(item => {
            for (const field in conditions) {
                if (item[field] !== conditions[field]) {
                    return false;
                }
            }
            return true;
        });
    }
}

@Pipe({ name: 'locale' })
export class LocaleDatePipe {
  transform(value: Date, format: string): string {
    var convertedDateString = value.toLocaleString();
    convertedDateString = convertedDateString.replace('at ', '');
    var dt = new Date(convertedDateString);
    var locale = 'en-US';
    var result = formatDate(dt, format, locale);
    return result;
  }
}


@Pipe({name: 'slugify'})
export class SlugifyPipe implements PipeTransform {
  transform(input: string): string {
    return input.toString().toLowerCase()
      .replace(/\s+/g, '-')
      .replace(/[^\w\-]+/g, '')
      .replace(/\-\-+/g, '-')
      .replace(/^-+/, '')
      .replace(/-+$/, '');
  }
}

