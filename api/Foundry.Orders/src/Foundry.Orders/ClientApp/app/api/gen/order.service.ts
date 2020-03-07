/*
Foundry
Copyright 2020 Carnegie Mellon University.
NO WARRANTY. THIS CARNEGIE MELLON UNIVERSITY AND SOFTWARE ENGINEERING INSTITUTE MATERIAL IS FURNISHED ON AN "AS-IS" BASIS. CARNEGIE MELLON UNIVERSITY MAKES NO WARRANTIES OF ANY KIND, EITHER EXPRESSED OR IMPLIED, AS TO ANY MATTER INCLUDING, BUT NOT LIMITED TO, WARRANTY OF FITNESS FOR PURPOSE OR MERCHANTABILITY, EXCLUSIVITY, OR RESULTS OBTAINED FROM USE OF THE MATERIAL. CARNEGIE MELLON UNIVERSITY DOES NOT MAKE ANY WARRANTY OF ANY KIND WITH RESPECT TO FREEDOM FROM PATENT, TRADEMARK, OR COPYRIGHT INFRINGEMENT.
Released under a MIT (SEI)-style license, please see license.txt or contact permission@sei.cmu.edu for full terms.
[DISTRIBUTION STATEMENT A] This material has been approved for public release and unlimited distribution.  Please see Copyright notice for non-US Government use and distribution.
Carnegie Mellon(R) and CERT(R) are registered in the U.S. Patent and Trademark Office by Carnegie Mellon University.
DM20-0194
*/


import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { ApiSettings } from "../api-settings";
import { DataFilter, OrderDetail, OrderEdit, PagedResultOrderOrderSummary } from "./models";
import { GeneratedService } from "./_service";

@Injectable()
export class GeneratedOrderService extends GeneratedService {

    constructor(
       protected http: HttpClient,
       protected api: ApiSettings
    ) { super(http, api); }

	public getOrders(dataFilter: DataFilter) : Observable<PagedResultOrderOrderSummary> {
		return this.http.get<PagedResultOrderOrderSummary>(this.api.url + "/api/orders" + this.paramify(dataFilter));
	}
	public postOrders(model: OrderEdit) : Observable<OrderDetail> {
		return this.http.post<OrderDetail>(this.api.url + "/api/orders", model);
	}
	public getOrder(id: number) : Observable<OrderDetail> {
		return this.http.get<OrderDetail>(this.api.url + "/api/order/" + id);
	}
	public putOrder(id: number, model: OrderEdit) : Observable<OrderDetail> {
		return this.http.put<OrderDetail>(this.api.url + "/api/order/" + id, model);
	}
	public getOrderEdit(id: number) : Observable<OrderEdit> {
		return this.http.get<OrderEdit>(this.api.url + "/api/order/" + id + "/edit");
	}
	public putOrderStatus(id: number, model: string) : Observable<OrderDetail> {
		return this.http.put<OrderDetail>(this.api.url + "/api/order/" + id + "/status", model);
	}
	public postOrderEmail(id: number) : Observable<OrderDetail> {
		return this.http.post<OrderDetail>(this.api.url + "/api/order/" + id + "/email", {});
	}

}

