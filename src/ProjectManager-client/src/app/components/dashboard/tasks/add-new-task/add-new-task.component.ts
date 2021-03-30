import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { Component, Inject, OnInit } from '@angular/core';
import { inject } from '@angular/core/testing';
import { Validators } from '@angular/forms';
import { FormControl } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DateCantBeLowerThanToday, TaskConclusionDateCantBeHigherThanProjectConclusionDate } from 'src/app/helpers/dateValidators.validator';
import { DownloadFileDialogComponent } from 'src/app/helpers/download-file-dialog/download-file-dialog.component';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { PopConfirmationComponent } from 'src/app/helpers/pop-confirmation/pop-confirmation.component';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { SelectAttendantListComponent } from '../../base-components/select-attendant-list/select-attendant-list.component';
import { CommentModel } from '../../models/comment.model';
import { NewTaskModel } from '../../models/new-task.model';
import { PersonModel } from '../../models/person.1.model';
import { TaskModel } from '../../models/task.model';
import { CommentServices } from '../../services/comment.services';
import { PersonServices } from '../../services/person.services';
import { TaskServices } from '../../services/task.services';

@Component({
  selector: 'app-add-new-task',
  templateUrl: './add-new-task.component.html',
  styleUrls: ['./add-new-task.component.css']
})
export class AddNewTaskComponent implements OnInit {
  private localStorageUtils = new LocalStorageUtils();

  newTaskForm: FormGroup;

  newTask: NewTaskModel;
  peopleToTask: PersonModel[];
  selectedAttendant: PersonModel = new PersonModel;

  isProjectManagerOrAdmin: boolean = false;

  editTask: TaskModel;
  editingTask: boolean = false;  

  commentForm: FormGroup;
  comment: CommentModel;
  commentStr: string;
  comments: CommentModel[] = [];
  isSubmiting: boolean = false;
  loadingComments: boolean = false;

  constructor(private dialogRef: MatDialogRef<AddNewTaskComponent>,
              private dialog: MatDialog,
              private taskService: TaskServices,
              private commentServices: CommentServices,
              private snackHelper: SnackBarHelper,
             @Inject(MAT_DIALOG_DATA) public data: any) { 
  }

  ngOnInit(): void {
    this.newTaskForm = new FormGroup({
      title: new FormControl('', [Validators.required]),
      priorityId: new FormControl('', [Validators.required]),
      conclusionDate: new FormControl('', [Validators.required, DateCantBeLowerThanToday]),
      description: new FormControl('', [Validators.required]),
      attendant: new FormControl('', [Validators.required])     
    });

    this.commentForm = new FormGroup({
      content: new FormControl('', [Validators.required])
    });

    if (this.data != null) {
      if (this.data.editTask != null) {        
        this.editingTask = true;
        this.editTask = this.data.editTask;

        this.newTaskForm.patchValue(this.data.editTask);
        this.newTaskForm.get('attendant').setValue(`${this.data.editTask.attendant.personCode} - ${this.data.editTask.attendant.fullName}`);

        this.getComments();
      }

      if (this.data.attendants != null) {
        this.peopleToTask = this.data.attendants;
        this.isProjectManagerOrAdmin = true;
      } else {
        this.newTaskForm.removeControl('attendant');
      }
    }
  }

  hasError(controlName: string, errorName: string) {
    return this.newTaskForm.controls[controlName].hasError(errorName);
  }

  closeDialog() {
    this.dialogRef.close();
  }

