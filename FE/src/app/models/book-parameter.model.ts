export class BookParameters {
    public title?: string;
    public author?: string;
    public descriptionKeyword?: string;
    public publisher?: string;
    public genre?: string;
}

export class PagingParameters {
    constructor(
        public pageNumber: number,
        public pageSize: number,
    ) {};
}