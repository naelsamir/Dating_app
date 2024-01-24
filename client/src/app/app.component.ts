import { Component, OnInit } from '@angular/core';
import { AcoountService } from './_services/acoount.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  // url ="https://localhost:5001/api/user";
  users:any;
  constructor( private accountService:AcoountService){ }

  ngOnInit(): void {
    // this.getUsers();
    this.setCurrentUser();
  }

  // getUsers(){
  //   this.http.get(this.url).subscribe({
  //     next:res=> this.users=res,
  //     error:error=>console.log(error),
  //   })
  // }

  setCurrentUser(){
    const userString = localStorage.getItem('user')
    if(!userString) return;
    const user = JSON.parse(userString);
    this.accountService.setCurrentUser(user)
  }
}
