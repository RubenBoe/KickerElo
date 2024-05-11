import { PlayerResult } from "./PlayerResult";
import { SeasonResult } from "./SeasonResult";

export interface ClientDetails {
    clientName: string;
    creationDate: Date;
    numberOfPlayers: number;
    seasons: SeasonResult[];
    currentLeader: PlayerResult;
}