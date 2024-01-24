import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AcoountService } from 'src/app/_services/acoount.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  member:Member|undefined;
  user:User |null= null;
  @ViewChild('editForm') editForm:NgForm |undefined
  // @HostListener('window:beforeunload',['$event']) unloadNotification($event:any){
  //   if(this.editForm?.dirty){
  //     $event.returnValue = true
  //   }
  // }

  constructor( private accountService:AcoountService, private memberService:MembersService,
    private toastr:ToastrService){
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:res=>this.user = res
    })
  }
  ngOnInit(): void {
    this.loadMember()
  }

  loadMember(){
    if(!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe({
      next:res=>this.member = res
    })
  }
  updateProfile(){
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next:()=>{
        this.toastr.success("profile Updated successfully");
        this.editForm?.reset(this.member)
      }
    })
    
  }
}
