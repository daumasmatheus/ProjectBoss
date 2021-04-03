import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SocialAuthService, SocialUser } from "angularx-social-login";
import { GoogleLoginProvider } from "angularx-social-login"; 
import { from } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { ExternalUser } from '../models/externalUser.model';
import { User } from '../models/user.model';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-log-in',
  templateUrl: './log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent implements OnInit {

  constructor(private accountService: AccountService,              
              private router: Router,
              private snackHelper: SnackBarHelper,
              private authService: SocialAuthService) { }

  loginForm: FormGroup;
  user: User;
  externalUser: ExternalUser;
  loginIn: boolean = false;  

  ngOnInit() {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$")]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)])
    }); 
  }

  signInWithGoogle() {
    from(this.authService.signIn(GoogleLoginProvider.PROVIDER_ID)).pipe(
      switchMap((socialUser: SocialUser) => {
        this.externalUser = {
          firstName: socialUser.firstName,
          lastName: socialUser.lastName,
          email: socialUser.email,
          provider: socialUser.provider,
          externalUserId: socialUser.id          
        }

        return this.accountService.externalAuth(this.externalUser)
      })
    ).subscribe(
      result => { this.proccessResult(result) },
      error => { this.proccessError(error) }
    );    
  }

  hasError = (controlName: string, errorName: string) => {
    return this.loginForm.controls[controlName].hasError(errorName);
  }

  login(){
    if (this.loginForm.dirty && this.loginForm.valid){
      this.user = Object.assign({}, this.user, this.loginForm.value);

      this.accountService.login(this.user)
          .subscribe(
            response => {this.proccessResult(response)},
            error => {this.proccessError(error)}
      );
    }
  }  

  proccessResult = (response: any) => {
    this.loginForm.disable();
    this.loginIn = true;
    
    this.accountService.localStorage.saveUserData(response);

    this.snackHelper.showSnackbar('Logado com sucesso!', MessageType.OkMessage, 3000);
    
    setTimeout(() => {
      this.router.navigate(['/dashboard'])
    }, 3000);
  }

  proccessError = (fail: any) => {
    fail.error.errors.Messages.forEach(errorMessage => {
      this.snackHelper.showSnackbar(errorMessage, MessageType.ErrorMessage);
    });    
  }
}