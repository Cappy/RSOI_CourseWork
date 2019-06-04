import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';

import { HomeComponent } from './home/home.component';
import { AdsComponent } from './ads/ads.component';
import { BookingsComponent } from './bookings/bookings.component';

import { HttpClientModule } from '@angular/common/http';
import { HttpModule } from '@angular/http';


import { NgxPaginationModule } from 'ngx-pagination';
import { SweetAlert2Module } from '@toverux/ngx-sweetalert2';
import { ReactiveFormsModule } from '@angular/forms';

import { OrderModule } from 'ngx-order-pipe';


////////
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { AlertComponent } from './_directives';
import { AuthGuard } from './_guards';
import { JwtInterceptor, ErrorInterceptor } from './_helpers';
import { AlertService, AuthenticationService, UserService } from './_services';
import { AccountComponent } from './account';
import { LoginComponent } from './login';
import { RegisterComponent } from './register';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    AdsComponent,
    BookingsComponent,
	AlertComponent,
    AccountComponent,
    LoginComponent,
    RegisterComponent
  ],
  imports: [
	ReactiveFormsModule,
	OrderModule,
    ReactiveFormsModule,
    SweetAlert2Module.forRoot(),
  	NgxPaginationModule,
    HttpClientModule,
    HttpModule,
    BrowserModule,
    AppRoutingModule,
	FormsModule
	
  ],
  providers: [
	AuthGuard,
    AlertService,
    AuthenticationService,
    UserService,
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
