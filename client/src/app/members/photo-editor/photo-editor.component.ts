import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AcoountService } from 'src/app/_services/acoount.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member:Member|undefined
  uploader:FileUploader |undefined
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user:User |undefined
  constructor( private accountservice :AcoountService ,private memberService:MembersService){
    this.accountservice.currentUser$.pipe(take(1)).subscribe({
      next:user => {
        if(user) this.user = user
      } 
    })
  }
  ngOnInit(): void {
    this.initializeUploader()
  }

  fileOverBase(e:any){
    this.hasBaseDropZoneOver = e
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url:this.baseUrl + "user/add-photo",
      authToken:"bearer "+this.user?.token,
      isHTML5 :true,
      allowedFileType:['image'],
      removeAfterUpload:true,
      autoUpload:false,
      maxFileSize:10*.1024*1024
    });
    this.uploader.onAfterAddingFile= (file)=>{
      file.withCredentials = false;
    }
    this.uploader.onSuccessItem= (item,response,status,header)=>{
      if(response){
        const photo = JSON.parse(response)
        this.member?.photos.push(photo)
      }
    }
  }

  deletePhoto(photoId:number){
    this.memberService.deletePhoto(photoId).subscribe({
      next:()=>{
        if(this.member){
          this.member.photos = this.member.photos.filter(x=>x.id !== photoId)
        }
      }
    })
  }

}
