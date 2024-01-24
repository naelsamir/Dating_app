import { Component, OnInit } from '@angular/core';
import { MessagesService } from '../_services/messages.service';
import { Message } from '../_models/Message';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
messages?:Message[]
pageNumber= 1;
pageSize= 5;
pagination?:Pagination;
Container="Unread";
messageContent=''
username:string|undefined
loading = false;

  constructor(private messageService:MessagesService){}
  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.pageNumber,this.pageSize,this.Container).subscribe({
      next:response=>{
        this.messages = response.result;
        this.pagination = response.pagination;
        this.loading= false
      }
    })
  }
  deleteMesaage(id:number){
    this.messageService.deleteMessage(id).subscribe({
      next:()=> this.messages?.splice(this.messages.findIndex(m=>m.id ===id),1)
    })
  }

  

  pagechanged(event:any){
    if(this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.loadMessages()
    }
  }

}
