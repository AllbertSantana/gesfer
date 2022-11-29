import { FocusMonitor } from '@angular/cdk/a11y';
import { BooleanInput, coerceBooleanProperty } from '@angular/cdk/coercion';
import { Component, ElementRef, Inject, Input, OnDestroy, Optional, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormBuilder, NgControl, Validators } from '@angular/forms';
import { MatFormField, MatFormFieldControl, MAT_FORM_FIELD } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { Cpf } from 'src/app/shared/model/cpf';

@Component({
  selector: 'cpf-input',
  templateUrl: './cpf-input.component.html',
  styleUrls: ['./cpf-input.component.css'],
  providers: [{provide: MatFormFieldControl, useExisting: CpfInputComponent}],
  host: {
    '[class.floating]': 'shouldLabelFloat',
    '[id]': 'id',
  },
})
export class CpfInputComponent implements ControlValueAccessor, MatFormFieldControl<Cpf>, OnDestroy {
  static nextId = 0;
  @ViewChild('primeirosTresDigitos') primeirosTresDigitosInput!: HTMLInputElement;
  @ViewChild('segundosTresDigitos') segundosTresDigitosInput!: HTMLInputElement;
  @ViewChild('terceirosTresDigitos') terceirosTresDigitosInput!: HTMLInputElement;
  @ViewChild('ultimosDoisDigitos') ultimosDoisDigitosInput!: HTMLInputElement;

  parts = this._formBuilder.group(
    {
      primeirosTresDigitos: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
      segundosTresDigitos: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
      terceirosTresDigitos: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
      ultimosDoisDigitos: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(2)]],
    }
  );

  stateChanges = new Subject<void>();
  focused = false;
  touched = false;
  controlType = 'cpf-input';
  id = `cpf-input-${CpfInputComponent.nextId++}`;
  onChange = (_: any) => {};
  onTouched = () => {};

  get empty() {
    const {
      value: { primeirosTresDigitos, segundosTresDigitos, terceirosTresDigitos, ultimosDoisDigitos }
    } = this.parts;

    return !primeirosTresDigitos && !segundosTresDigitos && !terceirosTresDigitos && !ultimosDoisDigitos;
  }

  get shouldLabelFloat() {
    return this.focused || !this.empty;
  }

  @Input('aria-describedby') userAriaDescribedBy!: string;

  @Input()
  get placeholder(): string {
    return this._placeholder;
  }
  set placeholder(value: string) {
    this._placeholder = value;
    this.stateChanges.next();
  }
  private _placeholder!: string;

  @Input()
  get required(): boolean {
    return this._required;
  }
  set required(value: BooleanInput) {
    this._required = coerceBooleanProperty(value);
    this.stateChanges.next();
  }
  private _required = false;

  @Input()
  get disabled(): boolean {
    return this._disabled;
  }
  set disabled(value: BooleanInput) {
    this._disabled = coerceBooleanProperty(value);
    this._disabled ? this.parts.disable() : this.parts.enable();
    this.stateChanges.next();
  }
  private _disabled = false;

  @Input()
  get value(): Cpf | null {
    if (this.parts.valid) {
      const {
        value: { primeirosTresDigitos, segundosTresDigitos, terceirosTresDigitos, ultimosDoisDigitos }
      } = this.parts;

      return new Cpf(primeirosTresDigitos!, segundosTresDigitos!, terceirosTresDigitos!, ultimosDoisDigitos!);
    }

    return null;
  }
  set value(cpf: Cpf | null) {
    const { primeirosTresDigitos, segundosTresDigitos, terceirosTresDigitos, ultimosDoisDigitos } = cpf || new Cpf('', '', '', '');
    this.parts.setValue({ primeirosTresDigitos, segundosTresDigitos, terceirosTresDigitos, ultimosDoisDigitos });
    this.stateChanges.next();
  }

  get errorState(): boolean {
    return this.parts.invalid && this.touched;
  }

  constructor(
    private _formBuilder: FormBuilder,
    private _focusMonitor: FocusMonitor,
    private _elementRef: ElementRef<HTMLElement>,
    @Optional() @Inject(MAT_FORM_FIELD) public _formField = MatFormField,
    @Optional() @Self() public ngControl: NgControl,
  ) {
    if (this.ngControl != null) {
      this.ngControl.valueAccessor = this;
    }
  }

  ngOnDestroy() {
    this.stateChanges.complete();
    this._focusMonitor.stopMonitoring(this._elementRef);
  }

  onFocusIn(event: FocusEvent) {
    if (!this.focused) {
      this.focused = true;
      this.stateChanges.next();
    }
  }

  onFocusOut(event: FocusEvent) {
    if (!this._elementRef.nativeElement.contains(event.relatedTarget as Element)) {
      this.touched = true;
      this.focused = false;
      this.onTouched();
      this.stateChanges.next();
    }
  }

  autoFocusNext(control: AbstractControl, nextElement?: HTMLInputElement): void {
    if (!control.errors && nextElement) {
      this._focusMonitor.focusVia(nextElement, 'program');
    }
  }

  autoFocusPrev(control: AbstractControl, prevElement: HTMLInputElement): void {
    if (control.value.length < 1) {
      this._focusMonitor.focusVia(prevElement, 'program');
    }
  }

  setDescribedByIds(ids: string[]) {
    const controlElement = this._elementRef.nativeElement.querySelector(
      '.cpf-input-container',
    )!;
    controlElement.setAttribute('aria-describedby', ids.join(' '));
  }

  onContainerClick() {
    // if (this.parts.controls.ultimosDoisDigitos.valid) {
    //   this._focusMonitor.focusVia(this.ultimosDoisDigitosInput, 'program');
    // } else if (this.parts.controls.terceirosTresDigitos.valid) {
    //   this._focusMonitor.focusVia(this.ultimosDoisDigitosInput, 'program');
    // } else if (this.parts.controls.segundosTresDigitos.valid) {
    //   this._focusMonitor.focusVia(this.terceirosTresDigitosInput, 'program');
    // } else if (this.parts.controls.primeirosTresDigitos.valid) {
    //   this._focusMonitor.focusVia(this.segundosTresDigitosInput, 'program');
    // } else {
    //   this._focusMonitor.focusVia(this.primeirosTresDigitosInput, 'program');
    // }
  }

  writeValue(cpf: Cpf | null): void {
    this.value = cpf;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  _handleInput(control: AbstractControl, nextElement?: HTMLInputElement): void {
    this.autoFocusNext(control, nextElement);
    this.onChange(this.value);
  }
}
