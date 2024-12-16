import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class MeetingsService {
  private baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getMeetings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/meetings`);
  }

  addMeeting(meeting: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/meetings`, meeting);
  }

  getAvailableEmails(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/users`);
  }

  addAttendee(meetingId: string, attendeeEmail: string): Observable<any> {
    const params = new HttpParams()
        .set('action', 'add_attendee')
        .set('email', attendeeEmail);

    return this.http.patch<any>(`${this.baseUrl}/meetings/${meetingId}`, {}, { params });
  }

  leaveMeeting(_id: string): Observable<any> {
    return this.http.patch<any>(`${this.baseUrl}/meetings/${_id}?action=remove_attendee`,{});
  }
}
