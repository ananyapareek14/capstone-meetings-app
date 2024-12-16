import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class TeamsService {
  private baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getTeams(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/teams`);
  }

  addTeams(team: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/teams`, team);
  }

  getAvailableEmails(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/users`);
  }

  addMemberToTeam(teamId: string, memberEmail: string): Observable<any> {
    const params = new HttpParams()
      .set('action', 'add_member')
      .set('email', memberEmail);

    return this.http.patch<any>(
      `${this.baseUrl}/teams/${teamId}`,
      {},
      { params }
    );
  }

  leaveTeam(_id: string): Observable<any> {
    return this.http.patch<any>(
      `${this.baseUrl}/teams/${_id}?action=remove_member`,
      {}
    );
  }
}
