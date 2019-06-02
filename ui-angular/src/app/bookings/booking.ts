export class Booking {
    constructor(
		public bookingid?: string,
        public adid?: string,
        public userid?: string,
		public bookedPrice?: number,
		public arrivalDate?: Date,
		public departureDate?: Date,
		public createdAt?: Date
		) { }
}