import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { InputConfig, InputControlConfig } from '../../model/form';
import { FormService } from '../../services/form/form.service';
import { FormGroup } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { Filter } from '../../model/list-property';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.css'],
})
export class FilterComponent implements OnInit, OnDestroy {
  private destroyed$ = new Subject<void>;
  private filtersControls: InputControlConfig[] = this.filterData.map(config => config.control);
  private filtersDisableControls: InputControlConfig[] = this.filterData.map(
    config => {
      return {
        name: `for${config.control.name}`,
        value: !config.control.isDisabled,
        usedAt: config.control.usedAt,
        isDisabled: false,
      };
    }
  );
  public filterControl!: FormGroup;
  public filter!: FormGroup;

  constructor(
    private formService: FormService,
    public dialogRef: MatDialogRef<FilterComponent>,
    @Inject(MAT_DIALOG_DATA) public filterData: InputConfig[]
  ) {}

  ngOnInit(): void {
    this.filterControl = this.formService.toFormGroup(this.filtersDisableControls);
    this.filter = this.formService.toFormGroup(this.filtersControls);

    this.filterControl.valueChanges
      .pipe(takeUntil(this.destroyed$))
      .subscribe(
        (filterControl) => {
          this.filtersControls.forEach(
            control => {
              if (filterControl[`for${control.name}`]) {
                this.filter.get(control.name)!.enable();
              } else {
                this.filter.get(control.name)!.reset();
                this.filter.get(control.name)!.disable();
              }
            }
          );
        }
      )
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  onSubmit(): void {
    let filterValues: Filter[] = this.filtersControls.map(control => {
      return {
        name: control.name,
        value: this.filter.get(control.name)!.disabled ? '' : this.filter.get(control.name)!.value,
      }
    });

    this.dialogRef.close(filterValues);
  }
}
