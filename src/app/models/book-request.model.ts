import { User } from "src/app/models/user";
import { Book } from "./book.model";

export interface BookRequest {
    bookRequestId: string,
    createdAt: Date,
    status: BookRequestStatus,
    requestedById: string,
    bookCopyId: string,
    book: Book,
    requestedBy: User,
}

export class BookRequestModified {
    constructor(
        public bookRequestId: string,
        public createdAt: string,
        public status: string,
        public book: Book,
        public requestedById: string,
        public bookCopyId: string,
        public requestedBy: User
    ) { };
}

enum BookRequestStatus {
    Pending = 0,
    Approved = 1,
    Rejected = 2
}

export const bookRequestStatusMap = new Map([
    [0, 'Pending'],
    [1, 'Approved'],
    [2, 'Rejected'],
]);
