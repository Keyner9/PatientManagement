import { ChangeDetectionStrategy, Component } from '@angular/core';
import { LayoutComponent } from './core/layout/layout.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LayoutComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
}
