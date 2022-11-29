import { FocusMonitor } from '@angular/cdk/a11y';
import { BooleanInput, coerceBooleanProperty } from '@angular/cdk/coercion';
import { Component, ElementRef, Inject, Input, OnDestroy, Optional, Self, ViewChild } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormBuilder, NgControl, Validators } from '@angular/forms';
import { MatFormField, MatFormFieldControl, MAT_FORM_FIELD } from '@angular/material/form-field';
import { Subject } from 'rxjs';
import { Matricula } from 'src/app/shared/model/matricula';

@Component({
  selector: 'matricula-input',
  templateUrl: './matricula-input.component.html',
  styleUrls: ['./matricula-input.component.css'],
  providers: [{provide: MatFormFieldControl, useExisting: MatriculaInputComponent}],
  host: {
    '[class.floating]': 'shouldLabelFloat',
    '[id]': 'id',
  },
})
export class MatriculaInputComponent implements ControlValueAccessor, MatFormFieldControl<Matricula>, OnDestroy {
  static nextId = 0;
  @ViewChild('idDigitos') idDigitosInput!: HTMLInputElement;
  @ViewChild('vinculoDigitos') vinculoDigitosInput!: HTMLInputElement;

  parts = this._formBuilder.group(
    {
      idDigitos: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      vinculoDigitos: ['', [Validators.required, Validators.minLength(1), Validators.maxLength(2)]],
    }
  );

  stateChanges = new Subject<void>();
  focused = false;
  touched = false;
  controlType = 'cpf-input';
  id = `cpf-input-${MatriculaInputComponent.nextId++}`;
  onChange = (_: any) => {};
  onTouched = () => {};

  get empty() {
    const {
      value: { idDigitos, vinculoDigitos }
    } = this.parts;

    return !idDigitos && !vinculoDigitos;
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
  get value(): Matricula | null {
    if (this.parts.valid) {
      const {
        value: { idDigitos, vinculoDigitos }
      } = this.parts;

      return new Matricula(idDigitos!, vinculoDigitos!);
    }

    return null;
  }
  set value(matricula: Matricula | null) {
    const { idDigitos, vinculoDigitos } = matricula || new Matricula('', '');
    this.parts.setValue({ idDigitos, vinculoDigitos });
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
      '.matricula-input-container',
    )!;
    controlElement.setAttribute('aria-describedby', ids.join(' '));
  }

  onContainerClick() {
    // if (this.parts.controls.vinculoDigitos.valid) {
    //   this._focusMonitor.focusVia(this.vinculoDigitosInput, 'program');
    // } else if (this.parts.controls.idDigitos.valid) {
    //   this._focusMonitor.focusVia(this.vinculoDigitosInput, 'program');
    // } else {
    //   this._focusMonitor.focusVia(this.idDigitosInput, 'program');
    // }
  }

  writeValue(cpf: Matricula | null): void {
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
    if (this.parts.controls.idDigitos.value && this.parts.controls.idDigitos.value?.length > 7) {
      this.autoFocusNext(control, nextElement);
    }
    this.onChange(this.value);
  }
}
