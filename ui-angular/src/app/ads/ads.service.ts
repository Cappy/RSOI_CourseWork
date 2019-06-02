import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Ad } from './ad';
import { ActivatedRoute } from '@angular/router';
 
@Injectable()
export class AdsService {
	
	page: number;
	size: number;
    private url = "/api/ads";
 
    constructor(private http: HttpClient, private route: ActivatedRoute) {		
	this.route.queryParams.subscribe(params => {
    this.page = params['page'];
    this.size = params['size'];
    });
    }
	
	getAllads() {
        return this.http.get(this.url);
    }
 
    getads(page: number, size: number) {
        return this.http.get(this.url + '?page=' + page + '&size=' + size);
    }
	
	getAllBookingsByAdId(adid: string) {
        return this.http.get(this.url + '/' + adid + '/bookings');
    }
 
    createad(ad: Ad) {
		return this.http.post(this.url, ad, { observe: 'response' });
    }
    updatead(ad: Ad) {
        return this.http.put(this.url + '/' + ad.adid, ad, { observe: 'response', responseType: 'text' });
    }
    deletead(adid: string) {
        return this.http.delete(this.url + '/' + adid);
    }
}