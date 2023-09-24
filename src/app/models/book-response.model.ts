import { Book } from "./book.model";

export interface BookResponse {
    currentPage: number,
    totalPages: number,
    pageSize: number,
    totalCount: number,
    hasPrevious: boolean,
    hasNext: boolean,
    data: Book[]
}