import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LineupCalendarioComponent } from './lineup-calendario.component';

describe('LineupCalendarioComponent', () => {
  let component: LineupCalendarioComponent;
  let fixture: ComponentFixture<LineupCalendarioComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LineupCalendarioComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LineupCalendarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
