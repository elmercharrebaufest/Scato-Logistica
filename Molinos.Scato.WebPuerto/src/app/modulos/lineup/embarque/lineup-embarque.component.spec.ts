import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LineupEmbarqueComponent } from './lineup-embarque.component';

describe('LineupEmbarqueComponent', () => {
  let component: LineupEmbarqueComponent;
  let fixture: ComponentFixture<LineupEmbarqueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LineupEmbarqueComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LineupEmbarqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
