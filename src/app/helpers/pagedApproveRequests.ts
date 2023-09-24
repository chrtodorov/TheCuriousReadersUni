import { User } from "src/app/models/user";

export interface PagedApproveRequests{
    pageSize:number;
    hasNext:boolean;
    hasPrevious:boolean;
    pageIndex:number;
    itemsperPage:number;
    totalPages:number;
    data:User[]; 

}