import { Component, OnInit } from '@angular/core';
import { BookingsService } from './bookings.service';

// import { CustomersService } from './../customers/customers.service';
// import { RoomsService } from './../rooms/rooms.service';

// import { Customer } from './../customers/customer';
// import { Room } from './../rooms/room';

import { User } from '../_models';

import { Booking } from './booking';
import { BookingFull } from './bookingFull';

import { HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ViewChild} from '@angular/core';
import swal,{ SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@toverux/ngx-sweetalert2';
import { SwalPartialTargets } from '@toverux/ngx-sweetalert2';

@Component({
  selector: 'app-bookings',
  templateUrl: './bookings.component.html',
  styleUrls: ['./bookings.component.scss'],
  providers: [BookingsService]
})
export class BookingsComponent implements OnInit {

  booking: Booking = new Booking();  
  bookings: Booking[];                
  
  errorMsg;

  

  bookingsCount: number;
  tableMode: boolean = true;    // табличный режим
  
  bookinginfo: boolean = false;
  bookingFull: BookingFull;
  
  currentUser: User;
  
  page: number = 1;
  size: number = 10;

  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  constructor(private bookingsService: BookingsService,
  private route: ActivatedRoute, private router: Router,
  private formBuilder: FormBuilder, public readonly swalTargets: SwalPartialTargets) 
  {
	this.route.queryParams.subscribe(params => {
		if (params['page']>0) {
			this.page = params['page'];
		}
		if (params['size']>0){
			this.size = params['size'];
		}
    });
	
  }

ngOnInit() {
		this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
		this.loadBookings();    // загрузка данных при старте компонента	
    }

    // получаем данные через сервис
    loadBookings() {	
		this.getBookingsCount();
        this.bookingsService.getBookingsOfUser(this.page,this.size, this.currentUser.userid)
            .subscribe((data: Booking[]) => this.bookings = data,
			(err: any) => { 
			this.errorMsg = "Error: " + err.error.err;
			this.errorSwal.show();
			});

		if (this.page > 0 && this.size > 0)
		{
		this.router.navigate(['/bookings'], { queryParams: { page: this.page, size: this.size } });
		}
    }
	
	getBookingsCount() {
        this.bookingsService.getAllBookings()
            .subscribe((data: Booking[]) => this.bookingsCount = data.length);
    }
	
	getFullBookingInfo(b: Booking) {
	this.bookingsService.getBooking(b.bookingid)
        .subscribe((data: BookingFull) => { this.bookingFull = data; this.bookinginfo = true; });
		console.log(this.bookingFull);
	}
	
    // сохранение данных
    save() {
        if (this.booking.bookingid == null) {
			
			this.bookingsService.createBooking(this.booking)
                .subscribe((data: HttpResponse<Booking>) => {
                    console.log(data);
					this.loadBookings();

                });

        } else {
            this.bookingsService.updateBooking(this.booking)
                .subscribe(data => this.loadBookings());
        }
		this.bookinginfo = false;
		this.cancel();
		this.saveSwal.show();
	}
	
    cancel() {
		console.log(this.bookings);
		this.loadBookings();
        this.booking = new Booking();
        this.tableMode = true;
    }
	
    delete(b: Booking) {
	  this.bookingsService.deleteBooking(b.bookingid)
            .subscribe(data => {
				this.loadBookings();				
				this.saveSwal.show();
			},
			(err: any) => {
			console.log(err);
			this.errorMsg = "Error: " + err.error.err;
			this.errorSwal.show();
			});
	this.cancel();
    }
	
    add() {

    }
	
}