  save(){
    if (this.editingTask == false) {
      this.newTask = {
        title: this.newTaskForm.controls["title"].value,
        statusId: 1,
        priorityId: +this.newTaskForm.controls["priorityId"].value,
        conclusionDate: this.newTaskForm.controls["conclusionDate"].value,
        description: this.newTaskForm.controls["description"].value,
        authorId: this.localStorageUtils.getUser().personId,
        attendantId: this.setAttendant(),
        projectId: null 
      }
      
      this.taskService.newTask(this.newTask).subscribe((resp: NewTaskModel) => {
        if (resp != null) {
          this.snackHelper.showSnackbar("Tarefa criada com sucesso", MessageType.OkMessage, 3000);
          this.dialogRef.close(true);
        }
      }, error => {
        this.snackHelper.showSnackbar("Falha ao criar a nova tarefa", MessageType.ErrorMessage, 3000);
        console.error(error);
        this.dialogRef.close(false);
      });
    } else {
      this.editTask = Object.assign({}, this.editTask, this.newTaskForm.value);

      if (this.peopleToTask){
        this.editTask.attendant = this.peopleToTask.find(x => x.personId == this.selectedAttendant[0]);
      }

      this.taskService.editTask(this.editTask).subscribe(
        (resp: boolean) => {
          this.snackHelper.showSnackbar("Tarefa editada com sucesso", MessageType.OkMessage, 3000);
          this.dialogRef.close(true);
        }, (error: any) => {
          this.snackHelper.showSnackbar("Falha ao editar a tarefa", MessageType.ErrorMessage, 3000);
          console.error(error);
          this.dialogRef.close(false);
        }
      )
    }
  } 
  
  setAttendant() {
    if (!this.peopleToTask){
      return this.localStorageUtils.getUser().personId;
    } else {
      return this.selectedAttendant.personId;
    }
  }  

  concludeTask(){
    this.taskService.setTaskComplete(JSON.stringify(this.data.editTask.taskId)).subscribe((resp: boolean) => {
      if (resp){
        this.snackHelper.showSnackbar("Tarefa marcada como concluída", MessageType.OkMessage, 3000);
        this.dialogRef.close(true);        
      }
    }, error => { console.error(error) });
  }

  compareSelected(option, value) {
    return option == value;
  }

  compareFn(opt: PersonModel, val: PersonModel) {
    return opt.personId === val.personId;
  }

  toggleTaskStatus(statusId: number){
    var params = {
      taskId: this.editTask.taskId,
      statusId: statusId
    };

    this.taskService.toggleTaskStatus(params).subscribe(
      (resp: boolean) => {
       if (resp){
        this.snackHelper.showSnackbar("Status da tarefa atualizado", MessageType.OkMessage, 3000);
        this.dialogRef.close(true);
       } 
      }, (error: any) => {
        this.snackHelper.showSnackbar("Falha ao atualizar o status da tarefa", MessageType.ErrorMessage, 3000);
        this.dialogRef.close(false);
        console.error(error);
      });
  }

  removeTask(){
    let dialogRef = this.dialog.open(PopConfirmationComponent, {
      hasBackdrop: true,
      data: {
        title: `Remover Tarefa ${this.editTask.title}`,
        message: 'Deseja remover a tarefa? A operação não pode ser desfeita.'
      }
    })

    dialogRef.afterClosed().subscribe((resp: boolean) => {
      if (resp) {
        if (this.editTask.author.userId == this.localStorageUtils.getUser().id || this.localStorageUtils.checkUserClaim("ADMINISTRATOR")) {
          this.taskService.removeTask(JSON.stringify(this.editTask.taskId)).subscribe(
            (resp: boolean) => {
              if (resp) {
                this.snackHelper.showSnackbar("Tarefa removida", MessageType.OkMessage, 3000);
                this.dialogRef.close(true);
              }
            }, (error: any) => {
              this.snackHelper.showSnackbar("Falha ao remover a tarefa", MessageType.ErrorMessage, 3000);
              console.error(error);
              this.dialogRef.close(false);
            }
          )
        } 
      }
    });
  }

  isMyTask(){
    return this.editTask.author.userId == this.localStorageUtils.getUser().id;
  }

  submitComment(){
    this.isSubmiting = true;

    this.comment = Object.assign({}, this.comment, this.commentForm.value);

    this.comment.personId = this.localStorageUtils.getUser().personId;
    this.comment.taskId = this.editTask.taskId;

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
    if (this.editTask && this.editTask.taskId){
      this.loadingComments = true;
    
      this.commentServices.getComments(this.editTask.taskId).subscribe(
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
        attendants: this.peopleToTask
      }
    });

    dialogRef.afterClosed().subscribe(
      (attendant: PersonModel) => {
        if (attendant) {
          this.selectedAttendant = attendant;
          this.newTaskForm.get('attendant').setValue(`${attendant.personCode} - ${attendant.fullName}`);          
        }
      }, error => {
        console.error(error);
      }
    )
  }  
}