import { Component, inject, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatchService } from '../../core/services/match.service';
import { LeaderboardEntryDto, PredictionHistoryResponse } from '../../core/models/api-models';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [FormsModule], // 💡 Eliminado CommonModule ya que usamos el flujo nativo @if y @for
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.css']
})
export class LeaderboardComponent implements OnInit {
  private matchService = inject(MatchService);

  leaderboard = signal<LeaderboardEntryDto[]>([]);
  selectedUserHistory = signal<PredictionHistoryResponse[] | null>(null);
  selectedUserId = signal<number | null>(null);
  isLoading = signal<boolean>(true);
  isLoadingHistory = signal<boolean>(false);
  errorMessage = signal<string>('');

  ngOnInit() {
    this.loadLeaderboard();
  }

  loadLeaderboard() {
    this.isLoading.set(true);
    this.matchService.getLeaderboard().subscribe({
      next: (data: LeaderboardEntryDto[]) => {
        this.leaderboard.set(data);
        this.isLoading.set(false);
      },
      error: (error: any) => {
        this.errorMessage.set('Error al cargar el ranking.');
        console.error('Error loading leaderboard:', error);
        this.isLoading.set(false);
      }
    });
  }

  selectUser(user: LeaderboardEntryDto) {
    if (this.selectedUserId() === user.userId) {
      this.selectedUserHistory.set(null);
      this.selectedUserId.set(null);
    } else {
      this.selectedUserId.set(user.userId);
      this.loadUserHistory(user.userId);
    }
  }

  loadUserHistory(userId: number) {
    this.isLoadingHistory.set(true);
    this.matchService.getUserPredictionHistory(userId).subscribe({
      next: (data: PredictionHistoryResponse[]) => {
        this.selectedUserHistory.set(data);
        this.isLoadingHistory.set(false);
      },
      error: (error: any) => {
        this.errorMessage.set('Error al cargar el historial del usuario.');
        console.error('Error loading user history:', error);
        this.isLoadingHistory.set(false);
      }
    });
  }

  closeHistory() {
    this.selectedUserHistory.set(null);
    this.selectedUserId.set(null);
  }

  getResultDisplay(prediction: PredictionHistoryResponse): string {
    if (prediction.homeScore === null || prediction.awayScore === null) {
      return 'Pendiente';
    }
    return `${prediction.homeScore} - ${prediction.awayScore}`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
