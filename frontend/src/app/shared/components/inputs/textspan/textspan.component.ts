import { Component, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { InputConfig, InputControlConfig, InputViewConfig } from 'src/app/shared/model/form';

@Component({
  selector: 'app-input-textspan',
  templateUrl: './textspan.component.html',
  styleUrls: ['./textspan.component.css']
})
export class TextspanComponent {
  @Input() input!: InputConfig;
  @Input() form!: FormGroup;
}
