import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';

import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { PersonModel } from './models/person.1.model';
import { ProfileComponent } from './profile/profile.component';
import { PersonServices } from './services/person.services';

@Component({
    selector: 'app-dashbord',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit { 
    private localStorageUtils = new LocalStorageUtils();

    personData: PersonModel;
    userName: string;
    isAdmin: boolean = false;

    constructor (private snackHelper: SnackBarHelper, private router: Router, private dialog: MatDialog, 
                 private activatedRoute: ActivatedRoute, private personService: PersonServices
                 ) { 
        this.getDataFromResolver();

        this.isAdmin = this.localStorageUtils.checkUserClaim("Administrator");
    }

    ngOnInit(): void {
        this.userName = `${this.personData.firstName} ${this.personData.lastName}`;
    }

    logout() {
        this.localStorageUtils.clearLocalUserData();

        this.snackHelper.showSnackbar("Efetuando logout", MessageType.OkMessage, 3000);
        setTimeout(() => {
            this.router.navigate(['/account/login']);
        }, 3000);
    }

    openProfileDialog() {
        let dialogRef = this.dialog.open(ProfileComponent, {
            hasBackdrop: true,        
            panelClass: 'full-width-dialog',
            maxHeight: '490px',
            data: { 
                personData: this.personData,                
            }
        });

        dialogRef.afterClosed().subscribe(
            (resp: boolean) => {
                if (resp){
                    this.getDataFromService();
                }
            }
        )
    }

    getDataFromResolver(){
        this.activatedRoute.data.subscribe(
            (data: {personData: PersonModel}) => {
                this.personData = data.personData;
            }
        );
    }

    getDataFromService(){
        this.personService.getPersonData(this.personData.personId).subscribe(
            (resp: PersonModel) => {
                if (resp != null) {
                    this.personData = resp;
                }
            }, error => {
                console.error(error);
            }
        );
    }    
}