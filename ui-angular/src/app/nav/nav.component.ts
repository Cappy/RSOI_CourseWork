import { Component, OnInit } from '@angular/core';

import { User } from '../_models';
import { UserService } from '../_services';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
	
  appTitle = 'HOTEL';

  currentUser: User;
  users: User[] = [];

  constructor(private userService: UserService) {
        this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
  }

  ngOnInit() {
  }
  
}
