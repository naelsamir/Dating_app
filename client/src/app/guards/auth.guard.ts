import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AcoountService } from '../_services/acoount.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AcoountService);
  const toastr = inject(ToastrService);
   return accountService.currentUser$.pipe(map(user=>{
    if(user) return true
    else{
      toastr.error('you should login first');
      return false
    }
   }))
};
