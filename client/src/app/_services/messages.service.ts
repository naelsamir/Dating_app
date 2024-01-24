import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/Message';

@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  baseUrl = environment.apiUrl

  constructor(private http:HttpClient) { }

  getMessages(pageNumber:number,pageSize:number,container:string){
    let params = getPaginationHeaders(pageNumber,pageSize)
    params = params.append("container",container);
    return getPaginatedResult<Message[]>(this.baseUrl +"message",params,this.http);
  }

  getMessagesThreads(username:string){
    return this.http.get<Message[]>(this.baseUrl+"message/thread/"+username)
  }

  sendMessage(username:string, content:string){
    return this.http.post<Message>(this.baseUrl+"message",{RecipientUsername:username,content})
  }

  deleteMessage(id:number){
    return this.http.delete(this.baseUrl+"message/"+id);
  }
}
