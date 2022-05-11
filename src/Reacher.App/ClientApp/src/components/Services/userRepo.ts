
export async function getUserModel() {
    const response = await fetch(`/api/user`, { redirect: 'manual' });
    if (!response.status || response.status === 302) {
        return null;
    }
    return response.json() as Promise<UserModel>;
}

export async function getCurrency(strikeUsername: string) {
    const response = await fetch(`/api/user/currency?strikeUsername=${strikeUsername}`);
    return response.status === 204 ? null : response.text();
}
export async function getReacherEmailAvailable(prefix: string) {
    const response = await fetch(`/api/user/isavailable?prefix=${prefix}`);
    return response.status === 204 ? false : response.json() as Promise<boolean>;
}

export async function updateSetupConfig(config?: SetupConfig) {
    await fetch(`/api/user`, {
        method: 'post',
        body: JSON.stringify(config),
        headers: {
            'Content-Type': 'application/json'
        },
    })
}

export async function updateWhitelist(whitelist: string) {
    await fetch(`/api/user/whitelist`, {
        method: 'put',
        body: JSON.stringify({ emailAddresses: whitelist }),
        headers: {
            'Content-Type': 'application/json'
        },
    })
}

export interface UserModel {
    user: LoggedInUser;
    config: SetupConfig;
    recentSenders: RecentSender[];
    whitelist: Whitelist
}

export interface RecentSender {
    emailAddress: string,
    fromName: string,
    messageCount: number
}

export interface Whitelist {
    emailAddresses: string;
}

export interface SetupConfig {
    name?: string;
    strikeUsername?: string;
    price?: number;
    reacherEmailPrefix?: string;
    destinationEmail?: string;
    currency?: string;
    disabled: boolean;
}

export interface LoggedInUser {
    email: string;
    id: string;
    name: string;
    strikeUsername: string;
}