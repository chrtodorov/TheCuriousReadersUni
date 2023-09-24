import { Book } from "./book"

export interface PagedBookResponse { 
    pageSize:number;
    hasNext:boolean;
    hasPrevious:boolean;
    pageIndex:number;
    itemsperPage:number;
    totalPages:number;
    data:Book[]; 
}