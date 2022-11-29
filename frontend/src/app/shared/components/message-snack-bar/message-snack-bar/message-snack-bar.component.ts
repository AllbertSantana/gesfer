import { Component, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { Message } from 'src/app/shared/model/message';

@Component({
  selector: 'app-message-snack-bar',
  templateUrl: './message-snack-bar.component.html',
  styles: [
  ]
})
export class MessageSnackBarComponent {
  constructor(@Inject(MAT_SNACK_BAR_DATA) public message: Message) {}
}
