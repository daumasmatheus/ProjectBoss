import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { MessageType } from 'src/app/helpers/message-type.enum';
import { MustMatch } from 'src/app/helpers/must-match.validator';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { User } from '../models/user.model';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private formBuilder: FormBuilder,
              private accountService: AccountService,
              private snackHelper: SnackBarHelper,
              private router: Router ) { }

  registerForm: FormGroup;
  user: User;

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      firstName: new FormControl('', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(255)
      ]),
      lastName: new FormControl('', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(255)
      ]),
      email: new FormControl('', [ 
        Validators.required,
        Validators.pattern("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$") 
      ]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(8)
      ]),
      passwordConfirmation: new FormControl('', [
        Validators.required        
      ])
    }, {
      validators: MustMatch('password', 'passwordConfirmation')
    });
  }

  hasError = (controlName: string, errorName: string) => {
    return this.registerForm.controls[controlName].hasError(errorName);
  }

  register = () => {
    if (this.registerForm.dirty && this.registerForm.valid) {
      this.registerForm.disable();

      this.user = Object.assign({}, this.user, this.registerForm.value);

      this.accountService.registerUser(this.user).subscribe(
        result => { this.proccessResult(result) },
        error => { this.proccessError(error) }
      );
    }
  }

  proccessResult = (response: any) => {
    this.registerForm.reset();
    this.registerForm.enable();

    this.snackHelper.showSnackbar('Usuário registrado com sucesso!', MessageType.OkMessage, 3000); 
    
    setTimeout(() => {
      this.router.navigate(['account/login']);
    }, 2000);
  }

  proccessError = (fail: any) => {
    fail.error.errors.Messages.forEach(errorMessage => {
      this.snackHelper.showSnackbar(errorMessage, MessageType.ErrorMessage, 3000);
    });
    this.registerForm.enable();    
  }
}