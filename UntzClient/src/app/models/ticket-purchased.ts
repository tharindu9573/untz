import { AuthUser } from "./auth-user";
import { GuestUser } from "./guest-user";
import { PaymentMethod } from "./payment-method";
import { QrCode } from "./qr-code";
import { Recipt } from "./recipt";
import { Ticket } from "./ticket";

export interface TicketPurchased{
    id: number,
    reference: number,
    createdTime: Date,
    user: AuthUser,
    guestUser: GuestUser,
    ticket: Ticket,
    paymentMethod: PaymentMethod,
    noOfTickets: number,
    sum: number,
    recipt?: Recipt,
    qrCode?: QrCode[],
    isAdmitted: boolean
}