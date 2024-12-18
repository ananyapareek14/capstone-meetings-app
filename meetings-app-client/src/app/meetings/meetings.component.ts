import { Component, OnInit } from '@angular/core';
import { MeetingsService } from '../services/meetings/meetings.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AddmeetingsComponent } from '../addmeetings/addmeetings.component';
import { ActivatedRoute, RouterLink, Router, RouterOutlet, NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-meetings',
  standalone: true,
  imports: [FormsModule,CommonModule, AddmeetingsComponent, RouterLink, RouterOutlet],
  templateUrl: './meetings.component.html',
  styleUrl: './meetings.component.scss',
})
export class MeetingsComponent implements OnInit {

  activeTab: string = 'search'; // Default to Search tab

  // Search/Filter Meetings
  selectedFilter: string = 'TODAY'; // Default filter
  searchTerm: string = ''; // Search input
  meetings: any[] = []; // All meetings data fetched from backend
  filteredMeetings: any[] = []; // Filtered results based on user input

  // Add Meeting
  meeting = {
    date: '',
    startTime: '',
    endTime: '',
    description: '',
    attendees: '',
  };

  availableEmails: string[] = []; // List of all available email IDs for adding attendees
  newAttendee: string = ''; // Selected attendee to add to a meeting

  constructor(private meetingsService: MeetingsService, private route:ActivatedRoute, private router: Router) {}

  ngOnInit() {

    // this.route.url.subscribe((url) => {
    //   // Check the current route segment to switch tabs
    //   if (url[1]?.path === 'add') {
    //     this.activeTab = 'add';
    //   } else {
    //     this.activeTab = 'search';
    //   }
    // });

    this.router.events.subscribe((event) => {
          if (event instanceof NavigationEnd) {
            this.loadMeetings();
            this.filterMeetings();
          }
        });

    this.route.firstChild?.url.subscribe((urlSegments) => {
      if (urlSegments.length > 0) {
        const tab = urlSegments[0].path; // Check if it's 'add' or 'search'
        this.activeTab = tab === 'add' ? 'add' : 'search'; // Set active tab
      }
    });

    this.fetchAvailableEmails();
    this.loadMeetings();
    this.filterMeetings();

  }


  loadMeetings() {
    this.meetingsService.getMeetings().subscribe(
      (data) => {
        // Add newAttendee field to each meeting
        this.meetings = data.map(meeting => ({
          ...meeting,
          newAttendee: '' // Initialize newAttendee field for each meeting
        }
        ));
        
        console.log(this.meetings);
        this.filterMeetings(); // Apply the filter after loading meetings
      },
      (error) => {
        console.error('Error fetching meetings:', error);
      }
    );
  }

  // loadMeetings() {
  //   this.meetingsService.getMeetings().subscribe(
  //     (data) => {
  //       this.meetings = data;
  
  //       this.meetings.forEach(meeting => {
  //         meeting.newAttendee = ''; 
  //       });
  
  //       console.log(this.meetings);
  //       this.filterMeetings(); 
  //     },
  //     (error) => {
  //       console.error('Error fetching meetings:', error);
  //     }
  //   );
  // }
  

  filterMeetings() {
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Remove time portion for accurate date-only comparison
  
    this.filteredMeetings = this.meetings.filter((meeting) => {
      const meetingDate = new Date(meeting.date); // Convert meeting date to Date object
      meetingDate.setHours(0, 0, 0, 0); // Remove time portion
  
      const matchesFilter =
        this.selectedFilter === 'ALL' ||
        (this.selectedFilter === 'PAST' && meetingDate < today) ||
        (this.selectedFilter === 'TODAY' &&
          meetingDate.getTime() === today.getTime()) ||
        (this.selectedFilter === 'UPCOMING' && meetingDate > today);
  
      const matchesSearch = this.searchTerm
        ? meeting.description
            .toLowerCase()
            .includes(this.searchTerm.toLowerCase())
        : true;
  
      return matchesFilter && matchesSearch;
    });
  }
  

  // Switch between tabs
  showTab(tab: string) {
    this.activeTab = tab;
    if (tab === 'add') {
      this.router.navigate(['/meetings/add']);
    } else {
      this.router.navigate(['/meetings']);
    }
  }

  fetchAvailableEmails() {
    this.meetingsService.getAvailableEmails().subscribe(
      (emails: string[]) => {
        this.availableEmails = emails.map((user: any) => user.email);
        console.log(emails);
      },
      (error) => {
        console.error('Error fetching available emails:', error);
      }
    );
  }

  // get attendeeEmails(): string {
  //   return this.meeting.attendees.map((a: any) => a.email).join(', ');
  // }

  addAttendee(meetingId: string, attendeeEmail: string) {
    if (!attendeeEmail) return;

    this.meetingsService.addAttendee(meetingId, attendeeEmail).subscribe(
      (response) => {
        alert('Attendee added successfully!');
        this.loadMeetings(); // Refresh meetings list
      },
      (error) => {
        console.error('Error adding attendee:', error);
        alert('Failed to add attendee. Please try again.');
      }
    );
  }

  leaveMeeting(meetingId: string) {
    this.meetingsService.leaveMeeting(meetingId).subscribe(
      (response) => {
        alert('You have left the meeting.');
        this.loadMeetings(); // Refresh meetings list
      },
      (error) => {
        console.error('Error leaving meeting:', error);
        alert('Failed to leave the meeting. Please try again.');
      }
    );
  }

  searchMeetings(): void {
    // Logic to filter meetings based on search terms
    this.filterMeetings(); // Call the existing filter logic
    console.log('Search executed with terms:', this.searchTerm);
  }

  // Validate meeting form inputs
  

  // Reset meeting form after submission
  
}
