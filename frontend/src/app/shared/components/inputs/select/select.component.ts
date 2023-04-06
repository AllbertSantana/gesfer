import { Component, Input, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { InputConfig, InputControlConfig, InputViewConfig } from 'src/app/shared/model/form';

@Component({
  selector: 'app-input-select',
  templateUrl: './select.component.html',
  styleUrls: ['./select.component.css']
})
export class SelectComponent {
  @Input() input!: InputConfig;
  @Input() form!: FormGroup;
}
