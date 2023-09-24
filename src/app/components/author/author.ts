import { FilterParameter } from './author.service'

export class Author {
    authorId: string = '';
    name: string = '';
    bio: string = '';
}

export interface AuthorResponse {
    currentPage: number,
    totalPages: number,
    pageSize: number,
    totalCount: number,
    hasPrevious: boolean,
    hasNext: boolean,
    data: Author[]
}

export class AuthorParameters {
    public name?: string;
}

export class AuthorPagingParameters {
    constructor(
        public pageNumber: number,
        public pageSize: number,
    ) {};
}

export class Filters {
    public filter: string;
    public filterParameters: FilterParameter[];
}