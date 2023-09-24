import { Author } from "../components/author/author";
import { Publisher } from "../components/publisher/publisher";

export class Book {
    bookId: string = '';
    isbn: string = '';
    title: string = '';
    description: string = '';
    genre: string = '';
    coverUrl: string = '';

    publisherId: string = '';
    authorsIds: string[] = [];
    bookCopies: BookCopy[] = [];
}

export class BookViewModel {
    bookId: string = '';
    isbn: string = '';
    title: string = '';
    description: string = '';
    genre: string = '';
    coverUrl: string = '';
    blobId:string = '';

    publisher: Publisher = new Publisher();
    authors: Author[] = [];
    bookCopies: BookCopy[] = [];
}

export class BookCopy {
    bookItemId: string = '';
    bookId: string = '';
    barcode: string = '';
    bookStatus: BookCopyStatus = 0;

    borrowedDate!: Date;
    returnDate!: Date;
}

export class Genre {
    name: string = '';
}

export enum BookCopyStatus { 
    Available,
    Reserved,
    Borrowed,
    NotAvailable
}