export interface AuthUser{
    id: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    email: string;
    userName: string;
    roles: string[];
    isByAdmin: boolean
}