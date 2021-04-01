import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { MatDialog } from '@angular/material/dialog';
import { AddNewTaskComponent } from './add-new-task/add-new-task.component';
import { NewTaskModel } from '../models/new-task.model';
import { TaskServices } from '../services/task.services';
import { TaskModel } from '../models/task.model';
import { LocalStorageUtils } from 'src/app/helpers/localstorage';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { DownloadFileDialogComponent } from 'src/app/helpers/download-file-dialog/download-file-dialog.component';
import { FileTypes } from 'src/app/helpers/filetypes.enum';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';
import { PersonModel } from '../models/person.1.model';
import { PersonServices } from '../services/person.services';

@Component({
  selector: 'app-tasks',
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit, AfterViewInit {
  private localStorageUtils = new LocalStorageUtils();

  tasks: TaskModel[];
  displayedColumns: string[] = ['status', 'title', 'conclusionDate', 'priority'];
  dataSource: MatTableDataSource<TaskModel>;

  peopleToTask: PersonModel[];

  isCommonUser: boolean = false;
  isProjecManager: boolean = false;
  isAdmin: boolean = false;

  isProjectManagerOrAdmin: boolean = this.isProjecManager || this.isAdmin;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private snackHelper: SnackBarHelper, private dialog: MatDialog, private taskService: TaskServices,
              private personServices: PersonServices) {
    this.isCommonUser = this.checkRole("User");
    this.isProjecManager = this.checkRole("Project Manager");
    this.isAdmin = this.checkRole("Administrator");

    this.getTasks();
    
    if (this.isProjecManager || this.isAdmin) {
      this.getPeopleToTask();
    }
  }  

  ngOnInit(): void { }  

  ngAfterViewInit() {
    this.paginatorConfig(this.paginator);
  }

  openNewTaskDialog(){
    let dialogRef = this.dialog.open(AddNewTaskComponent, {
      hasBackdrop: true,      
      maxHeight: '90vh',
      width: '700px',
      data: {
        attendants: this.peopleToTask
      }
    });    

    dialogRef.afterClosed().subscribe((resp: boolean) => {
      if (resp) {
        this.getTasks();
      }      
    });
  }

  openEditTaskDialog(task: TaskModel){
    let dialogRef = this.dialog.open(AddNewTaskComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        editTask: task,
        attendants: this.peopleToTask
      }
    });

    dialogRef.afterClosed().subscribe((resp: boolean) => {
      if (resp) {
        this.getTasks();
      }
    })
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }  

  paginatorConfig(paginator: MatPaginator){
    paginator._intl.nextPageLabel = 'Próximo';
    paginator._intl.previousPageLabel = 'Anterior';
    paginator._intl.itemsPerPageLabel = 'Itens por página';
    paginator._intl.getRangeLabel = (page: number, pageSize: number, length: number) => {
      const start = page * pageSize + 1;
      const end = (page + 1) * pageSize;
      return `${start} - ${end} de ${length}`;
    }
  }

  selectTask(task: TaskModel){
    if (task != null){
      this.openEditTaskDialog(task);
    }
  }

  getTasks(){    
    switch (true) {
      case this.isCommonUser:
        this.getAllTasksByAttendantId();
        break;
      case this.isProjecManager:
        this.getAllTaksByAuthorId();
        break;
      case this.isAdmin:
        this.getAllActiveTasks();
        break;      
    }    
  }

  getAllActiveTasks(){
    this.taskService.getAllActiveTasks().subscribe(
      (tasks: TaskModel[]) => {
        this.tasks = tasks;
        this.arrangeTable();
      }
    )
  }

  getAllTaksByAuthorId(){
    this.taskService.getAllTasksByAuthorId(this.localStorageUtils.getUser().personId).subscribe(
      (tasks: TaskModel[]) => {
        this.tasks = tasks;
        this.arrangeTable();
      }
    )
  }

  getAllTasksByAttendantId(){
    this.taskService.getAllTasksByAttendantId(this.localStorageUtils.getUser().personId).subscribe(
      (tasks: TaskModel[]) => {
        this.tasks = tasks;
        this.arrangeTable();
      }
    )
  }
  
  arrangeTable(){
    this.dataSource = new MatTableDataSource(this.tasks);

    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  importTasks() {
    let dialogRef = this.dialog.open(DownloadFileDialogComponent, {
      hasBackdrop: true, 
      data: {
        file: "Tarefas",
        checkboxTitle: "Apenas minhas tarefas",
        displayRestrictCheckbox: true,
        fileTypes: [FileTypes.Xlsx, FileTypes.Pdf]
      }
    })

    dialogRef.afterClosed().subscribe((resp: any) => {
      if (resp) {
        switch (resp.fileType) {
          case FileTypes.Xlsx:
              this.downloadTasksXlsx(resp.restrictData);
            break;        
          case FileTypes.Pdf:
            this.downloadTasksPdf(resp.restrictData);
            break;
        }
      }
    });
  }

  downloadTasksXlsx(restrictData: boolean){
    let userId = this.localStorageUtils.getUser().id; 

    this.taskService.downloadTasksXlsl(userId, restrictData).subscribe(
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
    );
  }

  downloadTasksPdf(restrictData: boolean){
    let userId = this.localStorageUtils.getUser().id; 

    this.taskService.downloadTasksPdf(userId, restrictData).subscribe(
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

  checkRole(role: string) {
    return this.localStorageUtils.checkUserClaim(role);
  }

  getPeopleToTask(){
    this.personServices.getAll().subscribe(
      (resp: PersonModel[]) => {
        setTimeout(() => {
          this.peopleToTask = resp;          
        }, 1000);
      }, error => console.error(error)
    );
  }
}