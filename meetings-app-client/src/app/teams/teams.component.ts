import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TeamsService } from '../services/teams/teams.service';

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teams.component.html',
  styleUrl: './teams.component.scss',
})
export class TeamsComponent implements OnInit {

  teams: any[] = [];
  users: string[] = [];
  selectedMember: string = '';
  showAddTeamForm: boolean = false;
  newTeam = { name: '', shortName: '', description: '' };

  constructor(private teamsService: TeamsService) {}

  ngOnInit(): void {
    this.fetchTeams();
    this.fetchAvailableEmails();
  }

  fetchTeams(): void {
    this.teamsService.getTeams().subscribe({
      next: (data) => {
        this.teams = data;
      },
      error: (err) => console.error('Failed to fetch teams', err),
    });
  }

  fetchAvailableEmails(): void {
    this.teamsService.getAvailableEmails().subscribe({
      next: (data) => {
        this.users = data.map((user: any) => user.email);
        console.log(data);
      },
      error: (err) => console.error('Failed to fetch available emails', err),
    });
  }

  addMember(team: any): void {
    console.log('Team ID:', team._id);
    if (this.selectedMember && !team.members.includes(this.selectedMember)) {
      this.teamsService
        .addMemberToTeam(team._id, this.selectedMember)
        .subscribe({
          next: () => {
            team.members.push(this.selectedMember);
            this.selectedMember = ''; // Clear selection
          },
          error: (err) => console.error('Failed to add member', err),
        });
    }
  }

  excuseYourself(team: any): void {
    this.teamsService.leaveTeam(team._id).subscribe({
      next: () => {
        team.members = team.members.filter(
          (member: string) => member !== this.selectedMember
        );
        console.log(`Excused yourself from team: ${team.name}`);
      },
      error: (err) => console.error('Failed to leave team', err),
    });
  }

  createTeam(): void {
    if (
      this.newTeam.name &&
      this.newTeam.shortName &&
      this.newTeam.description
    ) {
      this.teamsService.addTeams(this.newTeam).subscribe({
        next: (createdTeam) => {
          this.teams.push(createdTeam);
          this.newTeam = { name: '', shortName: '', description: '' }; // Clear form
          this.showAddTeamForm = false; // Hide form
        },
        error: (err) => console.error('Failed to create team', err),
      });
    }
  }

  toggleAddTeamForm(): void {
    this.showAddTeamForm = !this.showAddTeamForm;
  }
}
