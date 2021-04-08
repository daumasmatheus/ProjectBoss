import { AfterViewInit, Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { throwToolbarMixedModesError } from '@angular/material/toolbar';
import { MessageType } from 'src/app/helpers/message-type.enum';
import { PopConfirmationComponent } from 'src/app/helpers/pop-confirmation/pop-confirmation.component';
import { SnackBarHelper } from 'src/app/helpers/snack-bar.helper';
import { TableComponentComponent } from '../../base-components/table-component/table-component.component';
import { PersonModel } from '../../models/person.1.model';
import { PersonInProjectServices } from '../../services/personInProject.services';
import { SelectAttendantDialogComponent } from './select-attendant-dialog/select-attendant-dialog.component';

@Component({
  selector: 'app-attendants',
  templateUrl: './attendants.component.html',
  styleUrls: ['./attendants.component.css']
})
export class AttendantsComponent implements OnInit, AfterViewInit {
  @ViewChild(TableComponentComponent) tableComponent: TableComponentComponent; 

  @Input() projectId: string;
  @Input() attendants: PersonModel[];

  @Output() attendantRemovedEvt = new EventEmitter<boolean>();
  @Output() attendantsIncludedEvt = new EventEmitter<boolean>();

  displayedColumns: string[] = ['fullName', 'tasksAsigned'];
  displayedColumnNames: string[] = ['Nome', 'Tarefas'];

  constructor(private personInProjectServices: PersonInProjectServices,
              private snackHelper: SnackBarHelper,
              private dialog: MatDialog) { }

  ngAfterViewInit(): void {    
  }

  ngOnInit(): void {        
  }

  removeAttendants(){
    let parameters = {
      projectId: this.projectId,
      attendantIds: this.tableComponent.selection.selected.map(el => { return el.personId })
    }

    let dialogRef = this.dialog.open(PopConfirmationComponent, {
      hasBackdrop: true,
      data: {
        title: `Remover Participante(s)`,
        message: 'Deseja remover o(s) participante(s)? A operação não poderá ser desfeita.'
      }
    })

    dialogRef.afterClosed().subscribe((resp: boolean) => {
      if (resp) {
        this.personInProjectServices.removeProjectAttendants(parameters).subscribe(
          (resp: boolean) => {
            if (resp) {          
              this.attendantRemovedEvt.emit(true); 
              this.snackHelper.showSnackbar("Participante removido com sucesso", MessageType.OkMessage, 3000);          
            }
          }, error => {
            this.snackHelper.showSnackbar("Falha ao remover o participante", MessageType.ErrorMessage, 3000);
            console.error(error);
          }
        );
      }
    });    
  }  

  addAttendant(){ 
    let dialogRef = this.dialog.open(SelectAttendantDialogComponent, {
      hasBackdrop: true,
      maxHeight: '540px',
      width: '700px',
      data: {
        attendants: this.attendants
      }      
    });

    dialogRef.afterClosed().subscribe(
      (personIds: string[]) => {
        if (personIds && personIds.length > 0){
          let newAttendants = {
            projectId: this.projectId,
            attendantIds: personIds
          }

          this.personInProjectServices.includeNewAttendant(newAttendants).subscribe(
            (resp: boolean) => {
              if (resp){
                this.snackHelper.showSnackbar("Participante(s) incluído(s) com sucesso", MessageType.OkMessage, 3000);
                this.attendantsIncludedEvt.emit(true);
              }
            }, error => {
              this.snackHelper.showSnackbar("Falha ao incluir novo(s) participante(s)", MessageType.OkMessage, 3000);
              console.error(error);
            }
          )
        }        
      }
    )
  }
}