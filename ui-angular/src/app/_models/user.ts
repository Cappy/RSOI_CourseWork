export class User {
	constructor(
    public userid?: string,
	public name?: string,
    public surname?: string,
    public email?: string,
    public password?: string,
	public passwordConfirm?: string,
	public dateOfBirth?: Date,
	public isAdmin?: boolean,
	public isRentlord?: boolean) {}
}