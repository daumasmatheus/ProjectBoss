import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FileTypes } from '../filetypes.enum';
import { LocalStorageUtils } from '../localstorage';

@Component({
  selector: 'app-download-file-dialog',
  templateUrl: './download-file-dialog.component.html',
  styleUrls: ['./download-file-dialog.component.css']
})
export class DownloadFileDialogComponent implements OnInit {
  private localStorageUtils = new LocalStorageUtils();

  filesTypes: FileTypes[];
  file: string;
  checkboxTitle: string;
  isAdmin: boolean = false;
  restrictData: boolean = false;
  displayRestrictCheckbox: boolean;

  constructor(private dialogRef: MatDialogRef<DownloadFileDialogComponent>,
              @Inject(MAT_DIALOG_DATA) public data: any) { 
    this.isAdmin = this.localStorageUtils.checkUserClaim("administrator");
  }

  ngOnInit(): void {
    if (this.data != null) {
      this.filesTypes = this.data.fileTypes;
      this.file = this.data.file;
      this.displayRestrictCheckbox = this.data.displayRestrictCheckbox;
      if (this.data.checkboxTitle) {
        this.checkboxTitle = this.data.checkboxTitle;
      }      
    }
  }

  closeDialog(){
    this.dialogRef.close();
  }

  downloadFile(fileType: FileTypes){
    let data = {
      fileType: fileType,
      restrictData: this.restrictData
    }
    console.log(data);

    this.dialogRef.close(data);
  }

  restrictDataValue(value: boolean){
    this.restrictData = !value;
  }
}
