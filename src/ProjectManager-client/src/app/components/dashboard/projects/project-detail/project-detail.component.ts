import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DateCantBeLowerThanToday } from 'src/app/helpers/dateValidators.validator';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { ProjectDataModel } from '../../models/project.model';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css']
})
export class ProjectDetailComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  project: ProjectDataModel;
  projectForm: FormGroup;

  editing: boolean = false;
  
  isProjectManagerOrAdmin: boolean = this.localStorageUtils.checkUserClaim("Project Manager") || this.localStorageUtils.checkUserClaim("Administrator");

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              public dialogRef: MatDialogRef<ProjectDetailComponent>) { }

  ngOnInit(): void {   
    if(this.data && this.data.project){
      this.project = this.data.project;
    }    

    this.projectForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.maxLength(255)]),
      startDate: new FormControl('', []),
      conclusionDate: new FormControl('', [Validators.required, DateCantBeLowerThanToday]),
      description: new FormControl('', [Validators.required])
    }); 

    this.projectForm.patchValue(this.project);      
    this.projectForm.disable();
  }

  hasError(controlName: string, errorName: string){ 
    return this.projectForm.controls[controlName].hasError(errorName);   
  }  

  enableEdit(){
    this.projectForm.enable();
    this.editing = !this.editing;
  }

  save(){
    this.project = Object.assign({}, this.project, this.projectForm.value);

    this.dialogRef.close(this.project);
  }
}