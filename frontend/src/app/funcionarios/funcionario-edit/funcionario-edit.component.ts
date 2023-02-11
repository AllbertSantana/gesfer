import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize, Subject, takeUntil, tap } from 'rxjs';
import { FuncionariosService } from '../funcionarios.service';
import { Funcionario } from '../model/funcionario';

@Component({
  selector: 'app-funcionario-edit',
  templateUrl: `funcionario-edit.component.html`,
})
export class FuncionarioEditComponent implements OnInit, OnDestroy {
  destroyed$ = new Subject<void>;
  selectedFuncionario?: Funcionario;
  updateFuncionarioForm?: FormGroup;

  get nome() { return this.updateFuncionarioForm?.get('nome')!; }
  get cpf() { return this.updateFuncionarioForm?.get('cpf')!; }
  get matricula() { return this.updateFuncionarioForm?.get('matricula')!; }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private funcionariosService: FuncionariosService,
  ) { }

  ngOnInit(): void {
    this.getFuncionario();
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  getFuncionario(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (id === null) {
      this.router.navigateByUrl('/funcionarios');
    } else {
      this.funcionariosService.getFuncionarioById(id)
        .pipe(takeUntil(this.destroyed$))
        .subscribe(
          funcionario => {
            if (funcionario === null) {
              this.router.navigateByUrl('/funcionarios');
            } else {
              this.selectedFuncionario = funcionario;
              this.createForm();
            }
          }
        )
    }
  }

  createForm() {
    this.updateFuncionarioForm = new FormGroup({
      nome: new FormControl(this.selectedFuncionario!.nome, {nonNullable: true, validators: [Validators.required, Validators.maxLength(100)]}),
      cpf: new FormControl(this.selectedFuncionario!.cpf, {nonNullable: true, validators: [Validators.required, Validators.minLength(14), Validators.maxLength(14)]}),
      matricula: new FormControl(this.selectedFuncionario!.matricula, {nonNullable: true, validators: [Validators.required, Validators.minLength(6), Validators.maxLength(11)]})
    });
  }

  onSubmit() {
    this.updateFuncionarioForm?.disable();

    let funcionarioEditado: Funcionario = {
      id: this.selectedFuncionario!.id,
      nome: this.updateFuncionarioForm!.value.nome!,
      cpf: this.updateFuncionarioForm!.value.cpf!,
      matricula: this.updateFuncionarioForm!.value.matricula!
    };

    this.funcionariosService.updateFuncionario(funcionarioEditado)
      .pipe(
        takeUntil(this.destroyed$),
        finalize(() => {
          this.updateFuncionarioForm?.enable();
        })
      )
      .subscribe();
  }
}
