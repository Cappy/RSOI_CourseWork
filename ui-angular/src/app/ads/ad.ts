export class Ad {
    constructor(	
		public adid?: string, 
        public userid?: string,
        public caption?: string,
        public city?: string,
        public adress?: string,
        public type?: string,
        public whatRented?: string,
        public bedrooms?: number,
        public beds?: number,
        public bathrooms?: number,
        public description?: string,
        public price?: number,
        public createdAt?: Date) {}
}