import { PlayerGameResult } from "./PlayerGameResult";


export interface TeamGameResult {
    playerResults: PlayerGameResult[];
    teamNumber: number;
    points: number;
}