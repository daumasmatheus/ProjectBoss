import { ThrowStmt } from '@angular/compiler';
import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DateCantBeLowerThanToday, TaskConclusionDateCantBeHigherThanProjectConclusionDate } from 'src/app/helpers/dateValidators.validator';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { SelectAttendantListComponent } from '../../base-components/select-attendant-list/select-attendant-list.component';
import { CommentModel } from '../../models/comment.model';
import { PersonModel } from '../../models/person.1.model';
import { Person } from '../../models/person.model';
import { ProjectTask } from '../../models/project-task.model';
import { ProjectDataModel } from '../../models/project.model';
import { CommentServices } from '../../services/comment.services';

@Component({
  selector: 'app-add-task-dialog',
  templateUrl: './add-task-dialog.component.html',
  styleUrls: ['./add-task-dialog.component.css']
})
export class AddTaskDialogComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  attendants: PersonModel[];
  
  taskForm: FormGroup;
  taskData: ProjectTask;
  selectedAttendant: PersonModel;
  projectConclusionDate: Date;
  project: ProjectDataModel;

  fromProject: boolean = false;
  editing: boolean = false;

  commentForm: FormGroup;
  commentStr: string;
  comment: CommentModel;
  comments: CommentModel[] = [];
  isSubmiting: boolean = false;
  loadingComments: boolean = false; 

  isProjectManagerOrAdmin: boolean = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              public dialogRef: MatDialogRef<AddTaskDialogComponent>,
              private dialog: MatDialog,
              private commentServices: CommentServices,
              private snackHelper: SnackBarHelper) { 
    this.isProjectManagerOrAdmin = this.localStorageUtils.checkUserClaim("Administrator") || this.localStorageUtils.checkUserClaim("ProjectManager")              
  }  

  ngOnInit(): void {
    this.projectConclusionDate = this.data?.projectConclusionDate;

    this.taskForm = new FormGroup({
      title: new FormControl('', [Validators.required]),
      priorityId: new FormControl('', [Validators.required]),
      conclusionDate: new FormControl('', [Validators.required, 
                                           DateCantBeLowerThanToday, 
                                           TaskConclusionDateCantBeHigherThanProjectConclusionDate(this.projectConclusionDate)]),
      description: new FormControl('', [Validators.required]),
      attendant: new FormControl('', [Validators.required])
    });

    this.commentForm = new FormGroup({
      content: new FormControl('')
    })

    // debugger;
    if (this.data != null && this.data.newTask){
      this.taskData = new ProjectTask(this.data.newTask.id);
      this.attendants = this.data.selectedAttendants;      
    }
    if (this.data != null && this.data.taskToEdit){
      this.taskForm.patchValue(this.data.taskToEdit);
      this.taskForm.get('attendant').setValue(`${this.data.taskToEdit.attendant.personCode} - ${this.data.taskToEdit.attendant.fullName}`);

      this.attendants = this.data.selectedAttendants;
    } 
    if (this.data && this.data.fromProject){
      this.attendants = this.data.attendants;
      this.project = this.data.project;   
      this.fromProject = this.data.fromProject; 
    }
    if (this.data && this.data.editTask){    
      this.fromProject = this.data.fromProject;  
      this.taskData = this.data.task;
      this.attendants = this.data.attendants;  
      
      this.editing = true;

      this.taskForm.patchValue(this.taskData);
      this.taskForm.get('attendant').setValue(`${this.taskData.attendant.personCode} - ${this.taskData.attendant.fullName}`);

      this.taskForm.disable();            
    }

    this.getComments();
  }

  saveTaskData() {    
    this.taskData = Object.assign({}, this.taskData, this.taskForm.value);

    this.taskData.attendant = this.selectedAttendant;
    this.taskData.statusId = 1;
    this.taskData.authorId = this.localStorageUtils.getUser().personId;

    if (this.fromProject && !this.taskData.projectId){
      this.taskData.projectId = this.project.projectId;
    }

    if (this.taskData.id == null && !this.fromProject)
      this.taskData.id = this.data.taskToEdit.id;    

    this.dialogRef.close(this.taskData);
  }  

  closeDialog() {
    this.dialogRef.close();
  }
  
  compareSelected(option, value) {
    return option.id == value.id;
  }

  compareSelectedVal(opt, val) {
    return opt == val;
  }

  hasError(controlName: string, errorName: string) {
    return this.taskForm.controls[controlName].hasError(errorName);
  }

  enableEdit() {
    this.taskForm.enable();
  }

  submitComment(){
    this.isSubmiting = true;

    this.comment = Object.assign({}, this.comment, this.commentForm.value);

    this.comment.personId = this.localStorageUtils.getUser().personId;
    this.comment.taskId = this.taskData.taskId;

    this.commentServices.submitComment(this.comment).subscribe(
      (resp: CommentModel) => {
        if (resp){
          this.getComments();
          this.isSubmiting = false;
          this.snackHelper.showSnackbar("Comentário criado com sucesso", MessageType.OkMessage, 3000);

          this.commentForm.reset();
        }
      }, error => {
        this.snackHelper.showSnackbar("Falha ao submeter o comentário", MessageType.ErrorMessage, 3000);
        console.error(error);
        this.isSubmiting = false;
      }
    )
  }

  getComments(){
    if (this.taskData && this.taskData.taskId){
      this.loadingComments = true;
    
      this.commentServices.getComments(this.taskData.taskId).subscribe(
        (comments: CommentModel[]) => {
          this.comments = comments;
          this.loadingComments = false;
        }, error => console.error(error)
      );
    }
  }

  openAttendantSelectDialog(){
    let dialogRef = this.dialog.open(SelectAttendantListComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        attendants: this.attendants
      }
    });

    dialogRef.afterClosed().subscribe(
      (attendant: PersonModel) => {
        if (attendant) {
          this.selectedAttendant = attendant;
          this.taskForm.get('attendant').setValue(`${attendant.personCode} - ${attendant.fullName}`);          
        }
      }, error => {
        console.error(error);
      }
    )
  }
}