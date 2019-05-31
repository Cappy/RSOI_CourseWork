import { Component, OnInit } from '@angular/core';
import { CustomersService } from './customers.service';
import { Customer } from './customer';
import { HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ViewChild} from '@angular/core';
import swal,{ SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@toverux/ngx-sweetalert2';
import { SwalPartialTargets } from '@toverux/ngx-sweetalert2';

import { throwError, Observable } from 'rxjs'
import { catchError } from 'rxjs/operators'﻿

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss'],
  providers: [CustomersService]
})
export class CustomersComponent implements OnInit {

  customer: Customer = new Customer();   // изменяемый клиент
  customers: Customer[];                // массив клиентов
  errorMsg;
  customersCount: number;
  tableMode: boolean = true;          // табличный режим
  page: number = 1;
  size: number = 10;
  
  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  
  constructor(private customersService: CustomersService,
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
		
        this.loadCustomers();    // загрузка данных при старте компонента
		//this.getCustomersCount();
		
    }


    // получаем данные через сервис
    loadCustomers() {	
		this.getCustomersCount();
        this.customersService.getCustomers(this.page,this.size).subscribe((data: Customer[]) => this.customers = data,
			(err: HttpErrorResponse) => { this.errorMsg = "Ошибка: " + err.statusText +
			" (" + err.status + ")" +"\n" + err.message;
			this.errorSwal.show();
			});
		
		if (this.page > 0 && this.size > 0)
		{
		this.router.navigate(['/customers'], { queryParams: { page: this.page, size: this.size } });
		}
    }
	
	getCustomersCount() {
        this.customersService.getAllCustomers()
            .subscribe((data: Customer[]) => this.customersCount = data.length,
			(err: HttpErrorResponse) => { this.errorMsg = "Ошибка: " + err + " (" + err.status + ")";
			this.errorSwal.show();
			});
    }
	
    // сохранение данных
    save() {
        if (this.customer.customerId == null) {
			
			this.customersService.createCustomer(this.customer)
                .subscribe((data: HttpResponse<Customer>) => {
					this.loadCustomers();
					if(data.body != null)
					{
						console.log(data);
						this.saveSwal.show();
					}
                    //this.customers.push(data.body);
                },
			(err: HttpErrorResponse) => { this.errorMsg = "Ошибка: " + err.statusText + " (" + err.status + ")";
			this.errorSwal.show();
			});
            // this.customersservice.createcustomer(this.customer)
				// .subscribe((data: customer) => this.customers.push(data));
        } else {
            this.customersService.updateCustomer(this.customer)
                .subscribe(data => {
					this.loadCustomers();
					if(data.body != null)
					{
						console.log(data);
						this.saveSwal.show();
					}
                    //this.customers.push(data.body);
                },
			(err: HttpErrorResponse) => { this.errorMsg = "Ошибка: " + err.statusText + " (" + err.status + ")";
			this.errorSwal.show();
			});
        }
		this.cancel();
	}
 
    editCustomer(c: Customer) {
        this.customer = c;
    }
	
    cancel() {
		this.loadCustomers();
        this.customer = new Customer();
        this.tableMode = true;
    }
	
    delete(c: Customer) {
	  
	  this.customersService.deleteCustomer(c.customerId)
            .subscribe(data => { this.loadCustomers(); 
			this.saveSwal.show();
			},
			(err: HttpErrorResponse) => { this.errorMsg = "Ошибка: " + err.statusText + " (" + err.status + ")";
			this.errorSwal.show();
			});
	this.cancel();
    }
	
    add() {
        this.tableMode = false;
    }
	
}