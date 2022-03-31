import useSWR from "swr";
import { fetchInvoiceIsPaid } from './invoiceRepo'

const useInvoiceStatePoller = (invoiceId: string) => {
    const { data, error } = useSWR(invoiceId, fetchInvoiceIsPaid, {
        refreshInterval: 2000,
    });

    if (error) {
        console.log(error);
    }

    return data;
};




export default useInvoiceStatePoller;
