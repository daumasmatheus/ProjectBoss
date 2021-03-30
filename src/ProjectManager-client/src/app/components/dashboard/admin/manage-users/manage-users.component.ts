import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { faThumbsDown } from '@fortawesome/free-regular-svg-icons';
import { DownloadFileDialogComponent } from 'src/app/helpers/download-file-dialog/download-file-dialog.component';
import { FileTypes } from 'src/app/helpers/filetypes.enum';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { UserDataModel } from '../../models/userData.model';
import { UsersServices } from '../../services/users.services';
import { UserDetailsComponent } from '../user-details/user-details.component';

@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  displayedColumns: string[] = [ 'email', 'role.name', 'provider' ];
  usersDataSource: MatTableDataSource<UserDataModel>;

  users: UserDataModel[];

  constructor(private usersService: UsersServices, 
              private dialog: MatDialog,
              private snackHelper: SnackBarHelper) {
    this.getUsers();
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.paginatorConfig(this.paginator);    
  }

  getUsers(){
    this.usersService.getUsers().subscribe(
      (response: UserDataModel[]) => { 
        console.log(response);
        this.users = response;

        this.usersDataSource = new MatTableDataSource(response);
        // this.usersDataSource.sortingDataAccessor = (item, property) => {
        //   switch (property) {
        //     case 'role.name': return item.role.name
        //     default: return item[property];
        //   }
        // };
        this.usersDataSource.paginator = this.paginator;
        this.usersDataSource.sort = this.sort;
      }, error => {
        console.error(error);
      }
    );
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

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.usersDataSource.filter = filterValue.trim().toLowerCase();

    if (this.usersDataSource.paginator) {
      this.usersDataSource.paginator.firstPage();
    }
  }

  selectUser(selectedUser: UserDataModel){
    let dialogRef = this.dialog.open(UserDetailsComponent, {
      hasBackdrop: true,
      maxHeight: '90vh',
      width: '700px',
      data: {
        user: selectedUser
      }
    });

    dialogRef.afterClosed().subscribe(
      (resp: any) => {
        if (resp) {
          this.editUserData(resp);
        }
      }
    );
  }

  editUserData(userData: UserDataModel){
    this.usersService.editUser(userData).subscribe(
      (resp: boolean) => {
        if (resp) {
          this.snackHelper.showSnackbar("Dados do usuário editados com sucesso", MessageType.OkMessage, 3000);
          this.getUsers();
        }
      }, error => {
        console.error(error);
        this.snackHelper.showSnackbar("Falha ao editar os dados do usuário", MessageType.ErrorMessage, 3000);
      }
    );
  }

  exportData(){
    let dialogRef = this.dialog.open(DownloadFileDialogComponent, {
      hasBackdrop: true,
      data: {
        file: "Usuários",
        displayRestrictCheckbox: false,
        fileTypes: [FileTypes.Pdf, FileTypes.Xlsx]
      }
    });

    dialogRef.afterClosed().subscribe(
      (resp: FileTypes) => {
        switch (resp) {
          case FileTypes.Pdf:
            this.downloadPdf();
            break;    
          case FileTypes.Xlsx:
            this.downloadXlsx();
            break;
        }
      }
    )
  }

  downloadPdf(){
    this.usersService.downloadDataPdf().subscribe(
      (resp: any) => {
        var blob = new Blob([resp], { type: "application/pdf" });
        window.open(window.URL.createObjectURL(blob));

        this.snackHelper.showSnackbar("Dados exportados com sucesso", MessageType.OkMessage, 3000);
      }, error => {
        this.snackHelper.showSnackbar("Falha ao gerar o arquivo", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }

  downloadXlsx(){
    this.usersService.downloadDataXlsl().subscribe(
      (resp: any) => {
        var blob = new Blob([resp], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        window.open(window.URL.createObjectURL(blob));

        this.snackHelper.showSnackbar("Dados exportados com sucesso", MessageType.OkMessage, 3000);
      }, error => {
        this.snackHelper.showSnackbar("Falha ao gerar o arquivo", MessageType.ErrorMessage, 3000);
        console.error(error);
      }
    )
  }
}