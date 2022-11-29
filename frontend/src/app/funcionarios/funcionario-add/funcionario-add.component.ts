import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Cpf } from 'src/app/shared/model/cpf';
import { Matricula } from 'src/app/shared/model/matricula';

@Component({
  selector: 'app-funcionario-add',
  templateUrl: './funcionario-add.component.html',
})
export class FuncionarioAddComponent implements OnInit {
  addFuncionarioForm!: FormGroup;

  ngOnInit(): void {
    this.addFuncionarioForm = new FormGroup({
      nome: new FormControl('', [Validators.required]),
      cpf: new FormControl(Cpf.fromString('123.456.789-10')),
      matricula: new FormControl(new Matricula('', ''))
    });
  }

  get nome() { return this.addFuncionarioForm.get('nome'); }
  get cpf() { return this.addFuncionarioForm.get('cpf'); }
  get matricula() { return this.addFuncionarioForm.get('matricula'); }

  onSubmit() {
    console.log(this.addFuncionarioForm.value);
  }
}
