import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { BaseGuard } from 'src/app/guards/base.guard';

@Injectable({providedIn: 'root'})
export class NewProjectGuard extends BaseGuard implements CanActivate {
    constructor(protected router: Router) { super(router); }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        console.log(this.localStorageUtils.getUser())
        if (this.localStorageUtils.checkUserClaim("Administrator") || this.localStorageUtils.checkUserClaim("Project Manager")){
            return true;
        } else {
            this.navigateToDashboard();
        }
    }
}