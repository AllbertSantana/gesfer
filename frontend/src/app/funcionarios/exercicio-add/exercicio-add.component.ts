import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, finalize, takeUntil } from 'rxjs';
import { MessageTypes } from 'src/app/shared/model/message';
import { MessageService } from 'src/app/shared/services/message/message.service';
import { FuncionarioExercicioService } from '../funcionario-exercicio.service';
import { NewExercicio } from '../model/exercicio';
import * as moment from 'moment';

@Component({
  selector: 'app-exercicio-add',
  templateUrl: './exercicio-add.component.html',
  styleUrls: ['./exercicio-add.component.css']
})
export class ExercicioAddComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  addExercicioForm!: FormGroup;
  funcionarioId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private message: MessageService,
    public exercicioService: FuncionarioExercicioService,
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
      this.funcionarioId = parseInt(id);
      this.createForm();
    }
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get exercicio() { return this.addExercicioForm.get('exercicio') as FormGroup; }
  get exercicioInicio() { return this.exercicio.get('inicio'); }
  get exercicioFim() { return this.exercicio.get('fim'); }

  get ferias() { return this.addExercicioForm.get('ferias') as FormArray<FormGroup>; }
  feriasInicio(index: number) { return this.ferias.get(index.toString())!.get('inicio'); }
  feriasFim(index: number) { return this.ferias.get(index.toString())!.get('fim'); }

  createForm(): void {
    this.addExercicioForm = new FormGroup({
      exercicio: new FormGroup({
        inicio: new FormControl<Date | null>(null, {validators: Validators.required}),
        fim: new FormControl<Date | null>(null, {validators: Validators.required})
      }),
      ferias: new FormArray<FormGroup>([])
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

  onSubmit() {
    if (this.addExercicioForm.valid) {
      this.addExercicioForm.disable();

      let newExercicio: NewExercicio = {
        dataInicio: this.addExercicioForm.value.exercicio.inicio.format('YYYY-MM-DD'),
        dataFim: this.addExercicioForm.value.exercicio.fim.format('YYYY-MM-DD'),
        ferias: this.addExercicioForm.value.ferias.map((periodo: any) => {
          return {
            dataInicio: periodo.inicio.format('YYYY-MM-DD'),
            dataFim: periodo.fim.format('YYYY-MM-DD'),
          }
        })
      }

      this.exercicioService.addExercicio(newExercicio, this.funcionarioId)
        .pipe(
          takeUntil(this.destroyed$),
          finalize(() => this.addExercicioForm?.enable())
        )
        .subscribe();
    }
  }
}
