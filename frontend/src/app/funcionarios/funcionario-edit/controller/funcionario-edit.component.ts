import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { FuncionariosService } from '../../funcionarios.service';
import { Funcionario } from '../../model/funcionario';
import { FuncionarioEditDialogComponent } from '../dialog/funcionario-edit-dialog.component';

@Component({
  selector: 'app-funcionario-edit',
  template: ``,
})
export class FuncionarioEditComponent implements OnInit {
  destroyed$ = new Subject<void>;
  selectedFuncionario?: Funcionario;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private dialog: MatDialog,
    private funcionariosService: FuncionariosService,
  ) { }

  ngOnInit(): void {
    this.getFuncionario();
  }

  getFuncionario(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.funcionariosService.getSelectedRows()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        selection => this.selectedFuncionario = selection.get(id)
      );

    if (this.selectedFuncionario == undefined) {
      this.router.navigateByUrl('/funcionarios');
    } else {
      this.openDialog(this.selectedFuncionario);
    }
  }

  openDialog(funcionario: Funcionario): void {
    const dialogRef = this.dialog.open(FuncionarioEditDialogComponent, {
      data: funcionario,
    });

    dialogRef.afterClosed().subscribe(
      result => {
        this.location.back();
      }
    );
  }
}
