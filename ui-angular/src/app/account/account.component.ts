import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { User } from '../_models';
import { Token } from '../_models';
import { AlertService, UserService } from '../_services';

@Component({templateUrl: 'account.component.html'})
export class AccountComponent implements OnInit {
    currentUser: User;
    users: User[] = [];
	OAuth2Token: string;
	
	normalMode: boolean = true;
	editMode: boolean = false;
	bd: string;
	createdAtDate: string;
	
	changeF: FormGroup;
    loading = false;
    submitted = false;

    constructor(private userService: UserService, private alertService: AlertService, private http: HttpClient, private formBuilder: FormBuilder) {
        this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    }

    ngOnInit() {
           this.changeF = this.formBuilder.group({
			userid: ['', Validators.required],
            name: ['', Validators.required],
            surname: ['', Validators.required],
			dateOfBirth: ['', Validators.required],
			email: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(6)]],
			passwordConfirm: ['', [Validators.required, Validators.minLength(6)]],
			createdAt: ['', Validators.required],
			isAdmin: ['', Validators.required],
			isRentlord: ['', Validators.required]
        });
    }
	
	get f() { return this.changeF.controls; }

    deleteUser(id: string) {
        this.userService.delete(id).pipe(first()).subscribe(() => { 
            //this.loadAllUsers() 
        });
    }
	
	changeForm() {
		this.normalMode = false;
		this.editMode = true;
		this.userService.getById(this.currentUser.userid).pipe(first())
            .subscribe((data: any) => {
				this.f.name.setValue(data.name);
				this.f.surname.setValue(data.surname);
				this.f.email.setValue(data.email);
				this.bd = data.dateOfBirth.substring(0,10);
				this.f.dateOfBirth.setValue(this.bd);
				this.f.isAdmin.setValue(data.isAdmin);
				this.f.isRentlord.setValue(data.isRentlord);
				this.f.userid.setValue(data.userid);
				this.createdAtDate = data.createdAt.substring(0,10);
				this.f.createdAt.setValue(this.createdAtDate);
				console.log(this.changeF.value);
                },
                error => {
                    this.alertService.error(error.error.message);
                    this.loading = false;
                });
	console.log(this.f.value);
	}
	
	updateUser(){
		this.submitted = true;

        // stop here if form is invalid
        // if (this.changeForm.invalid) {
            // return;
        // }
		this.loading = true;
		this.userService.update(this.changeF.value).subscribe(data => {
			this.alertService.success('Information updated successfuly', true);	
			this.normalMode = true;
			this.editMode = false;
			this.loading = false;
		},
                error => {
                    this.alertService.error(error.error.message);
                    this.loading = false;
					this.normalMode = true;
                });
	}
	
	cancel()
	{
		this.normalMode = true;
		this.editMode = false;
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