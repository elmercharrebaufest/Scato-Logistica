import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlanoContentComponent } from './plano-content.component';

describe('PlanoContentComponent', () => {
  let component: PlanoContentComponent;
  let fixture: ComponentFixture<PlanoContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PlanoContentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PlanoContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
