import { User } from './../_models/user';
import { Ad } from './../ads/ad';

export class BookingFull {
    constructor(
		public bookingid?: string,
        public user?: User,
        public ad?: Ad,
		public bookedPrice?: number,
		public arrivalDate?: Date,
        public departureDate?: Date,
        public createdAt?: Date
		) { }
}