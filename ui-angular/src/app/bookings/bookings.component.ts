import { Component, OnInit } from '@angular/core';
import { BookingsService } from './bookings.service';

import { CustomersService } from './../customers/customers.service';
import { RoomsService } from './../rooms/rooms.service';

import { Customer } from './../customers/customer';
import { Room } from './../rooms/room';

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
  providers: [BookingsService, CustomersService, RoomsService]
})
export class BookingsComponent implements OnInit {

  booking: Booking = new Booking();  
  bookings: Booking[];                
  
  errorMsg;
  customer: Customer = new Customer();   
  customers: Customer[]; 
  
  room: Room = new Room();
  rooms: Room[]; 
  
  bookingsCount: number;
  tableMode: boolean = true;    // табличный режим
  
  bookinginfo: boolean = false;
  bookingFull: BookingFull;
  
  page: number = 1;
  size: number = 10;

  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  constructor(private bookingsService: BookingsService,
  private customersService: CustomersService,
  private roomsService: RoomsService,
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
        this.loadBookings();    // загрузка данных при старте компонента	
    }

    // получаем данные через сервис
    loadBookings() {	
		this.getBookingsCount();
        this.bookingsService.getBookings(this.page,this.size)
            .subscribe((data: Booking[]) => this.bookings = data,
			(err: any) => { 
			this.errorMsg = "Ошибка: " + err.error.err;
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
	this.bookingsService.getBooking(b.bookingId)
        .subscribe((data: BookingFull) => { this.bookingFull = data; this.bookinginfo = true; });
	}
	
    // сохранение данных
    save() {
        if (this.booking.bookingId == null) {
			
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
 
    editBooking(b) { //b -- букинг со всеми полями, мы выбираем только нужные нам для модели
		this.roomsService.getAllRooms()
            .subscribe((data: Room[]) => this.rooms = data);
		this.customersService.getAllCustomers()
            .subscribe((data: Customer[]) => this.customers = data);
			
        this.booking.bookingId = b.bookingId;
		this.booking.roomId = b.room.roomId;
		this.booking.customerId = b.customer.customerId;
    }

	
    cancel() {
		this.loadBookings();
        this.booking = new Booking();
        this.tableMode = true;
    }
	
    delete(b: Booking) {
	  
	  this.bookingsService.deleteBooking(b.bookingId)
            .subscribe(data => this.loadBookings());
	this.cancel();
    }
	
    add() {
		this.bookinginfo = false;
		this.roomsService.getAllRooms()
            .subscribe((data: Room[]) => this.rooms = data);
		this.customersService.getAllCustomers()
            .subscribe((data: Customer[]) => this.customers = data);
        this.tableMode = false;
    }
	
}
