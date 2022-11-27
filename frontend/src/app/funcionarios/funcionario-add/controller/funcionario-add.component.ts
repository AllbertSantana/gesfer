import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { FuncionarioAddDialogComponent } from '../dialog/funcionario-add-dialog.component';

@Component({
  selector: 'app-funcionario-add',
  template: ``,
})
export class FuncionarioAddComponent implements OnInit {
  constructor(
    private location: Location,
    private dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    this.openDialog();
  }

  openDialog(): void {
    const dialogRef = this.dialog.open(FuncionarioAddDialogComponent);

    dialogRef.afterClosed().subscribe(
      result => {
        this.location.back();
      }
    );
  }
}
