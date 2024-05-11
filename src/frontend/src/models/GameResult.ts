import { TeamGameResult } from "./TeamGameResult";


export interface GameResult {
    gameID: string;
    seasonID: string;
    date: Date;
    teamResults: TeamGameResult[];
}