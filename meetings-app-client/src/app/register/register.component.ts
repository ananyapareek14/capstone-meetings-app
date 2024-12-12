// import { Component } from '@angular/core';
// import { Router,RouterLink } from '@angular/router';
// import { AuthService } from '../services/auth/auth.service';
// import { IRegister } from '../models/iregister';

// @Component({
//   selector: 'app-register',
//   standalone: true,
//   imports: [RouterLink],
//   templateUrl: './register.component.html',
//   styleUrl: './register.component.scss'
// })
// export class RegisterComponent {
//   registrationData: IRegister = {
//     name: '',
//     email: '',
//     password: ''
//   };

//   constructor(private authService: AuthService, private router: Router) {}

//   register() {
//     this.authService.register(this.registrationData).subscribe({
//       next: () => {
//         alert('Registration successful! Redirecting to login page.');
//         this.router.navigateByUrl('/login');
//       },
//       error: (err) => {
//         console.error('Registration failed:', err);
//         alert('Registration failed. Please try again.');
//       }
//     });
//   }
// }


import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  registerForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group(
      {
        name: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required],
      },
      { validator: this.matchPasswords('password', 'confirmPassword') }
    );
  }

  matchPasswords(password: string, confirmPassword: string) {
    return (formGroup: FormGroup) => {
      const passControl = formGroup.controls[password];
      const confirmPassControl = formGroup.controls[confirmPassword];

      if (confirmPassControl.errors && !confirmPassControl.errors['passwordMismatch']) {
        return;
      }

      if (passControl.value !== confirmPassControl.value) {
        confirmPassControl.setErrors({ passwordMismatch: true });
      } else {
        confirmPassControl.setErrors(null);
      }
    };
  }

  register() {
    if (this.registerForm.invalid) {
      alert('Please fix the errors in the form before submitting.');
      return;
    }

    const { name, email, password } = this.registerForm.value;

    this.authService.register({ name, email, password }).subscribe({
      next: () => {
        alert('Registration successful! Redirecting to login page.');
        this.router.navigateByUrl('/login');
      },
      error: (err) => {
        console.error('Registration failed:', err);
        alert('Registration failed. Please try again.');
      },
    });
  }
}

