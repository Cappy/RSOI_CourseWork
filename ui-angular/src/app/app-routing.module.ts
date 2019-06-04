import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
// import { CustomersComponent } from './customers/customers.component';
import { AdsComponent } from './ads/ads.component';
import { BookingsComponent } from './bookings/bookings.component';

import { AccountComponent } from './account';
import { LoginComponent } from './login';
import { RegisterComponent } from './register';
import { AuthGuard } from './_guards';

import { HttpClientModule } from '@angular/common/http';
import { HttpModule } from '@angular/http';

const routes: Routes = [
  { path: '', component: AdsComponent, canActivate: [AuthGuard] },
 // { path: '', component: HomeComponent, canActivate: [AuthGuard] },
  // { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard] },
  { path: 'ads', component: AdsComponent, canActivate: [AuthGuard] },
  { path: 'bookings', component: BookingsComponent, canActivate: [AuthGuard] },
  //{ path: 'booking', component: BookingsComponent, canActivate: [AuthGuard] },
  
  
  { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // otherwise redirect to home
  //{ path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
