import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CargaLiquidosComponent } from './carga-liquidos.component';

describe('CargaLiquidosComponent', () => {
  let component: CargaLiquidosComponent;
  let fixture: ComponentFixture<CargaLiquidosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CargaLiquidosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CargaLiquidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
