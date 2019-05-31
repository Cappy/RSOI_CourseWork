import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse} from '@angular/common/http';
import { Customer } from './customer';
import { ActivatedRoute } from '@angular/router';
 
import { throwError, Observable } from 'rxjs'
import { catchError } from 'rxjs/operators'﻿
 
@Injectable()
export class CustomersService {
	
	page: number;
	size: number;
    private url = "/api/customers";
 
    constructor(private http: HttpClient, private route: ActivatedRoute) {		
	this.route.queryParams.subscribe(params => {
    this.page = params['page'];
    this.size = params['size'];
    });
    }
	
	getAllCustomers() {
        return this.http.get(this.url).pipe(
        catchError((err: HttpErrorResponse) => {
            console.log('Handling error...', err);
            return throwError(err);
        })
		);
    }
 
    getCustomers(page: number, size: number) {
        return this.http.get(this.url + '?page=' + page + '&size=' + size).pipe(
        catchError((err: HttpErrorResponse) => {
            console.log('Handling error...', err);
            return throwError(err);
        })
		);
    }
 
    createCustomer(customer: Customer) {
		return this.http.post(this.url, customer, { observe: 'response' });
    }
    updateCustomer(customer: Customer) {
  
        return this.http.put(this.url + '/' + customer.customerId, customer, { observe: 'response', responseType: 'text' });
    }
    deleteCustomer(customerId: string) {
        return this.http.delete(this.url + '/' + customerId).pipe(
        catchError((err: HttpErrorResponse) => {
            console.log('Handling error...', err);
            return throwError(err);
        })
		);
    }
}