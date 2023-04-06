import { Injectable } from '@angular/core';
import { FormType, InputControlConfig } from '../../model/form';
import { FormControl, FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class FormService {
  constructor() { }

  toFormGroup(controls: InputControlConfig[]): FormGroup<any> {
    const group: any = {};

    controls.forEach(
        control => {
          group[control.name] = control.validators
          ? new FormControl({value: control.value, disabled: control.isDisabled}, {nonNullable: true, validators: control.validators.map(v => v.validator)})
          : new FormControl({value: control.value, disabled: control.isDisabled}, {nonNullable: true});
        }
    );

    return new FormGroup(group);
  }
}