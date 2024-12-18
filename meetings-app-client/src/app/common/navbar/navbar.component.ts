import { Component, OnInit } from '@angular/core';
import {
  Router,
  NavigationEnd,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit {
  showNavbar = true;
  userEmail: string = '';

  constructor(private router: Router, private authService: AuthService) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.showNavbar = event.url !== '/login';
        // this.showNavbar = event.url !== '/register';
      }
    });
  }

  ngOnInit() {
    this.userEmail = this.authService.getUserEmail();
    console.log('User Email:', this.userEmail);
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}

