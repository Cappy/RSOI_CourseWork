import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

import { User } from '../_models';
import { Token } from '../_models';
import { UserService } from '../_services';

@Component({templateUrl: 'account.component.html'})
export class AccountComponent implements OnInit {
    currentUser: User;
    users: User[] = [];
	OAuth2Token: string;

    constructor(private userService: UserService, private http: HttpClient) {
        this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    }

    ngOnInit() {
        //this.loadAllUsers();
    }

    deleteUser(id: string) {
        this.userService.delete(id).pipe(first()).subscribe(() => { 
            //this.loadAllUsers() 
        });
    }
	
	// getToken(currentUser: User) {
		// return this.http.post<Token>(`/api/auth/get-oauth2-token`, currentUser).subscribe(OAuth2Token => {
			// this.OAuth2Token = OAuth2Token.token;
		// });
	// }

    // private loadAllUsers() {
        // this.userService.getAll().pipe(first()).subscribe(users => { 
            // this.users = users; 
        // });
    // }
}