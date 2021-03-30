import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { JwtHelperService } from "@auth0/angular-jwt";
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { BaseGuard } from 'src/app/guards/base.guard';

@Injectable()
export class JwtValidationGuard extends BaseGuard implements CanActivate {
    
    constructor (protected router: Router) { super(router); }

    private jwtHelper = new JwtHelperService();

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this.localStorageUtils.getUserToken()){
            this.navigateToLogin();
        } else {
            var userToken = this.localStorageUtils.getUserToken();
            if (this.jwtHelper.isTokenExpired(userToken)){
                console.log('token expired');
                this.navigateToLogin();
            } else {
                return true;
            }
        }
    }
}