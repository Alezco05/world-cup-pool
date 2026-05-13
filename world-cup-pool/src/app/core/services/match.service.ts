import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Match, PredictionRequest, PredictionHistoryResponse, LeaderboardEntryDto } from '../models/api-models';

@Injectable({
  providedIn: 'root'
})
export class MatchService {
  private readonly api = inject(ApiService);

  getMatches(): Observable<Match[]> {
    return this.api.get<Match[]>('matches');
  }

  submitPrediction(prediction: PredictionRequest): Observable<any> {
    return this.api.post('predictions', prediction);
  }

  getLeaderboard(): Observable<LeaderboardEntryDto[]> {
    return this.api.get<LeaderboardEntryDto[]>('leaderboard');
  }

  getUserPredictionHistory(userId: number): Observable<PredictionHistoryResponse[]> {
    return this.api.get<PredictionHistoryResponse[]>(`leaderboard/user/${userId}`);
  }

  updateMatchScore(matchId: number, homeScore: number, awayScore: number): Observable<any> {
    return this.api.post(`admin/matches/${matchId}/score`, {
      homeScore,
      awayScore
    });
  }
}
