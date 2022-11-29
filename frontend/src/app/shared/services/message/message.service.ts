import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MessageSnackBarComponent } from '../../components/message-snack-bar/message-snack-bar/message-snack-bar.component';
import { Message } from '../../model/message';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  constructor(private _snackBar: MatSnackBar) { }

  notify(message: Message): void {
    this._snackBar.openFromComponent(MessageSnackBarComponent, {
      duration: 4000,
      data: message,
      horizontalPosition: 'start',
      verticalPosition: 'bottom',
    });
  }
}