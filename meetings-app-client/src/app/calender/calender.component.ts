import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarService } from '../services/calendar/calendar.service';
import IMeeting from '../models/IMeeting';

@Component({
  selector: 'app-calender',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './calender.component.html',
  styleUrl: './calender.component.scss',
})
export class CalendarComponent implements OnInit {
  selectedDate: Date = new Date();
  meetings: IMeeting[] = [];
  hours = Array.from({ length: 24 }, (_, i) => ({
    label: `${i}`,
    value: i,
  }));

  constructor(private calendarService: CalendarService) {}

  onDateChange(event: any): void {
    const inputValue = event.target.value;
    const [year, month, day] = inputValue.split('-').map(Number);

    if (day && month && year) {
      const newDate = new Date(year, month - 1, day);
      if (!isNaN(newDate.getTime())) {
        this.selectedDate = newDate;
        this.loadMeetingsForDate(this.selectedDate);
      }
    }
  }

  formatDate(date: Date): string {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${year}-${month}-${day}`;
  }

  loadMeetingsForDate(date: Date | null): void {
    if (!date) {
      this.meetings = [];
      return;
    }
    const formattedDate = this.formatDate(date);

    // Fetch meetings using AuthService
    this.calendarService.getMeetingsByDate(formattedDate).subscribe(
      (data) => {
        console.log(data);
        this.meetings = data.map((meeting) => ({
          ...meeting,
          attendees: meeting.attendees.map((attendee: any) => attendee.email),
        }));
      },
      (error) => {
        console.error('Error fetching meetings:', error);
        alert('Failed to load meetings. Please try again later.');
      }
    );
  }

  hasMeetingAt(hour: number): boolean {
    return this.meetings.some(
      (meeting) =>
        meeting.startTime.hours === hour ||
        (meeting.startTime.hours < hour && meeting.endTime.hours >= hour)
    );
  }

  getMeetingsAt(hour: number): IMeeting[] {
    return this.meetings.filter(
      (meeting) =>
        // The meeting starts within this hour
        meeting.startTime.hours === hour ||
        // Or the meeting spans into this hour but doesn't start or end elsewhere
        (meeting.startTime.hours < hour && meeting.endTime.hours > hour) ||
        // Or the meeting spans into this hour and ends in this hour
        (meeting.startTime.hours < hour &&
          meeting.endTime.hours === hour &&
          meeting.endTime.minutes > 0)
    );
  }

  // getMeetingsAt(hour: number): IMeeting[] {
  //   return this.meetings.filter(
  //     (meeting) =>
  //       // The meeting starts within this hour
  //       (meeting.startTime.hours === hour && meeting.startTime.minutes > 0) ||
  //       // The meeting spans into this hour (start before this hour and ends after this hour)
  //       (meeting.startTime.hours < hour && meeting.endTime.hours > hour) ||
  //       // The meeting ends within this hour (it starts before the hour and ends at the hour or before)
  //       (meeting.startTime.hours < hour &&
  //         meeting.endTime.hours === hour &&
  //         meeting.endTime.minutes > 0) ||
  //       // The meeting ends exactly at the hour boundary (e.g., 8:30 to 9:30)
  //       (meeting.startTime.hours === hour &&
  //         meeting.endTime.hours === hour &&
  //         meeting.startTime.minutes < meeting.endTime.minutes)
  //   );
  // }

  // getMeetingsAt(hour: number): IMeeting[] {
  //   return this.meetings.filter(
  //     (meeting) =>
  //       // The meeting starts within this hour
  //       (meeting.startTime.hours === hour && meeting.startTime.minutes > 0) ||
  //       // The meeting spans into this hour (starts before and ends after this hour)
  //       (meeting.startTime.hours < hour && meeting.endTime.hours > hour) ||
  //       // The meeting ends within this hour (it starts before this hour and ends at or before this hour)
  //       (meeting.startTime.hours < hour &&
  //         meeting.endTime.hours === hour &&
  //         meeting.endTime.minutes > 0) ||
  //       // The meeting is exactly one hour long (e.g., 11:00 to 12:00)
  //       (meeting.startTime.hours === hour &&
  //         meeting.endTime.hours === hour &&
  //         meeting.startTime.minutes === 0 &&
  //         meeting.endTime.minutes === 0) ||
  //       // The meeting is a full-hour meeting, like 11:00–12:00
  //       (meeting.startTime.hours === hour &&
  //         meeting.endTime.hours === hour &&
  //         meeting.startTime.minutes === 0 &&
  //         meeting.endTime.minutes === 0)
  //   );
  // }

  getMeetingStyles(meeting: IMeeting): any {
    const startHour = meeting.startTime.hours;
    const endHour = meeting.endTime.hours;
    const startMinutes = meeting.startTime.minutes || 0;
    const endMinutes = meeting.endTime.minutes || 0;

    const topOffset = (startMinutes / 60) * 100; // Percentage from the top
    console.log(topOffset);
    const durationMinutes =
      (endHour - startHour) * 60 + (endMinutes - startMinutes);
    const height = Math.max(durationMinutes, 20); // Ensure minimum height for visibility

    const inlineStyle =
      height < 40
        ? {
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }
        : {};

    return {
      position: 'absolute',
      top: `${topOffset}%`,
      height: `${height}px`,
      ...inlineStyle,
    };
  }

  ngOnInit(): void {
    this.loadMeetingsForDate(this.selectedDate);
  }
}