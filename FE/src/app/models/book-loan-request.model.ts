export class BookLoanRequest {
    constructor(
        public from: Date,
        public to: Date,
        public loanedCopy: string,
        public loanedTo: string
    ) { };
}