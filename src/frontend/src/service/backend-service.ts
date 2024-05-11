import axios from "axios"
import { ClientDetails } from "src/models/ClientDetails"

// const apiUrl = process.env["NX_API_URL"];
const apiUrl = "http://localhost:7123/api/";

axios.interceptors.response.use(originalResponse => {
    handleDates(originalResponse.data);
    return originalResponse;
})

const isoDateFormat = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2}(?:\.\d*)?)((-(\d{2}):(\d{2})|Z)?)$/;

function isIsoDateString(value: any): boolean {
  return value && typeof value === "string" && isoDateFormat.test(value);
}

export function handleDates(body: any) {
  if (body === null || body === undefined || typeof body !== "object")
    return body;

  for (const key of Object.keys(body)) {
    const value = body[key];
    if (isIsoDateString(value)) body[key] = new Date(value);
    else if (typeof value === "object") handleDates(value);
  }
}

export const getClientDetails = async (clientToken: string): Promise<ClientDetails> => {
    return axios.get<ClientDetails>(`${apiUrl}Client/${clientToken}`).then(res => res.data)
}