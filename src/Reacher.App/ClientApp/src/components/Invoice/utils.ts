const normalizeCurrency = (currency: string) => {
    switch (currency) {
        case "USDT":
            return "USD";
        default:
            return currency;
    }
};

export const formatCurrency = ({ amount, currency = "USD", locales = "en" }: { amount: number, currency: string, locales: string }) => {
    return new Intl.NumberFormat(locales, {
        style: "currency",
        currency: normalizeCurrency(currency),
    })
        .format(amount)
        .replace(/\.00$/, "");
};
