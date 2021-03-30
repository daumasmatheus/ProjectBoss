import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BaseGuard } from 'src/app/guards/base.guard';

@Injectable({providedIn: 'root'})
export class HomeGuard extends BaseGuard implements CanActivate {
    private jwtHelper = new JwtHelperService()

    constructor(protected router: Router) { super(router); }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.localStorageUtils.getUserToken() && !this.jwtHelper.isTokenExpired(this.localStorageUtils.getUserToken())){
            this.navigateToDashboard();
        } else {
            return true;
        }
    }
}