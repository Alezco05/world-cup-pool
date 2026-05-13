import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatchService } from '../../core/services/match.service';
import { Match } from '../../core/models/api-models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  private matchService = inject(MatchService);

  matches = signal<Match[]>([]);
  isLoading = signal<boolean>(true);
  successMessage = signal<string>('');
  errorMessage = signal<string>('');
  editingMatchId = signal<number | null>(null);
  editScores = signal<Map<number, { home: number; away: number }>>(new Map());
  isSaving = signal<Map<number, boolean>>(new Map());

  ngOnInit() {
    this.loadMatches();
  }

  loadMatches() {
    this.isLoading.set(true);
    this.matchService.getMatches().subscribe({
      next: (data: Match[]) => {
        this.matches.set(data);
        // Inicializar con los scores actuales
        const scores = new Map<number, { home: number; away: number }>();
        data.forEach(match => {
          scores.set(match.id, {
            home: match.homeScore ?? 0,
            away: match.awayScore ?? 0
          });
        });
        this.editScores.set(scores);
        this.isLoading.set(false);
      },
      error: (error: any) => {
        this.errorMessage.set('Error al cargar los partidos.');
        console.error('Error loading matches:', error);
        this.isLoading.set(false);
      }
    });
  }

  startEditing(matchId: number) {
    this.editingMatchId.set(matchId);
  }

  cancelEditing() {
    this.editingMatchId.set(null);
  }

  updateScore(matchId: number, type: 'home' | 'away', value: string) {
    const numValue = parseInt(value, 10) || 0;
    const scores = new Map(this.editScores());
    const current = scores.get(matchId);
    if (current) {
      scores.set(matchId, { ...current, [type]: numValue });
      this.editScores.set(scores);
    }
  }

  saveScore(matchId: number) {
    const score = this.editScores().get(matchId);
    if (score !== undefined) {
      this.setSavingState(matchId, true);
      this.matchService.updateMatchScore(matchId, score.home, score.away).subscribe({
        next: () => {
          this.successMessage.set('Marcador actualizado exitosamente.');
          this.editingMatchId.set(null);
          this.setSavingState(matchId, false);
          setTimeout(() => this.successMessage.set(''), 3000);
          this.loadMatches();
        },
        error: (error: any) => {
          this.errorMessage.set('Error al actualizar el marcador.');
          console.error('Error updating score:', error);
          this.setSavingState(matchId, false);
          setTimeout(() => this.errorMessage.set(''), 3000);
        }
      });
    }
  }

  private setSavingState(matchId: number, isSaving: boolean) {
    const saving = new Map(this.isSaving());
    if (isSaving) {
      saving.set(matchId, true);
    } else {
      saving.delete(matchId);
    }
    this.isSaving.set(saving);
  }

  getMatchesByGroup(group: string): Match[] {
    return this.matches().filter(match => match.group === group);
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

  getScoreDisplay(match: Match): string {
    if (match.homeScore === null || match.awayScore === null) {
      return 'Por definir';
    }
    return `${match.homeScore} - ${match.awayScore}`;
  }
}