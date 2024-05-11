import axios from "axios"
import { ClientDetails } from "src/models/ClientDetails"


export const getClientDetails = async (): Promise<ClientDetails> => {
    return {
        clientName: "Test",
        creationDate: new Date(),
        currentLeader: {
            eloNumber: 1200,
            lastUpdated: new Date(),
            nickname: "Ruben",
            playerID: "asd"
        },
        numberOfPlayers: 3,
        seasons: [
            {
                endDate: null,
                seasonId: "1",
                seasonNumber: 1,
                startDate: new Date()
            }
        ]
    }
}