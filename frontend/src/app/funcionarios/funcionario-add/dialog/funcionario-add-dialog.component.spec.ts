import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FuncionarioAddDialogComponent } from './funcionario-add-dialog.component';

describe('FuncionarioAddDialogComponent', () => {
  let component: FuncionarioAddDialogComponent;
  let fixture: ComponentFixture<FuncionarioAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FuncionarioAddDialogComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FuncionarioAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
