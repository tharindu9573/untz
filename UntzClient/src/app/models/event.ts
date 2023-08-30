import { Image } from "./image";
import { MainEvent } from "./main-event";
import { Ticket } from "./ticket";

export interface UntzEvent{
    id: number,
    createdDate: Date,
    description: string,
    preSaleStartDate?: Date,
    images: Image[],
    location: string,
    eventStartTime: Date,
    entrance: string,
    isActive: boolean,
    mainEvent: MainEvent,
    name: string,
    tickets: Ticket[]
}