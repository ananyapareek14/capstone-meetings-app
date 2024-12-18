import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environment/environment';
import ILogin, { ILoginCredentials } from '../../models/IAuth';
import { IRegister } from '../../models/IRegister';
import { tap } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private userEmailSubject = new BehaviorSubject<string>(''); // Initial email is empty
  userEmail$ = this.userEmailSubject.asObservable();
  private readonly AUTH_KEY = 'auth'
  private apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}



  login(credentials: ILoginCredentials) {
    return this.http.post<ILogin>(`${this.apiUrl}/auth/login`, credentials, {
      headers: { 
        'Content-Type': 'application/json' 
      },
    }).pipe(
      tap((loginResponse) => {
        const email = loginResponse.email; // Ensure your response has an `email` field
        if (email) {
          this.setUserEmail(email); // Update the observable
        }
          localStorage.setItem(
            this.AUTH_KEY,
            JSON.stringify(loginResponse)
          );
      }),
    );
  }

  register(data: IRegister) {
    return this.http.post<void>(`${this.apiUrl}/auth/register`, data, {
      headers: { 'Content-Type': 'application/json' },
      observe: 'response',
    }).pipe(
      tap(response => {
        if (response.status === 201) {
          console.log('User registered successfully');
        }
      })
    )
  }

  getAuthToken(): string | null {
    const auth = localStorage.getItem(this.AUTH_KEY);
    if (auth) {
      const parsedAuth = JSON.parse(auth);
      return parsedAuth.token || null; // Adjust "token" to match your backend's key
    }
    return null;
  }

  setUserEmail(email: string): void {
    this.userEmailSubject.next(email);
    localStorage.setItem(this.AUTH_KEY, JSON.stringify({ email }));
  }

  // getUserEmail(): string {
  //   const auth = localStorage.getItem(this.AUTH_KEY);
  //   if (auth) {
  //     const parsedAuth = JSON.parse(auth);
  //     return parsedAuth.email || '';  
  //   }
  //   return '';
  // }

  getUserEmail(): string {
    const auth = localStorage.getItem(this.AUTH_KEY);
    if (auth) {
      const parsedAuth = JSON.parse(auth);
      const email = parsedAuth.email || '';
      this.userEmailSubject.next(email); // Update the observable with the current email
      return email;
    }
    return '';
  }

  isAuthenticated(): boolean {
    return !!this.getAuthToken(); // Returns true if a token exists, false otherwise
  }

  logout() {
    localStorage.removeItem(this.AUTH_KEY);
  }
}
