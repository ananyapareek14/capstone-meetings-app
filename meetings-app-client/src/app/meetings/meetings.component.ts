// import { Component, OnInit } from '@angular/core';
// import { MeetingsService } from '../services/meetings/meetings.service';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';
// import { AddmeetingsComponent } from '../addmeetings/addmeetings.component';
// import { ActivatedRoute, RouterLink, Router, RouterOutlet, NavigationEnd } from '@angular/router';

// @Component({
//   selector: 'app-meetings',
//   standalone: true,
//   imports: [FormsModule, CommonModule, AddmeetingsComponent, RouterLink, RouterOutlet],
//   templateUrl: './meetings.component.html',
//   styleUrls: ['./meetings.component.scss'],
// })
// export class MeetingsComponent implements OnInit {
//   activeTab: string = 'search'; // Default to Search tab

//   // Search/Filter Meetings
//   selectedFilter: string = 'ALL'; // Default filter
//   searchTerm: string = ''; // Search input
//   meetings: any[] = []; // All meetings data fetched from backend
//   filteredMeetings: any[] = []; // Meetings filtered by search/filter

//   // Add Meeting
//   meeting = {
//     date: '',
//     startTime: '',
//     endTime: '',
//     description: '',
//     attendees: '',
//   };

//   availableEmails: string[] = []; // List of all available email IDs for adding attendees
//   newAttendee: string = ''; // Selected attendee to add to a meeting

//   constructor(private meetingsService: MeetingsService, private route: ActivatedRoute, private router: Router) {}

//   ngOnInit() {
//     this.router.events.subscribe((event) => {
//       if (event instanceof NavigationEnd) {
//         if (event.url === '/meetings') {
//           this.activeTab = 'search'; // Reset the active tab
//           this.loadMeetings(); // Reload meetings data
//         }
//       }
//     });

//     this.route.firstChild?.url.subscribe((urlSegments) => {
//       if (urlSegments.length > 0) {
//         const tab = urlSegments[0].path;
//         this.activeTab = tab === 'add' ? 'add' : 'search';
//       }
//     });

//     this.fetchAvailableEmails();
//     this.loadMeetings();
//   }

//   loadMeetings() {
//     let period: 'all' | 'past' | 'present' | 'future';

//     switch (this.selectedFilter) {
//       case 'ALL':
//         period = 'all';
//         break;
//       case 'PAST':
//         period = 'past';
//         break;
//       case 'TODAY':
//         period = 'present';
//         break;
//       case 'UPCOMING':
//         period = 'future';
//         break;
//       default:
//         period = 'all'; // Fallback to 'all' if filter is not recognized
//     }

//     this.meetingsService.getMeetings('', period).subscribe(
//       (data) => {
//         this.meetings = data.map((meeting) => ({
//           ...meeting,
//           newAttendee: '', // Initialize newAttendee field for each meeting
//         }));
//         this.applySearchAndFilter();
//         console.log(this.meetings);
//       },
//       (error) => {
//         console.error('Error fetching meetings:', error);
//       }
//     );
//   }  

//   applySearchAndFilter() {
//     const term = this.searchTerm.toLowerCase();
//     this.filteredMeetings = this.meetings.filter((meeting) => {
//       const matchesSearch = term
//         ? meeting.description.toLowerCase().includes(term) ||
//           meeting.attendees.some((attendee: string) => attendee.toLowerCase().includes(term))
//         : true;

//       const matchesFilter =
//         this.selectedFilter === 'ALL' ||
//         (this.selectedFilter === 'PAST' && this.isPastMeeting(meeting)) ||
//         (this.selectedFilter === 'TODAY' && this.isTodayMeeting(meeting)) ||
//         (this.selectedFilter === 'UPCOMING' && this.isUpcomingMeeting(meeting));

//       return matchesSearch && matchesFilter;
//     });
//   }

//   // Helper to check if meeting is past
//   isPastMeeting(meeting: any) {
//     const currentDate = new Date();
//     const meetingDate = new Date(meeting.date);
//     return meetingDate < currentDate;
//   }

//   // Helper to check if meeting is today
//   isTodayMeeting(meeting: any) {
//     const currentDate = new Date();
//     const meetingDate = new Date(meeting.date);
//     return meetingDate.toDateString() === currentDate.toDateString();
//   }

//   // Helper to check if meeting is upcoming
//   isUpcomingMeeting(meeting: any) {
//     const currentDate = new Date();
//     const meetingDate = new Date(meeting.date);
//     return meetingDate > currentDate;
//   }

//   // Switch between tabs
//   showTab(tab: string) {
//     this.activeTab = tab;
//     if (tab === 'add') {
//       this.router.navigate(['/meetings/add']);
//     } else {
//       this.router.navigate(['/meetings']);
//     }
//   }

//   fetchAvailableEmails() {
//     this.meetingsService.getAvailableEmails().subscribe(
//       (emails: string[]) => {
//         this.availableEmails = emails;
//         console.log(emails);
//       },
//       (error) => {
//         console.error('Error fetching available emails:', error);
//       }
//     );
//   }

//   addAttendee(meetingId: string, attendeeEmail: string) {
//     if (!attendeeEmail) return;

//     this.meetingsService.addAttendee(meetingId, attendeeEmail).subscribe(
//       () => {
//         alert('Attendee added successfully!');
//         this.loadMeetings(); // Refresh meetings list
//       },
//       (error) => {
//         console.error('Error adding attendee:', error);
//         alert('Failed to add attendee. Please try again.');
//       }
//     );
//   }

