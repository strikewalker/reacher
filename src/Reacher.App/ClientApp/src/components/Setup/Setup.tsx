import {
    Box, Center, FormControl,
    FormLabel, Grid, Heading, Image, Input, InputGroup,
    InputLeftAddon,
    InputRightAddon, Link, 
    NumberInput,
    NumberInputField,
    Spinner,
    Text, VStack
} from "@chakra-ui/react";
import * as React from 'react';
import Button from '../Button';
import logo from '../../images/logo_light.svg';
import { getSetupModel, LoggedInUser, SetupConfig, updateSetupConfig } from './setupRepo';


const isTest = !!["localhost", "test"].find(f => window.location.host.indexOf(f) > -1);
const reacherSuffix = `@${(isTest ? "testing." : "")}reacher.me`;
const userColor = "#fdaa26";

let load = 0;
const Setup: React.FC = () => {
    let [saves, setSaves] = React.useState(0);
    let [saved, setSaved] = React.useState<boolean>();
    let [isLoading, setLoading] = React.useState<boolean>();
    let [error, setError] = React.useState<boolean>();
    let [user, setUser] = React.useState<LoggedInUser>();
    let [setupConfig, setSetupConfigTemp] = React.useState<SetupConfig>();
    const setSetupConfig = (action: (config: SetupConfig) => SetupConfig) => {
        setSetupConfigTemp(action as any)
    }
    const handleSubmit = async (event: React.FormEvent<any>) => {
        event.preventDefault();
        try {
            setLoading(true);
            setError(false);
            await updateSetupConfig(setupConfig);
            setSaved(true);
            setTimeout(() => setSaved(false), 8000);
            setSaves(++saves);
        }
        catch (e) {
            console.error(e);
            setError(true);
        }
        finally {
            setLoading(false);
        }
    }
    const reloadAccount = async () => {
        try {
            var setupModel = await getSetupModel();
            if (!setupModel) {
                window.location.href = '/account/login';
                return;
            }
            const { user, config } = setupModel!;
            setUser(user);
            setSetupConfigTemp(config);
        }
        catch (e: any) {
            console.error(e);
            setError(true);
        }
    }
    React.useEffect(() => {
        reloadAccount();
    }, [saves]);
    if (!user || !setupConfig) {
        if (error) {
            return <Text mb={16} color="red">
                An error occurred. Please try again or see console for details.
            </Text>;
        }
        return <Center><Spinner size="xl" /></Center>;
    }
    return (<>
        <Grid minH="100vh" p={3} pt={6} key={saves}>
            <Box>
                <VStack spacing={4}>
                    <Center style={{ textAlign: "center" }}>
                        <VStack spacing={4}>
                            <Box>
                                <Link href="/">
                                    <Image src={logo} height="100px" display="inline" />
                                </Link>
                            </Box>
                        </VStack>
                    </Center>
                    <Box maxW={600}>
                        <Center><Heading as="h1" size="xl" mb={8} mt={8}>
                            Set Up Your Reacher Email
                        </Heading></Center>
                        <Text mb={4}>
                            You are logged in as Strike user, <b>{user.name}</b> ({user.strikeUsername}). <br />
                        </Text>
                        <Text mb={8} fontSize="sm">
                            Not you? <Link color={userColor} href="/account/logout">Click Here</Link> to log out.
                        </Text>
                        <Text mb={8}>
                            To set up your Reacher email address, provide the details below, and hit 'Submit' to save your changes.
                        </Text>
                        <form onSubmit={handleSubmit}>
                            <VStack
                                spacing={4}
                            >
                                <FormControl>
                                    <FormLabel>Reacher Email</FormLabel>
                                    <InputGroup>
                                        <Input
                                            placeholder="yourname"
                                            value={setupConfig!.reacherEmailPrefix}
                                            onChange={v => setSetupConfig(c => ({ ...c, reacherEmailPrefix: v.target.value }))}
                                            required
                                            autoComplete="off"
                                        />
                                        <InputRightAddon children={reacherSuffix} />
                                    </InputGroup>
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Display Name</FormLabel>
                                    <Input
                                        placeholder="Satoshi Nakamoto"
                                        value={setupConfig!.name}
                                        onChange={v => setSetupConfig(c => ({ ...c, name: v.target.value }))}
                                        required
                                        autoComplete="off"
                                    />
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Strike Username</FormLabel>
                                    <Input
                                        placeholder="satoshi"
                                        value={setupConfig!.strikeUsername}
                                        onChange={v => setSetupConfig(c => ({ ...c, strikeUsername: v.target.value }))}
                                        required
                                        autoComplete="off"
                                    />
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Destination Email</FormLabel>
                                    <Input
                                        type="email"
                                        placeholder="user@email.com"
                                        value={setupConfig!.destinationEmail}
                                        onChange={v => setSetupConfig(c => ({ ...c, destinationEmail: v.target.value }))}
                                        required
                                        autoComplete="off"
                                    />
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Tip Amount to be Reached (USD)</FormLabel>
                                    <InputGroup>
                                        <InputLeftAddon children={`$`} />
                                        <Input
                                            placeholder="2.00"
                                            defaultValue={setupConfig!.price?.toFixed(2)}
                                            onChange={v => {
                                                var value = parseFloat(v.target.value);
                                                if (value > 0) {
                                                    setSetupConfig(c => ({ ...c, price: parseFloat(value.toFixed(2)) }))
                                                }
                                            }}
                                            required
                                            autoComplete="off"
                                        />
                                    </InputGroup>
                                </FormControl>
                                {error && <Text color="red">
                                    An error occurred. Please try again or see console for details.
                                </Text>}
                                {saved && <Text color="green" textAlign="center">
                                    Success! Emails sent to {setupConfig.reacherEmailPrefix}{reacherSuffix}<br />
                                    with a tip will be sent to {setupConfig.destinationEmail}.
                                </Text>}
                                <Button isLoading={isLoading} type="submit" width="100%">
                                    Submit
                                </Button>
                            </VStack>
                        </form>
                    </Box>
                </VStack>
            </Box>
        </Grid>
    </>);
}
export default Setup;