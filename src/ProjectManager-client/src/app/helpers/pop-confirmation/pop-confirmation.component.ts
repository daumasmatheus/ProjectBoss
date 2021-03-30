import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-pop-confirmation',
  templateUrl: './pop-confirmation.component.html',
  styleUrls: ['./pop-confirmation.component.css']
})
export class PopConfirmationComponent implements OnInit {
  title: string;
  message: string;

  constructor(private dialogRef: MatDialogRef<PopConfirmationComponent>,
              @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit(): void {
    if (this.data != null) {
      this.title = this.data.title;
      this.message = this.data.message;
    }
  }

  close(response: boolean){
    this.dialogRef.close(response);
  }
}