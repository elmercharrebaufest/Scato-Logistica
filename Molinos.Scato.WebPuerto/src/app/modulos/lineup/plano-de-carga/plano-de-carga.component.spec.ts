import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlanoDeCargaComponent } from './plano-de-carga.component';

describe('PlanoDeCargaComponent', () => {
  let component: PlanoDeCargaComponent;
  let fixture: ComponentFixture<PlanoDeCargaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlanoDeCargaComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanoDeCargaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
