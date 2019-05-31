import { Customer } from './../customers/customer';
import { Room } from './../rooms/room';

export class BookingFull {
    constructor(
		public bookingId?: string,
        public customer?: Customer,
        public room?: Room
		) { }
}