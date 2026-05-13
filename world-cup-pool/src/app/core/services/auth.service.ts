import { Injectable, signal, computed, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/api-models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly api = inject(ApiService);

  private readonly authState = signal<AuthResponse | null>(this.loadAuthFromStorage());

  isAuthenticated = computed(() => !!this.authState()?.token);
  isAdmin = computed(() => this.authState()?.role === 'Admin');
  currentUser = computed(() => this.authState());

  private loadAuthFromStorage(): AuthResponse | null {
    try {
      const stored = localStorage.getItem('auth');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  private saveAuthToStorage(auth: AuthResponse | null): void {
    if (auth) {
      localStorage.setItem('auth', JSON.stringify(auth));
      localStorage.setItem('token', auth.token);
    } else {
      localStorage.removeItem('auth');
      localStorage.removeItem('token');
    }
  }

  register(credentials: RegisterRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/register', credentials).pipe(
      tap(auth => {
        this.authState.set(auth);
        this.saveAuthToStorage(auth);
      })
    );
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.api.post<AuthResponse>('auth/login', credentials).pipe(
      tap(auth => {
        this.authState.set(auth);
        this.saveAuthToStorage(auth);
      })
    );
  }

  logout(): void {
    this.authState.set(null);
    this.saveAuthToStorage(null);
  }

  getToken(): string | null {
    return this.authState()?.token || null;
  }

  getUser(): AuthResponse | null {
    return this.authState();
  }
}
