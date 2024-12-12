interface IMeeting {
  name: string;
  attendees: { email: string }[];
  startTime: { hours: number; minutes: number };
  endTime: { hours: number; minutes: number };
}

export type { IMeeting as default }
