import { Component, OnInit } from '@angular/core';
import { AdsService } from './ads.service';
import { Ad } from './ad';
import { Booking } from '../bookings/booking';
import { HttpResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { ViewChild} from '@angular/core';
import swal,{ SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@toverux/ngx-sweetalert2';
import { SwalPartialTargets } from '@toverux/ngx-sweetalert2';

import { User } from '../_models';
import { BookingsService } from '../bookings/bookings.service';

@Component({
  selector: 'app-ads',
  templateUrl: './ads.component.html',
  styleUrls: ['./ads.component.scss'],
  providers: [AdsService, BookingsService]
})
export class AdsComponent implements OnInit {

  ad: Ad = new Ad();   // изменяемая комната
  ads: Ad[];                // массив комнат
  
  booking: Booking = new Booking();
  bookings: Booking[]; //брони
  bookingF: FormGroup;
  
  errorMsg;
  adsCount: number;
  tableMode: boolean = true;          // табличный режим
  editMode: boolean = false;          // режим редактирования
  createMode: boolean = false;		  // режим создания
  fullInfoMode: boolean = false;
  bookingMode: boolean = false;
  loading: boolean = false;
  bookingsUndefined: boolean = false;
  page: number = 1;
  size: number = 10;
  
  submitted: boolean = false;
  
  currentUser: User;

  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  constructor(private adsService: AdsService,
  private bookingsService: BookingsService,
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
        this.loadads();    // загрузка данных при старте компонента
		//this.getadsCount();
		this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
		
		this.bookingF = this.formBuilder.group({
			adid: [''],
			userid: [''],
			bookedPrice: [''],
			arrivalDate: ['', Validators.required],
			departureDate: ['', Validators.required]
        });
		
    }
	
	// convenience getter for easy access to form fields
    get f() { return this.bookingF.controls; }
	
	onSubmit(){
		this.submitted = true;

        // stop here if form is invalid
        // if (this.bookingF.invalid) {
            // return;
        // }
		this.loading = true;
        this.bookingsService.createBooking(this.bookingF.value)
            .pipe(first())
            .subscribe(
                data => {
					this.loading = false;
					//this.f.arrivalDate = null;
					//this.f.departureDate = null;
					this.cancel();
					this.saveSwal.show();
                    this.router.navigate(['/ads']);
                },
                (err: any) => {
					this.loading = false;
					this.ads.push(this.ad);
					//this.f.arrivalDate = null;
					//this.f.departureDate = null;
                    this.errorMsg = "Error: " + err.error.err;
					this.errorSwal.show();
                });
	}

    // получаем данные через сервис
    loadads() {	
		this.getadsCount();
        this.adsService.getads(this.page,this.size)
            .subscribe((data: Ad[]) => this.ads = data);
		if (this.page > 0 && this.size > 0)
		{
		this.router.navigate(['/ads'], { queryParams: { page: this.page, size: this.size } });
		}
    }
	
	getadsCount() {
        this.adsService.getAllads()
            .subscribe((data: Ad[]) => this.adsCount = data.length);
    }
	
    // сохранение данных
    save() {
        if (this.ad.adid == null) {
			this.ad.userid = this.currentUser.userid;
			this.adsService.createad(this.ad)
                .subscribe((data: HttpResponse<Ad>) => {
					this.loadads();
					this.saveSwal.show();
                    //this.ads.push(data.body);
                },
				(err) => { 
				this.errorMsg = "Error: " + err.error.err;
				this.errorSwal.show();
			});
            // this.adsservice.createad(this.ad)
				// .subscribe((data: ad) => this.ads.push(data));
        } else {
            this.adsService.updatead(this.ad)
                .subscribe(data => {
					this.loadads();
					this.saveSwal.show();
					},
				(err) => { 
				this.errorMsg = "Error: " + err.error.err;
				this.errorSwal.show();
			});
        }
		this.cancel();
		this.loadads();
		
	}
 
    editad(c: Ad) {
		this.tableMode = false;
		this.fullInfoMode = false;
		this.editMode = true;
        this.ad = c;
    }
	
	fullInfo(c: Ad) {
		
		this.adsService.getAllBookingsByAdId(c.adid)
			.subscribe((data: Booking[]) => {
				console.log(data);
			if (data.length === 0)
			{
				this.bookingsUndefined = true;
			}
			this.bookings = data;
			});
			
		//console.log(this.bookings);
		this.tableMode = false;
		this.fullInfoMode = true;
        this.ad = c;
    }
	
	bookingForm() {
		this.bookingF.reset();
		this.fullInfoMode = false;
		this.bookingMode = true;

    }
	
	closeBooking(){
		//this.f.arrivalDate = null;
		//this.f.departureDate = null;
		this.bookingMode = false;
		this.fullInfoMode = true;
		this.bookingsUndefined = false;
	}
	
    cancel() {
        this.tableMode = true;
		this.editMode = false;
		this.createMode = false;
		this.fullInfoMode = false;
		this.bookingMode = false;
		this.bookingsUndefined = false;
		this.loadads();
		this.loadads();
    }
	
	
    delete(c: Ad) {
	  
	  this.adsService.deletead(c.adid)
            .subscribe(data => {
					this.loadads();
					this.saveSwal.show();
			},
			(err: any) => {
			console.log(err);
			this.errorMsg = "Error: " + err.error.err;
			this.errorSwal.show();
			});
	this.cancel();
	this.loadads();
    }
	
    add() {
		this.ad = new Ad();
        this.tableMode = false;
		this.createMode = true;
    }
	
}
