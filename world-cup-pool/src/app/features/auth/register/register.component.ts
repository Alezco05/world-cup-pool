import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/api-models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterModule], // 💡 Eliminado CommonModule
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm: FormGroup = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  errorMessage = signal<string>('');
  isLoading = signal<boolean>(false);

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading.set(true);
      const credentials: RegisterRequest = this.registerForm.value;
      
      this.authService.register(credentials).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.errorMessage.set('');
          this.router.navigate(['/auth/login']);
        },
        error: (error) => {
          this.isLoading.set(false);
          this.errorMessage.set('El correo ya está registrado o hubo un error.');
          console.error('Register error:', error);
        }
      });
    }
  }

  getUsernameErrorMessage(): string {
    const control = this.registerForm.get('username');
    if (control?.hasError('required')) return 'El nombre de usuario es requerido.';
    if (control?.hasError('minlength')) return 'El nombre debe tener al menos 3 caracteres.';
    return '';
  }

  getEmailErrorMessage(): string {
    const control = this.registerForm.get('email');
    if (control?.hasError('required')) return 'El correo es requerido.';
    if (control?.hasError('email')) return 'Ingresa un correo válido.';
    return '';
  }

  getPasswordErrorMessage(): string {
    const control = this.registerForm.get('password');
    if (control?.hasError('required')) return 'La contraseña es requerida.';
    if (control?.hasError('minlength')) return 'La contraseña debe tener al menos 6 caracteres.';
    return '';
  }
}
