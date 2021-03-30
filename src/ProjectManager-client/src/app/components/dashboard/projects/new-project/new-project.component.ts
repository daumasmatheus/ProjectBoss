import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatStepper } from '@angular/material/stepper';
import { DateCantBeLowerThanToday } from 'src/app/helpers/dateValidators.validator';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { TableComponentComponent } from '../../base-components/table-component/table-component.component';
import { NewProject } from '../../models/new-project.model';
import { Person } from '../../models/person.model';
import { ProjectTask } from '../../models/project-task.model';
import { AddTaskDialogComponent } from '../add-task-dialog/add-task-dialog.component';
import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';

import * as moment from 'moment';
import 'moment/locale/pt-br';
import { NewTaskModel } from '../../models/new-task.model';
import { PersonModel } from '../../models/person.1.model';
import { PersonServices } from '../../services/person.services';
import { ActivatedRoute } from '@angular/router';
import { ProjectServices } from '../../services/project.services';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';

@Component({
  selector: 'app-new-project',
  templateUrl: './new-project.component.html',
  styleUrls: ['./new-project.component.css'],
  providers: [{
    provide: STEPPER_GLOBAL_OPTIONS, useValue: { displayDefaultIndicatorType: false }
  }]
})
export class NewProjectComponent implements OnInit, AfterViewInit {
  localStorageUtils = new LocalStorageUtils();
  moment: any = moment;

  @ViewChild('stepper') private stepper: MatStepper;
  @ViewChild('attendantsTable') private attendantsTable: TableComponentComponent;

  displayedColumns: string[] = ['fullName'];
  displayedColumnNames: string[] = ['Nome'];

  people: PersonModel[] = [];
  projectDataForm: FormGroup;
  projectData: NewProject;
  selectedAttendants: PersonModel[];
  newTask: ProjectTask;
  projectTasks: ProjectTask[] = [];
  disablePrevSteps: boolean = false;  

  constructor(private dialog: MatDialog,
              private activatedRoute: ActivatedRoute,
              private projectServices: ProjectServices,
              private snackHelper: SnackBarHelper) { 
    this.moment.locale('pt-br');    
    
    this.getAttendantsFromRoute();
  }

  ngAfterViewInit(): void { }

  ngOnInit(): void {
    this.projectDataForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.maxLength(255)]),
      startDate: new FormControl('', [Validators.required, DateCantBeLowerThanToday]),
      conclusionDate: new FormControl('', [Validators.required, DateCantBeLowerThanToday]),
      description: new FormControl('', [Validators.required])
    });    
  }

  saveProjectData() {
    this.projectData = Object.assign({}, this.projectData, this.projectDataForm.value);
  }

  getAttendants() {
    this.selectedAttendants = this.attendantsTable.selection.selected;    
    setTimeout(() => {
      this.stepper.next();
    }, 1);
  }

  openNewTaskDialog() {
    this.newTask = new ProjectTask(this.setTaskId())

    let dialogRef = this.dialog.open(AddTaskDialogComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        newTask: this.newTask,
        selectedAttendants: this.selectedAttendants,
        projectConclusionDate: this.projectDataForm.get('conclusionDate').value
      }
    });

    dialogRef.afterClosed().subscribe((task: ProjectTask) => {
      if (task != null) {
        console.log(task);
        this.projectTasks.push(task);        
        this.snackHelper.showSnackbar("Tarefa incluída", MessageType.OkMessage, 2000);
      }
    });    
  }

  removeTask(task: ProjectTask){
    this.projectTasks = this.projectTasks.filter(p => p !== task);
    this.snackHelper.showSnackbar("Tarefa Removida", MessageType.OkMessage, 2000);
  }

  editTask(task: ProjectTask) {
    let editDialogRef = this.dialog.open(AddTaskDialogComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        taskToEdit: task,
        selectedAttendants: this.selectedAttendants,
        projectConclusionDate: this.projectDataForm.get('conclusionDate').value
      }
    });

    editDialogRef.afterClosed().subscribe((task: ProjectTask) => {
      if (task != null) {
        this.projectTasks.forEach((el, i) => {
          if (el.id == task.id) {
            this.projectTasks[i] = task;
          }
        });
        
        this.snackHelper.showSnackbar("Tarefa editada", MessageType.OkMessage, 2000);
      }
    });
  }

  setTaskId() {
    if (this.projectTasks.length > 0) {
      return this.projectTasks.length + 1;
    } else {
      return 1;
    }
  }

  isStepComplete(step: number) {
    switch (step) {
      case 1:
        return this.projectDataForm.valid;
      case 2:
        return (this.selectedAttendants != null && this.selectedAttendants.length > 0);
      case 3:
        return (this.projectTasks != null && this.projectTasks.length > 0);
    }
  }  

  createProject(){
    this.projectData.tasks = this.projectTasks;
    this.projectData.authorId = this.localStorageUtils.getUser().personId;

    if (!this.projectData.attendantIds){
      this.selectedAttendants.forEach(element => {
        if (!this.projectData.attendantIds) {
          this.projectData.attendantIds = [];
        }
  
        this.projectData.attendantIds.push(element.personId);
      });
    }
    
    this.disablePrevSteps = true;
    
    console.log(this.projectData);
    this.projectServices.saveProject(this.projectData).subscribe(
      (resp: any) => {
        setTimeout(() => {
          this.stepper.next();
        }, 1);    
        this.snackHelper.showSnackbar("Novo projeto salvo com sucesso", MessageType.OkMessage, 3000);
      }, error => {
        this.snackHelper.showSnackbar("Erro ao salvar o projeto", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    );
  }

  hasError(controlName: string, errorName: string){
    return this.projectDataForm.controls[controlName].hasError(errorName);
  }

  getAttendantsFromRoute() {
    this.activatedRoute.data.subscribe(
      (data: {attendants: PersonModel[]}) => {
        this.people = data.attendants;
      }
    );
  }

  getPriorityStr(priorityId: number){
    switch (priorityId) {
      case 1:
          return 'Baixa'
      case 2:
          return 'Normal'
      case 3:
          return 'Alta'
      }
  }
}