import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatchService } from '../../core/services/match.service';
import { Match, PredictionRequest } from '../../core/models/api-models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [FormsModule], // 💡 Eliminamos CommonModule porque usaremos @if nativo
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  private matchService = inject(MatchService);

  matches = signal<Match[]>([]);
  successMessage = signal<string>('');
  errorMessage = signal<string>('');
  predictions = signal<Map<number, { home: number; away: number }>>(new Map());
  isLoading = signal<boolean>(true);

  ngOnInit() {
    this.loadMatches();
  }

  loadMatches() {
    this.isLoading.set(true);
    this.matchService.getMatches().subscribe({
      next: (data: Match[]) => {
        this.matches.set(data);
        const preds = new Map<number, { home: number; away: number }>();
        data.forEach((match: Match) => {
          preds.set(match.id, { home: 0, away: 0 });
        });
        this.predictions.set(preds);
        this.isLoading.set(false);
      },
      error: (error: any) => {
        this.errorMessage.set('Error al cargar los partidos.');
        console.error('Error loading matches:', error);
        this.isLoading.set(false);
      }
    });
  }

  savePrediction(matchId: number) {
    const pred = this.predictions().get(matchId);
    if (pred !== undefined) {
      const prediction: PredictionRequest = {
        matchId,
        predictedHomeScore: pred.home,
        predictedAwayScore: pred.away
      };
      this.matchService.submitPrediction(prediction).subscribe({
        next: () => {
          this.successMessage.set('¡Pronóstico guardado exitosamente!');
          setTimeout(() => this.successMessage.set(''), 3000);
        },
        error: (error: any) => {
          this.errorMessage.set('Error al guardar el pronóstico.');
          console.error('Error saving prediction:', error);
          setTimeout(() => this.errorMessage.set(''), 3000);
        }
      });
    }
  }

  updatePrediction(matchId: number, type: 'home' | 'away', value: string) {
    const numValue = parseInt(value, 10) || 0;
    const preds = new Map(this.predictions());
    const current = preds.get(matchId);
    if (current) {
      preds.set(matchId, { ...current, [type]: numValue });
      this.predictions.set(preds);
    }
  }

  getMatchesByGroup(group: string): Match[] {
    return this.matches().filter(match => match.group === group);
  }

  isMatchOpen(match: Match): boolean {
  // 0 = Scheduled, 1 = Live. Ambos permitirían apostar.
  return match.status === 0 || match.status === 1 || match.status === 'Scheduled';
}


  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getFlagEmoji(countryName: string): string {
    if (!countryName) return '🏳️';

    const flags: { [key: string]: string } = {
      'argentina': '🇦🇷',
      'argenti': '🇦🇷', // Manejo del texto cortado
      'mexico': '🇲🇽',
      'poland': '🇵🇱',
      'saudi arabia': '🇸🇦',
      'france': '🇫🇷',
      'australia': '🇦🇺',
      'denmark': '🇩🇰',
      'tunisia': '🇹🇳'
    };

    return flags[countryName.toLowerCase().trim()] || '🏳️';
  }
}
