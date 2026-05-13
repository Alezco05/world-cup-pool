import { Component, inject } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  // Inyectamos los servicios de forma pública para poder leer sus signals desde el HTML
  public authService = inject(AuthService);
  private readonly router = inject(Router);

  onLogout(): void {
    // Llama al servicio para limpiar el localStorage y resetear el authState signal
    this.authService.logout();
    
    // Redirige de inmediato a la pantalla de login
    this.router.navigate(['/auth/login']);
  }
}
