import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        // 🚀 Carga perezosa del componente de Login
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        // 🚀 Carga perezosa del componente de Registro
        loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'leaderboard',
    // 🚀 Carga perezosa del Ranking Global
    loadComponent: () => import('./features/leaderboard/leaderboard.component').then(m => m.LeaderboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
      loadComponent: () => import('./features/admin/admin.component').then(m => m.AdminComponent),
    canActivate: [adminGuard]
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
