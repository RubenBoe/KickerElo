import { GameResult } from "./GameResult";


export interface PlayerDetailsResult {
    playerID: string;
    nickname: string;
    fullName: string;
    eloNumber: number;
    lastUpdated: Date;
    gamesPlayed: GameResult[];
}