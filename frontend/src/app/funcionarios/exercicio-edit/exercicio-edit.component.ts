import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, finalize, takeUntil } from 'rxjs';
import { MessageTypes } from 'src/app/shared/model/message';
import { MessageService } from 'src/app/shared/services/message/message.service';
import { FuncionarioExercicioService } from '../funcionario-exercicio.service';
import { Exercicio, UpdatedExercicio } from '../model/exercicio';
import * as moment from 'moment';

@Component({
  selector: 'app-exercicio-edit',
  templateUrl: './exercicio-edit.component.html',
  styleUrls: ['./exercicio-edit.component.css']
})
export class ExercicioEditComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  selectedExercicio?: Exercicio;
  editExercicioForm?: FormGroup;
  funcionarioId!: number;
  exercicioId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public exercicioService: FuncionarioExercicioService,
    private message: MessageService,
  ) {}

  ngOnInit(): void {
    const funcionarioId = this.route.snapshot.paramMap.get('funcionarioId');
    const exercicioId = this.route.snapshot.paramMap.get('exercicioId');

    if (funcionarioId === null || isNaN(parseInt(funcionarioId, 10))) {
      this.router.navigateByUrl('/funcionarios');

      this.message.notify({
        title: 'ID requisitado para funcionário é inválido',
        type: MessageTypes.error
      })
    } else {
      if (exercicioId === null || isNaN(parseInt(exercicioId, 10))) {
        this.router.navigateByUrl(`/funcionarios/${funcionarioId}/exercicios`);

        this.message.notify({
          title: 'ID requisitado para exercício é inválido',
          type: MessageTypes.error
        })
      } else {
        this.funcionarioId = parseInt(funcionarioId);
        this.exercicioId = parseInt(exercicioId);

        this.exercicioService.getExercicioById(this.funcionarioId, this.exercicioId)
          .pipe(takeUntil(this.destroyed$))
          .subscribe(
            exercicio => {
              if (exercicio === null) {
                this.router.navigateByUrl(`/funcionarios/${funcionarioId}/exercicios`);
              } else {
                this.selectedExercicio = exercicio;
                this.createForm();
              }
            }
          );
      }
    }
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get exercicio() { return this.editExercicioForm?.get('exercicio') as FormGroup; }
  get exercicioInicio() { return this.exercicio.get('inicio'); }
  get exercicioFim() { return this.exercicio.get('fim'); }

  get ferias() { return this.editExercicioForm?.get('ferias') as FormArray<FormGroup>; }
  feriasInicio(index: number) { return this.ferias.get(index.toString())!.get('inicio'); }
  feriasFim(index: number) { return this.ferias.get(index.toString())!.get('fim'); }

  createForm() {
    const exercicioInicio = this.formatDateToUTC(this.selectedExercicio!.dataInicio);
    const exercicioFim = this.formatDateToUTC(this.selectedExercicio!.dataFim);
    const feriasArr = this.selectedExercicio!.ferias.map(ferias => {
      let feriasInicio = this.formatDateToUTC(ferias.dataInicio);
      let feriasFim = this.formatDateToUTC(ferias.dataFim);

      return new FormGroup({
        inicio: new FormControl<Date | null>(feriasInicio, {validators: Validators.required}),
        fim: new FormControl<Date | null>(feriasFim, {validators: Validators.required})
      }) 
    })

    this.editExercicioForm = new FormGroup({
      exercicio: new FormGroup({
        inicio: new FormControl<Date | null>(exercicioInicio, {validators: Validators.required}),
        fim: new FormControl<Date | null>(exercicioFim, {validators: Validators.required})
      }),
      ferias: new FormArray<FormGroup>(feriasArr)
    });
  }

  addFerias(): void {
    this.ferias.push(
      new FormGroup({
        inicio: new FormControl<Date | null>(null, {validators: Validators.required}),
        fim: new FormControl<Date | null>(null, {validators: Validators.required})
      })
    );
  }

  removeFerias(index: number): void {
    this.ferias.removeAt(index);
  }

  formatDateToUTC(date: string): Date {
    let dateObj = new Date(date);
    let utcDateStr = dateObj.toLocaleDateString(undefined, {timeZone: 'UTC', year: 'numeric', month: 'numeric', day: 'numeric'})
    let utcDateObj = new Date(utcDateStr);
    return utcDateObj;
  }

  onSubmit() {
    if (this.editExercicioForm?.valid) {
      this.editExercicioForm.disable();

      let updatedExercicio: UpdatedExercicio = {
        id: this.exercicioId,
        dataInicio: moment(this.editExercicioForm.value.exercicio.inicio).format('YYYY-MM-DD'),
        dataFim: moment(this.editExercicioForm.value.exercicio.fim).format('YYYY-MM-DD'),
        ferias: this.editExercicioForm.value.ferias.map((periodo: any) => {
          return {
            dataInicio: moment(periodo.inicio).format('YYYY-MM-DD'),
            dataFim: moment(periodo.fim).format('YYYY-MM-DD'),
          }
        })
      }

      this.exercicioService.updateExercicio(updatedExercicio, this.funcionarioId, this.exercicioId)
        .pipe(
          takeUntil(this.destroyed$),
          finalize(() => this.editExercicioForm?.enable())
        )
        .subscribe();
    }
  }
}