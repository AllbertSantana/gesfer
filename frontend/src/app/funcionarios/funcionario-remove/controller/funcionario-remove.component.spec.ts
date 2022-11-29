import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FuncionarioRemoveComponent } from './funcionario-remove.component';

describe('FuncionarioRemoveComponent', () => {
  let component: FuncionarioRemoveComponent;
  let fixture: ComponentFixture<FuncionarioRemoveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FuncionarioRemoveComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FuncionarioRemoveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
