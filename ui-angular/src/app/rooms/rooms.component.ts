import { Component, OnInit } from '@angular/core';
import { RoomsService } from './rooms.service';
import { Room } from './room';
import { HttpResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { ViewChild} from '@angular/core';
import swal,{ SweetAlertOptions } from 'sweetalert2';
import { SwalComponent } from '@toverux/ngx-sweetalert2';
import { SwalPartialTargets } from '@toverux/ngx-sweetalert2';

@Component({
  selector: 'app-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.scss'],
  providers: [RoomsService]
})
export class RoomsComponent implements OnInit {

  room: Room = new Room();   // изменяемая комната
  rooms: Room[];                // массив комнат
  errorMsg;
  roomsCount: number;
  tableMode: boolean = true;          // табличный режим
  page: number = 1;
  size: number = 10;

  @ViewChild('saveSwal') private saveSwal: SwalComponent;
  @ViewChild('errorSwal') private errorSwal: SwalComponent;
  
  constructor(private roomsService: RoomsService,
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
		
        this.loadRooms();    // загрузка данных при старте компонента
		//this.getRoomsCount();
		
    }

    // получаем данные через сервис
    loadRooms() {	
		this.getRoomsCount();
        this.roomsService.getRooms(this.page,this.size)
            .subscribe((data: Room[]) => this.rooms = data);
		if (this.page > 0 && this.size > 0)
		{
		this.router.navigate(['/rooms'], { queryParams: { page: this.page, size: this.size } });
		}
    }
	
	getRoomsCount() {
        this.roomsService.getAllRooms()
            .subscribe((data: Room[]) => this.roomsCount = data.length);
    }
	
    // сохранение данных
    save() {
        if (this.room.roomId == null) {
			
			this.roomsService.createRoom(this.room)
                .subscribe((data: HttpResponse<Room>) => {
					this.loadRooms();
					this.saveSwal.show();
                    //this.rooms.push(data.body);
                },
				(err) => { 
				this.errorMsg = "Ошибка: " + err.error.err;
				this.errorSwal.show();
			});
            // this.roomsservice.createroom(this.room)
				// .subscribe((data: room) => this.rooms.push(data));
        } else {
            this.roomsService.updateRoom(this.room)
                .subscribe(data => {
					this.loadRooms();
					this.saveSwal.show();
					},
				(err) => { 
				this.errorMsg = "Ошибка: " + err.error.err;
				this.errorSwal.show();
			});
        }
		this.cancel();
		this.loadRooms();
		
	}
 
    editRoom(c: Room) {
        this.room = c;
    }
	
    cancel() {
		this.loadRooms();
        this.room = new Room();
        this.tableMode = true;
    }
	
    delete(c: Room) {
	  
	  this.roomsService.deleteRoom(c.roomId)
            .subscribe(data => this.loadRooms(),
			(err: any) => { 
			this.errorMsg = "Ошибка: " + err.error.err;
			this.errorSwal.show();
			});
			
	this.cancel();
    }
	
    add() {
        this.tableMode = false;
    }
	
}
