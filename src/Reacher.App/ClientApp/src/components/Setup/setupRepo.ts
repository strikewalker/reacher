
export async function getSetupModel() {
    const response = await fetch(`/api/setup`, { redirect: 'manual' });
    if (!response.status || response.status === 302) {
        return null;
    }
    return response.json() as Promise<SetupModel>;
}

export async function getCurrency(strikeUsername: string) {
    const response = await fetch(`/api/setup/currency?strikeUsername=${strikeUsername}`);
    return response.status === 204 ? null : response.text();
}
export async function getReacherEmailAvailable(prefix: string) {
    const response = await fetch(`/api/setup/isavailable?prefix=${prefix}`);
    return response.status === 204 ? false : response.json() as Promise<boolean>;
}

export async function updateSetupConfig(config?: SetupConfig) {
    await fetch(`/api/setup`, {
        method: 'post',
        body: JSON.stringify(config),
        headers: {
            'Content-Type': 'application/json'
        },
    })
}

export interface SetupModel {
    user: LoggedInUser;
    config: SetupConfig;
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