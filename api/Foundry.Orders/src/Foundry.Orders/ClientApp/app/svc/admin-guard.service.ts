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
import { CanActivate, CanActivateChild, CanLoad,
    Router, Route, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { AuthService } from './auth.service';

@Injectable()
export class AdminGuard implements CanActivate, CanActivateChild {

    constructor(
        private authService: AuthService,
        private router: Router
        ){ }

    canLoad(route: Route): boolean {
        let url = `/${route.path}`;
        //console.log('AdminGuard#canLoad ' + url)
        return this.isAdmin(url);
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
        ) : boolean {

        let url: string = state.url;
        return this.isAdmin(url);
    }

    canActivateChild(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
        ): boolean {

        return this.canActivate(route, state);
    }

    isAdmin(url: string) : boolean {
        //console.log('AdminGuard#isAuthenticated()');

        if (this.authService.isAdmin()) {
            return true;
        }

        this.router.navigate(['notallowed']);
        return false;
    }
}
