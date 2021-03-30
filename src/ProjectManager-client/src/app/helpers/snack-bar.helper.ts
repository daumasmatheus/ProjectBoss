import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from "@angular/material/snack-bar";

import { MessageType } from './message-type.enum';

@Injectable()
export class SnackBarHelper {
    constructor(private snackBar: MatSnackBar) { }

    showSnackbar(message: string, messageType: MessageType, duration?: number) {
        let snackBarConfig: MatSnackBarConfig = {
            duration: duration,
            horizontalPosition: 'start',
            verticalPosition: 'top',
            panelClass: this.getClass(messageType)
        }

        this.snackBar.open(message, 'x', snackBarConfig)
    }

    private getClass(messageType: MessageType){
        switch (messageType) {
            case 1:
                return 'snack-ok';
            case 2:
                return 'snack-error';
            case 3:
                return 'snack-warn';
            case 4:
                return 'snack-info';
        }
    }
}