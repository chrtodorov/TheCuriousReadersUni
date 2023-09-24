export class PagedRequest<T>{
    constructor(
        public currentPage: number,
        public totalPages: number,
        public pageSize: number,
        public totalCount: number,
        public hasPrevious: boolean,
        public hasNext: boolean,
        public data: T[],
    ) { };
}