//   leaveMeeting(meetingId: string) {
//     this.meetingsService.leaveMeeting(meetingId).subscribe(
//       () => {
//         alert('You have left the meeting.');
//         this.loadMeetings(); // Refresh meetings list
//       },
//       (error) => {
//         console.error('Error leaving meeting:', error);
//         alert('Failed to leave the meeting. Please try again.');
//       }
//     );
//   }

//   searchMeetings() {
//     this.applySearchAndFilter(); // Apply search and filter to the meetings list
//     console.log('Search executed with terms:', this.searchTerm);
//   }

//   filterMeetings() {
//     this.applySearchAndFilter(); // Apply filter and search after selection
//     console.log('Filter applied:', this.selectedFilter);
//   }
// }


import { Component, OnInit, OnDestroy } from '@angular/core';
import { MeetingsService } from '../services/meetings/meetings.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AddmeetingsComponent } from '../addmeetings/addmeetings.component';
import { ActivatedRoute, RouterLink, Router, RouterOutlet, NavigationEnd } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-meetings',
  standalone: true,
  imports: [FormsModule, CommonModule, AddmeetingsComponent, RouterLink, RouterOutlet],
  templateUrl: './meetings.component.html',
  styleUrls: ['./meetings.component.scss'],
})
export class MeetingsComponent implements OnInit, OnDestroy {
  activeTab: string = 'search'; // Default to Search tab

  // Search/Filter Meetings
  selectedFilter: string = 'ALL'; // Default filter
  searchTerm: string = ''; // Search input
  meetings: any[] = []; // All meetings data fetched from backend
  filteredMeetings: any[] = []; // Meetings filtered by search/filter

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

  // For debouncing search
  private searchTermSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private meetingsService: MeetingsService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    // Handle router events to reset the active tab
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        if (event.url === '/meetings') {
          this.activeTab = 'search';
          this.loadMeetings();
        }
      }
    });

    // Set the active tab based on the route
    this.route.firstChild?.url.subscribe((urlSegments) => {
      if (urlSegments.length > 0) {
        const tab = urlSegments[0].path;
        this.activeTab = tab === 'add' ? 'add' : 'search';
      }
    });

    this.fetchAvailableEmails();
    this.loadMeetings();

    // Listen for changes to searchTermSubject with debounce
    this.searchTermSubject
      .pipe(debounceTime(100), takeUntil(this.destroy$))
      .subscribe(() => {
        this.applySearchAndFilter();
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMeetings() {
    let period: 'all' | 'past' | 'present' | 'future';

    switch (this.selectedFilter) {
      case 'ALL':
        period = 'all';
        break;
      case 'PAST':
        period = 'past';
        break;
      case 'TODAY':
        period = 'present';
        break;
      case 'UPCOMING':
        period = 'future';
        break;
      default:
        period = 'all';
    }

    this.meetingsService.getMeetings('', period).subscribe(
      (data) => {
        this.meetings = data.map((meeting) => ({
          ...meeting,
          newAttendee: '',
        }));
        this.applySearchAndFilter();
        console.log(this.meetings);
      },
      (error) => {
        console.error('Error fetching meetings:', error);
      }
    );
  }

  applySearchAndFilter() {
    const term = this.searchTerm.toLowerCase();
    this.filteredMeetings = this.meetings.filter((meeting) => {
      const matchesSearch = term
        ? meeting.description.toLowerCase().includes(term) ||
          meeting.attendees.some((attendee: string) => attendee.toLowerCase().includes(term))
        : true;

      const matchesFilter =
        this.selectedFilter === 'ALL' ||
        (this.selectedFilter === 'PAST' && this.isPastMeeting(meeting)) ||
        (this.selectedFilter === 'TODAY' && this.isTodayMeeting(meeting)) ||
        (this.selectedFilter === 'UPCOMING' && this.isUpcomingMeeting(meeting));

      return matchesSearch && matchesFilter;
    });
  }

  // Helper to check if meeting is past
  isPastMeeting(meeting: any) {
    const currentDate = new Date();
    const meetingDate = new Date(meeting.date);
    return meetingDate < currentDate;
  }

  // Helper to check if meeting is today
  isTodayMeeting(meeting: any) {
    const currentDate = new Date();
    const meetingDate = new Date(meeting.date);
    return meetingDate.toDateString() === currentDate.toDateString();
  }

  // Helper to check if meeting is upcoming
  isUpcomingMeeting(meeting: any) {
    const currentDate = new Date();
    const meetingDate = new Date(meeting.date);
    return meetingDate > currentDate;
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
        this.availableEmails = emails;
        console.log(emails);
      },
      (error) => {
        console.error('Error fetching available emails:', error);
      }
    );
  }

  addAttendee(meetingId: string, attendeeEmail: string) {
    if (!attendeeEmail) return;

    this.meetingsService.addAttendee(meetingId, attendeeEmail).subscribe(
      () => {
        alert('Attendee added successfully!');
        this.loadMeetings();
      },
      (error) => {
        console.error('Error adding attendee:', error);
        alert('Failed to add attendee. Please try again.');
      }
    );
  }

  leaveMeeting(meetingId: string) {
    this.meetingsService.leaveMeeting(meetingId).subscribe(
      () => {
        alert('You have left the meeting.');
        this.loadMeetings();
      },
      (error) => {
        console.error('Error leaving meeting:', error);
        alert('Failed to leave the meeting. Please try again.');
      }
    );
  }

  searchMeetings() {
    this.searchTermSubject.next(this.searchTerm);
  }

  filterMeetings() {
    this.applySearchAndFilter();
    console.log('Filter applied:', this.selectedFilter);
  }
}
