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
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { AuthFailedComponent } from './auth-failed.component';
import { AuthGuardService } from './auth-guard.service';
import { AuthPendingComponent } from './auth-pending.component';
import { AuthTestComponent } from './auth-test.component';
import { AuthService } from './auth.service';
import { AuthInterceptor } from './http-auth-interceptor';

@NgModule({
    declarations: [
        AuthPendingComponent,
        AuthFailedComponent,
        AuthTestComponent
    ],
    providers: [
        AuthService,
        AuthGuardService,
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true,
        }
    ],
    exports: [
       HttpClientModule,
       HttpModule
    ],
    imports: [
        HttpClientModule,
        CommonModule,
        HttpModule,
        RouterModule.forChild([
            { path: 'auth', component: AuthPendingComponent },
            { path: 'nope', component: AuthFailedComponent },
            { path: 'authtest', component: AuthTestComponent }
        ])
    ]
})
export class AuthModule {
    constructor (@Optional() @SkipSelf() parentModule: AuthModule) {
        if (parentModule) {
            throw new Error(
            'AuthModule is already loaded. Import it in the AppModule only');
        }
    }
}

