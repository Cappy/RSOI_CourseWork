import { Injectable } from '@angular/core';
import { HttpClient} from '@angular/common/http';
import { Room } from './room';
import { ActivatedRoute } from '@angular/router';
 
@Injectable()
export class RoomsService {
	
	page: number;
	size: number;
    private url = "/api/rooms";
 
    constructor(private http: HttpClient, private route: ActivatedRoute) {		
	this.route.queryParams.subscribe(params => {
    this.page = params['page'];
    this.size = params['size'];
    });
    }
	
	getAllRooms() {
        return this.http.get(this.url);
    }
 
    getRooms(page: number, size: number) {
        return this.http.get(this.url + '?page=' + page + '&size=' + size);
    }
 
    createRoom(room: Room) {
		return this.http.post(this.url, room, { observe: 'response' });
    }
    updateRoom(room: Room) {
  
        return this.http.put(this.url + '/' + room.roomId, room, { observe: 'response', responseType: 'text' });
    }
    deleteRoom(roomId: string) {
        return this.http.delete(this.url + '/' + roomId);
    }
}