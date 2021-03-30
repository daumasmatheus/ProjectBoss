import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { BaseGuard } from 'src/app/guards/base.guard';

@Injectable({providedIn: 'root'})
export class ManageUsersGuard extends BaseGuard implements CanActivate {
    constructor(protected router: Router) { super(router); }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (!this.localStorageUtils.checkUserClaim("Administrator")){
            this.navigateToDashboard();
        } else {
            return true;
        }
    }
}