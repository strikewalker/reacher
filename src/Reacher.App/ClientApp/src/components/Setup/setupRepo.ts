
export async function getSetupModel() {
    const response = await fetch(`/api/setup`, { redirect: 'manual' });
    if (!response.status || response.status === 302) {
        return null;
    }
    return response.json() as Promise<SetupModel>;
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
}

export interface LoggedInUser {
    email: string;
    id: string;
    name: string;
    strikeUsername: string;
}