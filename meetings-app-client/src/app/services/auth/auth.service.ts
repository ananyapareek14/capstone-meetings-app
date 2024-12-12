import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environment/environment';
import ILogin, { ILoginCredentials } from '../../models/IAuth';
import { IRegister } from '../../models/IRegister';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
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

  // getUserEmail(): string {
  //   return localStorage.getItem('email') || '';
  // }

  getUserEmail(): string {
    const auth = localStorage.getItem(this.AUTH_KEY);
    if (auth) {
      const parsedAuth = JSON.parse(auth);
      return parsedAuth.email || '';  
    }
    return '';
  }

  logout() {
    localStorage.removeItem(this.AUTH_KEY);
  }
}
