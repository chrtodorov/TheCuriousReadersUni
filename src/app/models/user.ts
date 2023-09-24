import { Address } from "./address";

export class User{
    userId: string = '';
    firstName: string = '';
    lastName: string = '';
    emailAddress: string = '';
    phoneNumber: string = '';
    password: string = '';
    roleName: string = '';
    address: Address = new Address();
}

export class UserViewModel{
    userId: string = '';
    firstName: string = '';
    lastName: string = '';
    emailAddress: string = '';
    phoneNumber: string = '';
    password: string = '';
    roleName: string = '';
    address: Address = new Address();
}