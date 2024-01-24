import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AcoountService } from './acoount.service';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl= environment.apiUrl;
  members:Member[]=[];
  memberCache= new Map();
  userParams:UserParams|undefined;
  user?:User|null

  constructor(private http: HttpClient, private accountService: AcoountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user){
          this.userParams = new UserParams(user);
          this.user = user;
        } 
      }
    })
  }

  getUserParams(){
    return this.userParams
  }
  setUserParams(params:UserParams){
    this.userParams= params
  }
  resetUserParams(){
    if(this.user){
      this.userParams= new UserParams(this.user);
      return this.userParams
    }
    return;
  }

  getMembers(userParams:UserParams){
    const response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response) return of(response);

   let params = getPaginationHeaders(userParams.PageNumber,userParams.pageSize)
   params = params.append("minAge",userParams.minAge.toString());
   params= params.append('maxAge',userParams.maxAge.toString());
   params= params.append('gender',userParams.gender);
   params= params.append('orderBy',userParams.orderBy);


    return getPaginatedResult<Member[]>(this.baseUrl+'user', params,this.http).pipe(
      map(response=>{
        this.memberCache.set(Object.values(userParams).join('-'),response);
        return response
    }))
  }



  

  getMember(username:string){
    const member =[...this.memberCache.values()]
    .reduce((arr,element)=>arr.concat(element.result),[])
    .find((member:Member)=>member.userName===username);
    if(member) return of(member)
    return this.http.get<Member>(this.baseUrl+"user/" + username);
  }

  updateMember(member :Member){
    return this.http.put(this.baseUrl+"user",member).pipe(
      map(()=>{
        const index = this.members.indexOf(member);
        this.members[index]={...this.members[index], ...member}
      })
    )
  }

  deletePhoto(photoId:number){
   return this.http.delete(this.baseUrl+"user/delete-photo/" +photoId)
  }

  AddLike(username:string){
    return this.http.post(this.baseUrl+"likes/"+username,{})
  }

  GetLikes(predicate:string,pageNumber:number,pageSize:number){
    let params = getPaginationHeaders(pageNumber,pageSize);
    params = params.append("predicate",predicate)
    return getPaginatedResult<Member[]>(this.baseUrl +'likes',params,this.http);
  }
}
