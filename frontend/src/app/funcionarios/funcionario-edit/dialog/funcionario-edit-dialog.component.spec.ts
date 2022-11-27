import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FuncionarioEditDialogComponent } from './funcionario-edit-dialog.component';

describe('FuncionarioEditDialogComponent', () => {
  let component: FuncionarioEditDialogComponent;
  let fixture: ComponentFixture<FuncionarioEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FuncionarioEditDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FuncionarioEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
