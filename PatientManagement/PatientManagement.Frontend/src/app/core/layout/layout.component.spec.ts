import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { MessageService } from 'primeng/api';
import { LayoutComponent } from './layout.component';

describe('LayoutComponent', () => {
  let fixture: ComponentFixture<LayoutComponent>;
  let component: LayoutComponent;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LayoutComponent],
      providers: [
        provideRouter([]),
        provideNoopAnimations(),
        MessageService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LayoutComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  it('should render the toolbar', () => {
    fixture.detectChanges();
    const toolbar = fixture.nativeElement.querySelector('p-toolbar');
    expect(toolbar).not.toBeNull();
  });

  it('should render the app title', () => {
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Gestion de Pacientes');
  });

  it('should render the patients navigation button', () => {
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button[aria-label="Ir a pacientes"]');
    expect(button).not.toBeNull();
    expect(button.textContent).toContain('Pacientes');
  });

  it('should render the toast component', () => {
    fixture.detectChanges();
    const toast = fixture.nativeElement.querySelector('p-toast');
    expect(toast).not.toBeNull();
  });

  it('should render the router outlet', () => {
    fixture.detectChanges();
    const outlet = fixture.nativeElement.querySelector('router-outlet');
    expect(outlet).not.toBeNull();
  });
});
