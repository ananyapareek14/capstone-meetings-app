import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './teams.component.html',
  styleUrl: './teams.component.scss'
})
export class TeamsComponent {
  teams = [
    {
      name: 'Customer acquisition campaign',
      shortName: '@customer-acquisition',
      description: 'Team to come up with strategies to win over customers by end of FY21',
      members: ['john@example.com', 'jane@example.com']
    },
    {
      name: 'MERN stack training',
      shortName: '@mern-stack-training',
      description: 'Group attending MERN stack training for website revamp project',
      members: ['jane@example.com', 'mark@example.com']
    }
  ];

  // Available users to add
  users = ['john@example.com', 'jane@example.com', 'mark@example.com', 'anna@example.com'];

  selectedMember: string = '';
  showAddTeamForm: boolean = false;

  // Data for creating new team
  newTeam = {
    name: '',
    shortName: '',
    description: ''
  };

  // Excuse yourself from the team (UI only)
  excuseYourself(team: any) {
    console.log(`Excused yourself from team: ${team.name}`);
    // Implement actual logic later
  }

  // Add member to team (UI only)
  addMember(team: any) {
    if (this.selectedMember && !team.members.includes(this.selectedMember)) {
      team.members.push(this.selectedMember);
    }
    this.selectedMember = ''; // Clear selection after adding
  }

  // Toggle new team form
  toggleAddTeamForm() {
    this.showAddTeamForm = !this.showAddTeamForm;
  }

  // Create new team (UI only)
  createTeam() {
    if (this.newTeam.name && this.newTeam.shortName && this.newTeam.description) {
      this.teams.push({
        ...this.newTeam,
        members: [] // Empty members list for new team
      });
      // Clear new team form
      this.newTeam = { name: '', shortName: '', description: '' };
      this.showAddTeamForm = false; // Hide form after creation
    }
  }
}
