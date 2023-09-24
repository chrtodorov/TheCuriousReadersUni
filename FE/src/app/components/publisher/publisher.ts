import { FilterParameter } from './publisher.service'

export class Publisher {
    publisherId: string = '';
    name: string = '';
}

export interface PublisherResponse {
    currentPage: number,
    totalPages: number,
    pageSize: number,
    totalCount: number,
    hasPrevious: boolean,
    hasNext: boolean,
    data: Publisher[]
}

export class PublisherParameters {
    public name?: string;
}

export class PublisherPagingParameters {
    constructor(
        public pageNumber: number,
        public pageSize: number,
    ) {};
}

export class Filters {
    public filter: string;
    public filterParameters: FilterParameter[];
}