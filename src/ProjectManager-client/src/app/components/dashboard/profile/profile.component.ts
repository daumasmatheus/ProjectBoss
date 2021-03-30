import { formatCurrency } from '@angular/common';
import { AfterViewInit, Component, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { PersonModel } from '../models/person.1.model';
import { PersonServices } from '../services/person.services';
import { combineLatest } from "rxjs";
import { map } from 'rxjs/operators';
import { validateBasis } from '@angular/flex-layout';
import { Person } from '../models/person.model';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { MustMatch } from 'src/app/helpers/must-match.validator';
import { AccountService } from '../../account/services/account.service';
import { ChangePasswordModel } from '../models/changePassword.model';
import { MatTabChangeEvent } from '@angular/material/tabs';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, AfterViewInit {
  @ViewChild('profileTabs') profileTabs;

  selectedTabIndex: number;

  localStorageUtils = new LocalStorageUtils();

  personData: PersonModel;
  changePasswordModel: ChangePasswordModel;

  personalDataForm: FormGroup;
  changePwdForm: FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              public dialogRef: MatDialogRef<ProfileComponent>,
              private personService: PersonServices,
              private accountService: AccountService,
              private snackHelper: SnackBarHelper, 
              private formBuilder: FormBuilder) { }
  
  
  ngAfterViewInit(): void {
    this.selectedTabIndex = this.profileTabs.selectedIndex;
  }

  ngOnInit(): void {  
    this.initPersonDataForm();    
    this.initChangePwdForm();

    if (this.data != null) {
      this.personData = this.data.personData;
      this.personalDataForm.patchValue(this.personData);
    }   
  }

  closeDialog() {
    this.dialogRef.close();
  }  

  initPersonDataForm(){
    this.personalDataForm = new FormGroup({
      firstName: new FormControl('', [Validators.maxLength(255)]),
      lastName: new FormControl('', [Validators.maxLength(255)]),
      role: new FormControl('', [Validators.maxLength(255)]),
      company: new FormControl('', [Validators.maxLength(255)]),
      country: new FormControl('', [Validators.maxLength(255)])
    })
  }

  initChangePwdForm(){
    this.changePwdForm = this.formBuilder.group({
      currentPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(8)]),
      newPasswordConfirm: new FormControl('', [Validators.required])
    }, { validators: MustMatch('newPassword', 'newPasswordConfirm') });
  }

  saveChanges() {
    if (this.personalDataForm.valid){
      this.personData = Object.assign({}, this.personData, this.personalDataForm.value);
    
      this.personService.saveChanges(this.personData).subscribe(
        (response: boolean) => {
          if (response) {     
            this.snackHelper.showSnackbar("Dados salvos com sucesso", MessageType.OkMessage, 3000);       
            this.dialogRef.close(true);
          }
        }, error => {  
          this.snackHelper.showSnackbar("Erro ao salvar os dados", MessageType.ErrorMessage, 3000); 
          this.dialogRef.close(false);        
          console.error(error);
        }
      ); 
    }

    if (this.changePwdForm.valid){
      this.changePassword();
    }    
  }

  hasErrorPersonalForm = (controlName: string, errorName: string) => {
    return this.personalDataForm.controls[controlName].hasError(errorName);
  }

  hasErrorChangePwd = (controlName: string, errorName: string) => {
    return this.changePwdForm.controls[controlName].hasError(errorName);
  }

  changePassword(){
    this.changePasswordModel = Object.assign({}, this.changePasswordModel, this.changePwdForm.value);

    this.changePasswordModel.userId = this.localStorageUtils.getUser().id;
    this.changePasswordModel.email = this.localStorageUtils.getUser().email;
    
    this.accountService.changePassword(this.changePasswordModel).subscribe(
      (resp: any) => {        
        if (resp.succeeded){
          this.snackHelper.showSnackbar("Senha alterada com sucesso", MessageType.OkMessage, 3000);
        } else {
          console.log(resp);
        }   
      }, error => {
        console.error(error);
        this.snackHelper.showSnackbar("Falha ao alterar a senha", MessageType.ErrorMessage, 3000);
      }
    )
  }

  tabChanged(tabChangedEvent: MatTabChangeEvent){
    this.selectedTabIndex = this.profileTabs.selectedIndex;
  }
}