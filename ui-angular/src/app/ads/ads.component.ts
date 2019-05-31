import { Component, OnInit } from '@angular/core';
import { AdsService } from './ads.service';
import { Ad } from './ad';
import { HttpResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ViewChild} from '@angular/core';
import swal,{ SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@toverux/ngx-sweetalert2';
import { SwalPartialTargets } from '@toverux/ngx-sweetalert2';

import { User } from '../_models';

@Component({
  selector: 'app-ads',
  templateUrl: './ads.component.html',
  styleUrls: ['./ads.component.scss'],
  providers: [AdsService]
})
export class AdsComponent implements OnInit {

  ad: Ad = new Ad();   // изменяемая комната
  ads: Ad[];                // массив комнат
  errorMsg;
  adsCount: number;
  tableMode: boolean = true;          // табличный режим
  editMode: boolean = false;          // режим редактирования
  createMode: boolean = false;		  // режим создания
  page: number = 1;
  size: number = 10;
  
  currentUser: User;

  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  constructor(private adsService: AdsService,
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
			
			this.adsService.createad(this.ad)
                .subscribe((data: HttpResponse<Ad>) => {
					this.loadads();
					this.saveSwal.show();
                    //this.ads.push(data.body);
                },
				(err) => { 
				this.errorMsg = "Ошибка: " + err.error.err;
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
				this.errorMsg = "Ошибка: " + err.error.err;
				this.errorSwal.show();
			});
        }
		this.cancel();
		this.loadads();
		
	}
 
    editad(c: Ad) {
		this.tableMode = false;
		this.editMode = true;
        this.ad = c;
    }
	
    cancel() {
		this.loadads();
        this.ad = new Ad();
        this.tableMode = true;
		this.editMode = false;
		this.createMode = false;
    }
	
    delete(c: Ad) {
	  
	  this.adsService.deletead(c.adid)
            .subscribe(data => this.loadads(),
			(err: any) => { 
			this.errorMsg = "Ошибка: " + err.error.err;
			this.errorSwal.show();
			});
			
	this.cancel();
    }
	
    add() {
        this.tableMode = false;
		this.createMode = true;
    }
	
}
