import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { finalize, Subject, takeUntil } from 'rxjs';
import { FuncionariosService } from '../funcionarios.service';
import { Funcionario } from '../model/funcionario';

@Component({
  selector: 'app-funcionario-add',
  templateUrl: './funcionario-add.component.html',
})
export class FuncionarioAddComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  addFuncionarioForm!: FormGroup;

  public cpfMask = [/\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '.', /\d/, /\d/, /\d/, '-', /\d/, /\d/];

  constructor(public funcionariosService: FuncionariosService) {}

  ngOnInit(): void {
    this.addFuncionarioForm = new FormGroup({
      nome: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.maxLength(100)]}),
      cpf: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(14), Validators.maxLength(14)]}),
      matricula: new FormControl('', {nonNullable: true, validators: [Validators.required, Validators.minLength(6), Validators.maxLength(11)]})
    });
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get nome() { return this.addFuncionarioForm.get('nome')!; }
  get cpf() { return this.addFuncionarioForm.get('cpf')!; }
  get matricula() { return this.addFuncionarioForm.get('matricula')!; }

  onSubmit() {
    this.addFuncionarioForm.disable();

    let funcionarioNovo: Funcionario = {
      id: 0,
      nome: this.addFuncionarioForm.value.nome,
      cpf: this.addFuncionarioForm.value.cpf,
      matricula: this.addFuncionarioForm.value.matricula
    };

    this.funcionariosService.addFuncionario(funcionarioNovo)
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => this.addFuncionarioForm?.enable())
      )
      .subscribe();
  }
}
