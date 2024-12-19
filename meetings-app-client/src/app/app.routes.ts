import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { CalendarComponent } from './calender/calender.component';
import { MeetingsComponent } from './meetings/meetings.component';
import { TeamsComponent } from './teams/teams.component';
import { RegisterComponent } from './register/register.component';
import { AddmeetingsComponent } from './addmeetings/addmeetings.component';
import { AuthGuard } from './services/auth/auth.guard';

export const routes: Routes = [
  {
    path: 'meetings',
    component: MeetingsComponent,
    canActivate: [AuthGuard], // Protect meetings route
    children: [
      { path: '', redirectTo: 'search', pathMatch: 'full' },
      {
        path: 'add',
        component: AddmeetingsComponent,
        canActivate: [AuthGuard], // Protect add meetings route
      },
    ],
  },
  {
    path: 'calendar',
    component: CalendarComponent,
    canActivate: [AuthGuard], // Protect calendar route
  },
  {
    path: 'teams',
    component: TeamsComponent,
    canActivate: [AuthGuard], // Protect teams route
  },
  {
    path: 'login',
    component: LoginComponent, // Login is public
  },
  {
    path: 'register',
    component: RegisterComponent, // Register is public
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full', // Redirect to login by default
  },
];
