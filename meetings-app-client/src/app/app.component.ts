import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { CalendarComponent } from './calender/calender.component';
import { NavbarComponent } from './common/navbar/navbar.component';
import { MeetingsComponent } from './meetings/meetings.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    LoginComponent,
    CalendarComponent,
    NavbarComponent,
    MeetingsComponent,
    FormsModule,
    CommonModule,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'meetings-app';
}
