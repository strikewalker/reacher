
export async function fetchInvoiceIsPaid(strikeInvoiceId: string) {
    const response = await fetch(`/api/invoice/${strikeInvoiceId}/paid`);
    return response.json() as Promise<InvoiceStatus>;
}

export async function fetchInvoice(emailId: string) {
    const response = await fetch(`/api/invoice/${emailId}`);
    return response.json() as Promise<Invoice>;
}

export async function createLnInvoice(strikeInvoiceId: string) {
    const response = await fetch(`/api/invoice/${strikeInvoiceId}/lnInvoice`, { method: "post" });
    return response.json() as Promise<LightningInvoice>;
}
export interface Invoice {
    id: string;
    strikeInvoiceId: string;
    costUsd: number;
    paid: boolean;
    reachable: Reachable;
}
interface Reachable {
    id: string;
    name: string;
    description: string;
    emailAddress: string;
}

interface InvoiceStatus {
    paid: boolean;
}

export interface LightningInvoice {
    lnInvoiceId: string;
    expirationInSeconds: number;
}