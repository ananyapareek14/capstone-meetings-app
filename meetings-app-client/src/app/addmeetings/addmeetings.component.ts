import { Component } from '@angular/core';
import { MeetingsService } from '../services/meetings/meetings.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-addmeetings',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './addmeetings.component.html',
  styleUrl: './addmeetings.component.scss'
})
export class AddmeetingsComponent {

  meetings: any[] = [];

  constructor(private meetingsService: MeetingsService) {}

  meeting = {
    name:'',
    date: '',
    startTime: '',
    endTime: '',
    description: '',
    attendees: '',
  };

  private resetMeetingForm() {
    this.meeting = {
      name:'',
      date: '',
      startTime: '',
      endTime: '',
      description: '',
      attendees: '',
    };
  }

  private validateMeetingInputs(): boolean {
    return (
      !!this.meeting.date && // Ensure the date is non-empty
      !!this.meeting.startTime && // Ensure the start time is non-empty
      !!this.meeting.endTime && // Ensure the end time is non-empty
      !!this.meeting.description && // Ensure the description is non-empty
      !!this.meeting.attendees // Ensure attendees field is non-empty
    );
  }

  addMeeting() {
    if (!this.validateMeetingInputs()) {
      alert('Please fill out all fields correctly.');
      return;
    }
  
    console.log('Meeting to be added:', this.meeting);
    
    const attendeesList = this.meeting.attendees.split(',').map((a) => a.trim());
    
    const [startHours, startMinutes] = this.meeting.startTime.split(':').map(Number);
    
    const [endHours, endMinutes] = this.meeting.endTime.split(':').map(Number);

    const meetingData = {
      name: this.meeting.name,
      description: this.meeting.description,
      date: this.meeting.date,
      startTime: { hours: startHours, minutes: startMinutes },
      endTime: { hours: endHours, minutes: endMinutes },
      attendees: attendeesList
    };

    this.meetingsService.addMeeting(meetingData).subscribe(
      (response) => {
        alert('Meeting added successfully!');
        this.resetMeetingForm();
        this.loadMeetings();
      },
      (error) => {
        console.error('Error adding meeting:', error);
        alert('Failed to add meeting. Please try again.');
      }
    );
  }
  
  
  loadMeetings() {
    this.meetingsService.getMeetings().subscribe(
      (data) => {
        this.meetings = data;
        console.log(this.meetings);
        // this.filterMeetings();
      },
      (error) => {
        console.error('Error fetching meetings:', error);
      }
    );
  }
}
