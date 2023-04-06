import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FilterComponent } from './components/filter/filter.component';
import { SelectComponent } from './components/inputs/select/select.component';
import { TextspanComponent } from './components/inputs/textspan/textspan.component';
import { MessageSnackBarComponent } from './components/message-snack-bar/message-snack-bar/message-snack-bar.component';
import { ReactiveFormsModule } from '@angular/forms';
import { TextMaskModule } from 'angular2-text-mask';
import { SharedMaterialModule } from './shared-material.module';
import { RemoveDialogComponent } from './components/remove/remove-dialog.component';

@NgModule({
  declarations: [
    MessageSnackBarComponent,
    RemoveDialogComponent,
    FilterComponent,
    TextspanComponent,
    SelectComponent,
  ],
  imports: [
    CommonModule,
    SharedMaterialModule,
    ReactiveFormsModule,
    TextMaskModule,
  ],
  exports: [
    TextspanComponent,
    SelectComponent,
    SharedMaterialModule,
  ]
})
export class SharedModule { }
