import { Component, OnInit, OnDestroy } from '@angular/core';
import {
  Router,
  NavigationEnd,
  RouterLink,
  RouterLinkActive,
} from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit, OnDestroy {
  showNavbar = true;
  userEmail: string = '';
  private subscription: Subscription = new Subscription();

  constructor(private router: Router, private authService: AuthService) {
    this.showNavbar = this.authService.isAuthenticated() && !['/login', '/register'].includes(this.router.url);
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.showNavbar = this.authService.isAuthenticated() && !['/login', '/register'].includes(event.url);
      }
    });
  }
  

  ngOnInit() {
    // this.userEmail = this.authService.getUserEmail();
    // console.log('User Email:', this.userEmail);
    this.subscription = this.authService.userEmail$.subscribe(email => {
      this.userEmail = email;
      console.log('Updated User Email:', this.userEmail);
    });

    this.authService.getUserEmail();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnDestroy() {
    // Clean up the subscription when the component is destroyed
    this.subscription.unsubscribe();
  }

}

