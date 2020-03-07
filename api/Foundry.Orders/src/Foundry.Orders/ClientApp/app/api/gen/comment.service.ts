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
import { Observable } from "rxjs/Observable";
import { ApiSettings } from "../api-settings";
import { GeneratedService } from "./_service";
import { CommentDetail,CommentEdit,DataFilter,IDataFilterComment,PagedResultCommentCommentDetail } from "./models";

@Injectable()
export class GeneratedCommentService extends GeneratedService {

    constructor(
       protected http: HttpClient,
       protected api: ApiSettings
    ) { super(http, api); }

	public getOrderComments(orderId: number, dataFilter: DataFilter) : Observable<PagedResultCommentCommentDetail> {
		return this.http.get<PagedResultCommentCommentDetail>(this.api.url + "/api/order/" + orderId + "/comments" + this.paramify(dataFilter));
	}
	public postOrderComments(orderId: number, model: CommentEdit) : Observable<CommentDetail> {
		return this.http.post<CommentDetail>(this.api.url + "/api/order/" + orderId + "/comments", model);
	}
	public putOrderComment(id: number, model: CommentEdit) : Observable<CommentDetail> {
		return this.http.put<CommentDetail>(this.api.url + "/api/order/" + id + "/comment", model);
	}

}


