import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, finalize, take, takeUntil } from 'rxjs';
import { MessageTypes } from 'src/app/shared/model/message';
import { MessageService } from 'src/app/shared/services/message/message.service';
import { FuncionarioExercicioService } from '../funcionario-exercicio.service';
import { FuncionarioExercicios } from '../model/exercicio';
import { MatDialog } from '@angular/material/dialog';
import { RemoveDialogComponent } from '../../shared/components/remove/remove-dialog.component';
import { AuthService } from 'src/app/shared/services/auth/auth.service';

@Component({
  selector: 'app-exercicio-list',
  templateUrl: './exercicio-list.component.html',
  styleUrls: ['./exercicio-list.component.css']
})
export class ExercicioListComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  selectedFuncionarioExercicios?: FuncionarioExercicios;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public exercicioService: FuncionarioExercicioService,
    public authService: AuthService,
    private message: MessageService,
    public dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id === null || isNaN(parseInt(id, 10))) {
      this.router.navigateByUrl('/funcionarios');

      this.message.notify({
        title: 'ID requisitado é inválido',
        type: MessageTypes.error
      })
    } else {
      this.loadExercicios(parseInt(id, 10));
    }
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  loadExercicios(id: number): void {
    this.exercicioService.getExercicios(id)
      .pipe(take(1))
      .subscribe(
        exercicios => {
          if (exercicios === null) {
            this.router.navigateByUrl('/funcionarios');
          } else {
            this.selectedFuncionarioExercicios = exercicios;
          }
        }
      )
  }

  openRemoveDialog(dataInicial: string, exercicioId: number): void {
    const removeDialogRef = this.dialog.open(RemoveDialogComponent, {
      data: {
        exercicioDataInicial: dataInicial
      },
    });

    removeDialogRef.afterClosed()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        (proceeded: boolean) => {
          if (proceeded) {
            this.exercicioService.deleteExercicio(this.selectedFuncionarioExercicios!.id, exercicioId)
              .pipe(
                takeUntil(this.destroyed$),
                finalize(() => this.loadExercicios(this.selectedFuncionarioExercicios!.id))
              )
              .subscribe();
          }
        }
      );
  }

  formatDate(date: string, withDay: boolean): string {
    let dateObj = new Date(date);

    return withDay
    ? dateObj.toLocaleDateString('pt-BR', {timeZone: 'UTC', year: 'numeric', month: 'numeric', day: 'numeric'})
    : dateObj.toLocaleDateString('pt-BR', {timeZone: 'UTC', year: 'numeric', month: 'numeric'})
  }

}
