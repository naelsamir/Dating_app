import { User } from "./user";

export class UserParams{
    gender:string;
    minAge = 0;
    maxAge = 99;
    PageNumber = 1;
    pageSize= 5;
    orderBy="lastActive";

    constructor(user:User){
        this.gender = user.gender==="female"? "male":"female";
    }
}