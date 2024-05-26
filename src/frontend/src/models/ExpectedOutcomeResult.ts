import { TeamResultCommand } from "./TeamResultCommand";


export interface ExpectedOutcomeResult {
    teams: {
        playerIDs: string[];
        points: number;
        teamNumber: number;
    }[]
}