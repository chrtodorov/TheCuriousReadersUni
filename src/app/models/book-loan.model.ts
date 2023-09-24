import { Book } from "./book.model";
import { User } from "./user.model";

export interface BookLoan {
    bookLoanId: string,
    from: Date,
    to: Date,
    timesExtended: number,
    bookItemId: string,
    bookItemBarcode: string,
    loanedTo: User
    book: Book
}