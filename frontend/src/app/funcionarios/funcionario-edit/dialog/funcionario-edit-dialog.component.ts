import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Funcionario } from '../../model/funcionario';

@Component({
  selector: 'app-funcionario-edit-dialog',
  templateUrl: './funcionario-edit-dialog.component.html',
  styleUrls: ['./funcionario-edit-dialog.component.css']
})
export class FuncionarioEditDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<FuncionarioEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Funcionario,
  ) {}
}
