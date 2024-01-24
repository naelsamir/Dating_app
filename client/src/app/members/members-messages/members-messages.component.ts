import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/Message';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  selector: 'app-members-messages',
  standalone:true,
  templateUrl: './members-messages.component.html',
  styleUrls: ['./members-messages.component.css'],
  imports:[CommonModule, TimeagoModule,FormsModule]
})
export class MembersMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?:NgForm
  @Input() username?:string;
  messages:Message[]=[];
  messageContent='';

  constructor(private messageService:MessagesService){}

  ngOnInit(): void {
  
  }

 

  sendMessage(){
    if(!this.username) return
      this.messageService.sendMessage(this.username,this.messageContent).subscribe({
        next: message=> {this.messages?.push(message);
        this.messageForm?.reset()}
      })
    
  }

}
function viewChild(arg0: string): (target: MembersMessagesComponent, propertyKey: "messageForm") => void {
  throw new Error('Function not implemented.');
}

