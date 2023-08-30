import { UntzEvent } from "./event";

export interface Ticket{
    id: number,
    name: string,
    price: number,
    event: UntzEvent
}