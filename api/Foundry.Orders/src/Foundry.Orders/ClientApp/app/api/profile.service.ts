/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/


import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable, Subject } from 'rxjs';
import { ApiSettings } from "./api-settings";
import { GeneratedProfileService } from "./gen/profile.service";
import { DataFilter, IDataFilterProfile, ProfileSummary } from "./gen/models";
import { AuthService } from "../svc/auth.service";

@Injectable()
export class ProfileService extends GeneratedProfileService {
    profile: ProfileSummary = null;
    profileSource: Subject<ProfileSummary> = new Subject<ProfileSummary>();
    profile$: Observable<ProfileSummary> = this.profileSource.asObservable();
    globalId: string = '';

    constructor(
        protected http: HttpClient,
        protected api: ApiSettings,
        private auth: AuthService) {
        super(http, api);

        this.auth.user$
            .subscribe(u => {
                if (u) {
                    //this.username = u.profile.name;
                    this.globalId = u.profile.sub;
                    this.loadProfile();
                } else {
                    //this.username = '';
                    //this.profile = null;

                }
            });
    }

    public loadProfile(): void {
        this.getProfileByGlobalId(this.globalId).subscribe((data) => {
            this.profile = data;
            this.profileSource.next(this.profile);
            //this.profile.name = this.username;
            //this.update();
        });
    }

    public getProfileByGlobalId(globalId): Observable<ProfileSummary> {
        return this.http.get<ProfileSummary>(this.api.url + "/api/profile/" + globalId);
    }
}

