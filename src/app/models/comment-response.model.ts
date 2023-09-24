import { User } from "./user.model";

export class CommentResponse {
    constructor(
        public commentId: string,
        public content: string,
        public user: User,
    ) { };
}