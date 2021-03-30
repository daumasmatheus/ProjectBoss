import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { CloseScrollStrategy } from '@angular/cdk/overlay';
import { ThrowStmt } from '@angular/compiler';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { EventManager } from '@angular/platform-browser';
import { Task, Tasks } from 'ngx-ui-loader';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { PersonModel } from '../../models/person.1.model';
import { ProjectTask } from '../../models/project-task.model';
import { ProjectDataModel } from '../../models/project.model';
import { TaskModel } from '../../models/task.model';
import { TaskServices } from '../../services/task.services';
import { AddTaskDialogComponent } from '../add-task-dialog/add-task-dialog.component';
import { ProjectDetailComponent } from '../project-detail/project-detail.component';

@Component({
  selector: 'app-board',
  templateUrl: './board.component.html',
  styleUrls: ['./board.component.css']
})
export class BoardComponent implements OnInit {
  localStorageUtils = new LocalStorageUtils();

  loadingTasks: boolean = false;

  projectId: string;
  project: ProjectDataModel;
  
  tasks: TaskModel[];

  plannedTasks: TaskModel[];
  onGoingTasks: TaskModel[];
  onHoldTasks: TaskModel[];
  finishedTasks: TaskModel[];

  attendants: PersonModel[];

  constructor(private taskServices: TaskServices,
              private dialog: MatDialog,
              private snackHelper: SnackBarHelper) { }

  ngOnInit(): void { 
    if (this.projectId) {
      this.loadTasks();
    }
  }

  loadTasks(){
    this.loadingTasks = true;
    this.taskServices.getTasksByProjectId(this.projectId).subscribe(
      (resp: TaskModel[]) => {
         this.tasks = resp;      
         
         this.plannedTasks = resp.filter(x => x.statusId == 1);
         this.onGoingTasks = resp.filter(x => x.statusId == 2);
         this.onHoldTasks = resp.filter(x => x.statusId == 3);
         this.finishedTasks = resp.filter(x => x.statusId == 4);

         this.loadingTasks = false;
      }
    )
  }

  toggleTaskStatus(event: CdkDragDrop<TaskModel[]>) {
    if (event.item.data.authorId == this.localStorageUtils.getUser().id || 
        (this.localStorageUtils.checkUserClaim("Administrator") || this.localStorageUtils.checkUserClaim("ProjectManager"))){
      if (event.previousContainer === event.container) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      } else {      
        let parameters = {
          taskId: event.item.data.taskId,
          statusId: this.getTaskStatus(event.container.id)
        };
  
        this.taskServices.toggleTaskStatus(parameters).subscribe(
          (resp: any) => {
            if (resp) {
              transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
            }
          }, (error: any) => {
            this.snackHelper.showSnackbar("Falha ao mudar o status da tarefa", MessageType.ErrorMessage, 3000);
            console.log(error);
          }
        );
      }
    } else {
      this.snackHelper.showSnackbar("Você nao possui permissão para mudar o status de uma tarefa que não lhe pertence", MessageType.WarningMessage, 3000);
    }
  }

  getTaskStatus(statusIdString: string) {
    switch (statusIdString) {
      case "planned-list":
        return 1;
      case "onGoing-list":
        return 2;
      case "onHold-list":
        return 3;
      case "finished-list":
        return 4;      
    }
  }
  
  openTaskDetails(task: TaskModel){
    let dialogRef = this.dialog.open(AddTaskDialogComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        task: task,
        fromProject: true,
        editTask: true,
        attendants: this.attendants,
        projectConclusionDate: this.project.conclusionDate
      }
    });

    dialogRef.afterClosed().subscribe(
      (task: any) => {
        if (task) {
          this.saveTaskChanges(task);
        }
      }
    )
  }

  saveTaskChanges(taskEdit: any){ 
    this.taskServices.editTask(taskEdit).subscribe(
      (resp: any) => {
        if (resp) {
          this.snackHelper.showSnackbar("Tarefa editada com sucesso", MessageType.OkMessage, 3000);
          this.loadTasks();
        }
      }, (error: any) => {
        this.snackHelper.showSnackbar("Tarefa editada com sucesso", MessageType.OkMessage, 3000);
        console.error(error);
      }
    )
  }
}