import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FuncionarioRemoveDialogComponent } from './funcionario-remove-dialog.component';

describe('FuncionarioRemoveDialogComponent', () => {
  let component: FuncionarioRemoveDialogComponent;
  let fixture: ComponentFixture<FuncionarioRemoveDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FuncionarioRemoveDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FuncionarioRemoveDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
