import { Component, Inject, OnInit } from '@angular/core';
import { inject } from '@angular/core/testing';
import { FormControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { debounceTime, startWith, switchMap } from 'rxjs/operators';
import { PersonModel } from '../../models/person.1.model';
import { of } from "rxjs";

@Component({
  selector: 'app-select-attendant-list',
  templateUrl: './select-attendant-list.component.html',
  styleUrls: ['./select-attendant-list.component.css']
})
export class SelectAttendantListComponent implements OnInit {
  filter = new FormControl();  
  attendants: PersonModel[];  
  selectedAttendant: PersonModel;

  loading: boolean = false;

  constructor(private dialogRef: MatDialogRef<SelectAttendantListComponent>,
              @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    this.loading = true;
    if (this.data && this.data.attendants) {
      this.attendants = this.data.attendants;      
    }  

    this.loading = false;
  }
  
  compareFn = (opt1: any, opt2: any) => opt1.personId === opt2.personId; 

  close(){
    this.dialogRef.close();
  }

  saveSelected(){
    this.dialogRef.close(this.selectedAttendant[0]);
  }

  $filter = this.filter.valueChanges.pipe(
    startWith(null),
    debounceTime(200),
    switchMap((resp: any) => {
      if (!resp) 
        return of(this.attendants);     

      return of(
        this.attendants.filter(x => x.fullName.toLowerCase().includes(resp.toLowerCase()) || x.personCode == resp)
      );
    })
  );
}