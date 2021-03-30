import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TableComponentComponent } from '../../../base-components/table-component/table-component.component';
import { PersonModel } from '../../../models/person.1.model';
import { PersonServices } from '../../../services/person.services';

@Component({
  selector: 'app-select-attendant-dialog',
  templateUrl: './select-attendant-dialog.component.html',
  styleUrls: ['./select-attendant-dialog.component.css']
})
export class SelectAttendantDialogComponent implements OnInit {
  newAttendants: PersonModel[] = [];
  currentAttendants: PersonModel[] = [];

  displayedColumns: string[] = ['fullName'];
  displayedColumnNames: string[] = ['Nome'];

  @ViewChild('attendantsTable') attendantsTable: TableComponentComponent;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, 
              public dialogRef: MatDialogRef<SelectAttendantDialogComponent>,
              private personServices: PersonServices) { }

  ngOnInit(): void {
    if (this.data && this.data.attendants){
      this.currentAttendants = this.data.attendants;
    }

    this.loadAttendants();
  }

  loadAttendants(){
    this.personServices.getAll().subscribe(
      (resp: PersonModel[]) => {
        this.newAttendants = resp.filter((el1) => {
          return !this.currentAttendants.find((el2) => {
            return el1.personId === el2.personId
          });
        });        
      }
    )
  }

  selectAttendants(){
    this.dialogRef.close(this.attendantsTable.selection.selected.map(el => { return el.personId }));
  }

  closeDialog(){
    this.dialogRef.close(null);
  }
}