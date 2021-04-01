import { AfterViewInit, Component, OnInit, Output, ViewChild } from '@angular/core';
import { ProjectDataModel, ProjectModel } from '../../models/project.model';
import { TaskModel } from '../../models/task.model';
import { ProjectServices } from '../../services/project.services';
import { TaskServices } from '../../services/task.services';
import { BoardComponent } from '../board/board.component';
import { ProjectHeaderComponent } from '../project-header/project-header.component';
import { Guid } from "guid-typescript";
import { PersonModel } from '../../models/person.1.model';
import { MatDialog } from '@angular/material/dialog';
import { ProjectDetailComponent } from '../project-detail/project-detail.component';
import { AddTaskDialogComponent } from '../add-task-dialog/add-task-dialog.component';
import { PersonInProjectServices } from '../../services/personInProject.services';
import { ProjectTask } from '../../models/project-task.model';
import { NewTaskModel } from '../../models/new-task.model';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { AttendantsComponent } from '../attendants/attendants.component';
import { MatTabGroup } from '@angular/material/tabs';
import { DownloadFileDialogComponent } from 'src/app/helpers/download-file-dialog/download-file-dialog.component';
import { FileTypes } from 'src/app/helpers/filetypes.enum';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.css']
})
export class ProjectComponent implements OnInit {  
  localStorageUtils = new LocalStorageUtils();

  @ViewChild(BoardComponent) boardComponent: BoardComponent;  
  @ViewChild(AttendantsComponent) attendantsComponent: AttendantsComponent;

  project: ProjectDataModel;
  projectId: string;

  tasks: TaskModel[];
  attendants: PersonModel[];

  isProjectManagerOrAdmin: boolean = this.localStorageUtils.checkUserClaim("Project Manager") || this.localStorageUtils.checkUserClaim("Administrator");

  constructor(private projectServices: ProjectServices,
              private taskServices: TaskServices,
              private personInProjectServices: PersonInProjectServices,
              private dialog: MatDialog,
              private snackHelper: SnackBarHelper) { }  

  ngOnInit(): void {}   

  toggleProjectStatus(){
    this.projectServices.toggleProjectStatus(JSON.stringify(this.projectId)).subscribe(
      (resp: boolean) => {
        this.snackHelper.showSnackbar("Status do projeto alterado com sucesso", MessageType.OkMessage, 3000);
        this.getProjectDataById(this.projectId);
      }, error => {
        this.snackHelper.showSnackbar("Falha ao alterar o status do projeto", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }  

  getProjectDataById($event){
    if ($event == '0')
        return;

    this.projectId = $event;
    
    this.projectServices.getProjectDataById(this.projectId).subscribe(
      (resp: ProjectDataModel) => {
        this.project = resp;        
        
        this.boardComponent.projectId = resp.projectId;
        this.boardComponent.project = resp;       

        this.boardComponent.ngOnInit();

        this.getProjectAttendants();
      }
    );
  }

  openProjectDetails(){
    let dialogRef = this.dialog.open(ProjectDetailComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        project: this.project
      }
    });

    dialogRef.afterClosed().subscribe(
      (resp: ProjectDataModel) => {
        if (resp){
          this.updateProjectData(resp);
        }
      }, err => {
        console.error(err);
      }
    )
  }

  updateProjectData(projectData: ProjectDataModel) {
    this.projectServices.editProjectData(projectData).subscribe(
      (resp: boolean) => {
        if (resp) {
          this.snackHelper.showSnackbar("Dados do projeto atualizados com sucesso", MessageType.OkMessage, 3000);
          this.getProjectData();
        }
      }, error => {
        this.snackHelper.showSnackbar("Falha ao atualizar os dados do projeto", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }

  getProjectData() {
    this.projectServices.getProjectDataById(this.projectId).subscribe(
      (resp: ProjectDataModel) => {        
        this.project = resp;        
      }
    );
  }

  addNewTask(){
    let dialogRef = this.dialog.open(AddTaskDialogComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        project: this.project,
        attendants: this.attendants,
        projectConclusionDate: this.project.conclusionDate,
        fromProject: true
      }
    });

    dialogRef.afterClosed().subscribe((task: ProjectTask) => {
      if (task != null) {
        this.saveNewTask(task);
      }
    });  
  }

  saveNewTask(task: ProjectTask) {
    let newTask: NewTaskModel = {
      authorId: task.authorId,
      attendantId: task.attendant.personId,
      conclusionDate: task.conclusionDate,
      description: task.description,
      priorityId: task.priorityId,
      statusId: task.statusId,
      title: task.title,
      projectId: task.projectId
    }

    this.taskServices.newTask(newTask).subscribe(
      (resp: NewTaskModel) => {
        if (resp){
          this.snackHelper.showSnackbar("Tarefa incluída com sucesso", MessageType.OkMessage, 3000);
          this.boardComponent.ngOnInit();
        }
      }
    )
  }

  getProjectAttendants(){
    this.personInProjectServices.getProjectAttendants(this.project.projectId).subscribe(
      (res: PersonModel[]) => {
        this.attendants = res;
        this.boardComponent.attendants = this.attendants;        
      }
    )
  }

  onTabChange($event){
    if ($event.index == 1) {
      this.attendantsComponent.ngOnInit();
    }
  }

  attendantRemoved($event){    
    if ($event){
      this.getProjectDataById(this.projectId);       
    }
  }  

  attendantIncluded($event){
    if ($event){
      this.getProjectDataById(this.projectId);   
    }
  }

  exportProject(){
    let dialogRef = this.dialog.open(DownloadFileDialogComponent, {
      hasBackdrop: true, 
      data: {
        file: `Projeto ${this.project.title}`,        
        displayRestrictCheckbox: false,
        fileTypes: [FileTypes.Xlsx, FileTypes.Pdf]
      }
    })

    dialogRef.afterClosed().subscribe((resp: FileTypes) => {
      if (resp){
        switch (resp) {
          case FileTypes.Pdf:
            return this.downloadPdf();
          case FileTypes.Xlsx:
            return this.downloadXlsx();          
        }
      }
    });
  }

  downloadPdf(){
    this.projectServices.exportProjectAsPdf(this.projectId).subscribe(
      (response: any) => {
        this.snackHelper.showSnackbar("Dados exportados com sucesso", MessageType.OkMessage, 3000);

        var blob = new Blob([response], { type: "application/pdf" });
        window.open(window.URL.createObjectURL(blob));
      }, error => {
        this.snackHelper.showSnackbar("Falha ao gerar o arquivo", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }

  downloadXlsx(){
    this.projectServices.exportProjectAsXlsx(this.projectId).subscribe(
      (response: any) => {
        this.snackHelper.showSnackbar("Dados exportados com sucesso", MessageType.OkMessage, 3000);
        
        var blob = new Blob([response], {
          type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });

        window.open(window.URL.createObjectURL(blob));
      }, error => {
        this.snackHelper.showSnackbar("Falha ao gerar o arquivo", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }
}