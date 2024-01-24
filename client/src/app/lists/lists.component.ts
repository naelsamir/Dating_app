import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  members:Member[]|undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 2;
  pagination:Pagination|undefined
  constructor(private memberService:MembersService){}
  ngOnInit(): void {
    this.loadLikes()
  }
  loadLikes(){
    this.memberService.GetLikes(this.predicate,this.pageNumber,this.pageSize).subscribe({
      next:res=>{
        this.members = res.result;
        this.pagination = res.pagination
      }
    })
  }

  pageChanged(event:any){
    if( this.pageNumber!=event.page){
      this.pageNumber = event.page;
      this.loadLikes()
    }
  }

}
