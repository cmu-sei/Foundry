/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/svc/auth.service';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {

  errorMessage: string;
  syncErrorMessage: string;

  constructor(
      private route: ActivatedRoute,
      private router: Router,
      private service: AuthService
  ) { }

  ngOnInit() {
      this.route.fragment.subscribe(frag => {
          this.validate(frag);
      });
  }

  validate(frag) {
      this.service.validateLogin(frag)
      .then((user) => {
          if (user && user.state) {
              this.router.navigateByUrl(user.state || '/home');
          }
          },
          (err) => {
              console.log(err);
              if (err) {
              this.errorMessage = err;
              }
              if (this.errorMessage.toString().includes('iat is in the future', 0)) {
                  // tslint:disable-next-line:max-line-length
                  this.syncErrorMessage = 'Error: Your system time is not synchronized with the Sketch server. Please contact your system administrator to correct your system time.';
              } else if (this.errorMessage.toString().includes('exp is in the past', 0)) {
                  // tslint:disable-next-line:max-line-length
                  this.syncErrorMessage = 'Error: Your system time is not synchronized with the Sketch server. Please contact your system administrator to correct your system time.';
              }
          }
      );
  }
}

