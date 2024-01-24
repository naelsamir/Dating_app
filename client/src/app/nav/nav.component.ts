import { Component, OnInit } from '@angular/core';
import { AcoountService } from '../_services/acoount.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

   model:any={};
   user:any;
  
  constructor(public accountService:AcoountService, private router:Router,
    private toastr: ToastrService, private memberService:MembersService){}
  ngOnInit(): void {
    
  }

  

  login(){
    this.accountService.login(this.model).subscribe({
      next:()=> {this.router.navigateByUrl('/members');
    this.accountService.currentUser$.subscribe(res=>console.log(res))}
    });
    // this.memberService.resetUserParams();

  }

  logOut(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
