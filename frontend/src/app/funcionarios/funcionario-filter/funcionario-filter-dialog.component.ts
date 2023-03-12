import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subject, takeUntil } from 'rxjs';
import { FuncionariosService } from '../funcionarios.service';
import { Filters } from '../model/funcionario';

@Component({
  selector: 'app-funcionario-filter-dialog',
  templateUrl: './funcionario-filter-dialog.component.html',
  styleUrls: ['./funcionario-filter-dialog.component.css']
})
export class FuncionarioFilterDialogComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  filterEnabled!: FormGroup;
  filters!: FormGroup;

  public cpfMask = [/\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '-', /\d/, /\d/];

  get nomeIsEnabled() { return this.filterEnabled.get('forNome')!.value; }
  get cpfIsEnabled() { return this.filterEnabled.get('forCpf')!.value; }
  get matriculaIsEnabled() { return this.filterEnabled.get('forMatricula')!.value; }

  get nome() { return this.filters.get('nome')!; }
  get cpf() { return this.filters.get('cpf')!; }
  get matricula() { return this.filters.get('matricula')!; }

  constructor(
    private funcionariosService: FuncionariosService,
    public dialogRef: MatDialogRef<FuncionarioFilterDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {
    this.funcionariosService.getFilters()
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        filters => {
          this.filterEnabled =  new FormGroup({
            forNome: new FormControl(!filters.nome.disabled),
            forCpf: new FormControl(!filters.cpf.disabled),
            forMatricula: new FormControl(!filters.matricula.disabled),
          });

          this.filters = new FormGroup({
            nome: new FormControl(
              {value: filters.nome.disabled ? '' : filters.nome.value, disabled: filters.nome.disabled},
              {nonNullable: true, validators: [Validators.required, Validators.maxLength(100)]}
            ),
            cpf: new FormControl(
              {value: filters.cpf.disabled ? '' : filters.cpf.value, disabled: filters.cpf.disabled},
              {nonNullable: true, validators: [Validators.required, Validators.minLength(14), Validators.maxLength(14)]}
            ),
            matricula: new FormControl(
              {value: filters.matricula.disabled ? '' : filters.matricula.value, disabled: filters.matricula.disabled},
              {nonNullable: true, validators: [Validators.required, Validators.minLength(6), Validators.maxLength(11)]}
            )
          });
        }
      );

    this.filterEnabled.valueChanges
      .pipe(takeUntil(this.destroyed$))  
      .subscribe(
        isEnabled => {
          if (isEnabled.forNome) {
            this.nome.enable();
          } else {
            this.nome.reset();
            this.nome.disable();
          }

          if (isEnabled.forCpf) {
            this.cpf.enable();
          } else {
            this.cpf.reset();
            this.cpf.disable();
          }

          if (isEnabled.forMatricula) {
            this.matricula.enable();
          } else {
            this.matricula.reset();
            this.matricula.disable();
          }
        }
      );
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSubmit(): void {
    let newFilters: Filters = {
      nome: {
        value: this.nome.disabled ? '' : this.nome.value,
        disabled: this.nome.disabled
      },
      cpf: {
        value: this.cpf.disabled ? '' : this.cpf.value,
        disabled: this.cpf.disabled
      },
      matricula: {
        value: this.matricula.disabled ? '' : this.matricula.value,
        disabled: this.matricula.disabled
      },
    };

    this.funcionariosService.updateFilters(newFilters);
    this.funcionariosService.updateHasFilter(!this.filters.disabled);

    this.dialogRef.close(this.filters.getRawValue());
  }
}