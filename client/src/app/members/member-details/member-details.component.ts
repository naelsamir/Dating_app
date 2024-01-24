import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { MembersMessagesComponent } from '../members-messages/members-messages.component';
import { Message } from 'src/app/_models/Message';
import { User } from 'src/app/_models/user';
import { take } from 'rxjs';
import { AcoountService } from 'src/app/_services/acoount.service';
import { MessagesService } from 'src/app/_services/messages.service';

@Component({
  standalone:true,
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
  imports:[CommonModule,TabsModule,GalleryModule,TimeagoModule,MembersMessagesComponent]
})
export class MemberDetailsComponent implements OnInit{
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  

  constructor( private route: ActivatedRoute,
    private messageService: MessagesService, private accountService: AcoountService) {
    
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })

    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages()
  }

  

  loadMessages() {
    if (this.member)
      this.messageService.getMessagesThreads(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages') {
    this.loadMessages()
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member?.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
