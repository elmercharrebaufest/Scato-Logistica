import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CargaSolidosComponent } from './carga-solidos.component';

describe('CargaSolidosComponent', () => {
  let component: CargaSolidosComponent;
  let fixture: ComponentFixture<CargaSolidosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CargaSolidosComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CargaSolidosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
