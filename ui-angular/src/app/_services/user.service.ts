﻿import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from '../_models';

@Injectable()
export class UserService {
    constructor(private http: HttpClient) { }

    // getAll() {
        // return this.http.get<User[]>(`/api/auth/`);
    // }

    getById(id?: string) {
        return this.http.get(`/api/auth/` + id);
    }

    register(user: User) {
        return this.http.post(`/api/auth/register`, user);
    }

    update(user: User) {
        return this.http.put(`/api/auth/` + user.userId, user);
    }

    delete(id?: string) {
        return this.http.delete(`/api/auth/` + id);
    }
}