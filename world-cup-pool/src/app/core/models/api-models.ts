// ============================================
// AUTH MODELS
// ============================================

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  id: number;
  username: string;
  email: string;
  role: 'User' | 'Admin';
}

// ============================================
// MATCH MODELS
// ============================================

export interface Match {
  id: number;
  homeTeam: string;
  awayTeam: string;
  group: string;
  matchDate: string; // ISO 8601 date string
  homeScore: number | null;
  awayScore: number | null;
  status: 'Scheduled' | 'Finished';
  homeFlag?: string;
  awayFlag?: string;
}

// ============================================
// PREDICTION MODELS
// ============================================

export interface PredictionRequest {
  matchId: number;
  predictedHomeScore: number;
  predictedAwayScore: number;
}

export interface PredictionHistoryResponse {
  predictionId: string;
  matchId: number;
  homeTeam: string;
  awayTeam: string;
  predictedHomeScore: number;
  predictedAwayScore: number;
  homeScore: number | null;
  awayScore: number | null;
  pointsEarned: number;
  status: 'Scheduled' | 'Finished';
  matchDate: string;
}

// ============================================
// LEADERBOARD MODELS
// ============================================

export interface LeaderboardEntryDto {
  userId: number;
  username: string;
  totalPoints: number;
}
