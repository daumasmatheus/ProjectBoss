import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { UserDataModel, UserRole } from '../../models/userData.model';
import { UsersServices } from '../../services/users.services';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css']
})
export class UserDetailsComponent implements OnInit {
  selectedUser: UserDataModel;
  userRoles: UserRole[] = [
    { name: 'Administrator', id: 'ED110E5C-F7B3-40FA-B3D3-736762213CB2' },
    { name: 'ProjectManager', id: '9c764096-5e10-4072-869c-e319f6c3dff0' },
    { name: 'CommonUser', id: '58d477dd-f134-40ac-9fe9-d7cd72775d0c' }
  ];

  userDataForm: FormGroup;
  personDataForm: FormGroup;

  proccessPwdReset: boolean = false;

  constructor(private dialogRef: MatDialogRef<UserDetailsComponent>,
              private dialog: MatDialog,
              private usersService: UsersServices,
              private snackHelper: SnackBarHelper,
              @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    this.userDataForm = new FormGroup({
      createdDate: new FormControl(''),
      userName: new FormControl(''),
      email: new FormControl(''),
      provider: new FormControl(''),
      role: new FormControl('')
    });

    this.personDataForm = new FormGroup({
      personCode: new FormControl(''),
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      country: new FormControl(''),
      company: new FormControl(''),
      role: new FormControl('')
    })

    if (this.data && this.data.user){      
      this.selectedUser = this.data.user;

      this.userDataForm.patchValue(this.selectedUser);
      this.personDataForm.patchValue(this.selectedUser.person);
    }
  }

  compareSelected(opt1: any, opt2: any) {    
    return opt1 === opt2.id;
  }

  closeDialog() {
    this.dialogRef.close();
  }

  saveChanges(){
    this.selectedUser = Object.assign({}, this.selectedUser, this.userDataForm.value);
    this.selectedUser.role = this.userRoles.find(el => el.id === this.userDataForm.get('role').value);

    this.selectedUser.person = Object.assign({}, this.selectedUser.person, this.personDataForm.value);    

    this.dialogRef.close(this.selectedUser);
  }

  resetUserPassword(){
    this.proccessPwdReset = true;
    this.usersService.resetUserPassword(JSON.stringify(this.selectedUser.id)).subscribe(
      (resp: boolean) => {
        if (resp) {
          this.proccessPwdReset = false;
          this.snackHelper.showSnackbar('A senha do usuário foi resetada para 123456789', MessageType.OkMessage, 3000);
        }
      }, error => {
        this.proccessPwdReset = false;
        console.error(error);
        this.snackHelper.showSnackbar('Falha ao resetar a senha do usuário', MessageType.ErrorMessage, 3000);
      }
    )
  }
}