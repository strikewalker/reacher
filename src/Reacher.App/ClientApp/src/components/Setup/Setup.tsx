import {
    Box, Button, Center, FormControl,
    FormLabel, Grid, Heading, Image, Input, InputGroup,
    InputLeftAddon,
    InputRightAddon, Link,
    NumberInput,
    NumberInputField,
    Spinner,
    Stack, Text, VStack
} from "@chakra-ui/react";
import * as React from 'react';
import logo from '../../images/logo_light.svg';
import { getSetupModel, LoggedInUser, SetupConfig, updateSetupConfig } from './setupRepo';


const isTest = !!["localhost", "test"].find(f => window.location.host.indexOf(f) > -1);
const reacherSuffix = `@${(isTest ? "test." : "")}reacher.me`;
const userColor = "#fdaa26";

const Setup: React.FC = () => {
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
        }
        catch (e) {
            console.error(e);
            setError(true);
        }
        finally {
            setLoading(false);
        }
    }
    React.useEffect(() => {
        (async () => {
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
        })();
    }, []);
    if (!user || !setupConfig) {
        if (error) {
            return <Text mb={16} color="red">
                An error occurred. Please try again or see console for details.
            </Text>;
        }
        return <Center><Spinner size="xl" /></Center>;
    }
    return (<>
        <Grid minH="100vh" p={3} pt={6}>
            <Box maxW="l">
                <VStack spacing={4}>
                    <Center style={{ textAlign: "center" }}>
                        <VStack spacing={4}>
                            <Box>
                                <Link href="/">
                                    <Image src={logo} height="140px" display="inline" />
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
                        <Text mb={8}>
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
                                            name="reacheremail"
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
                                        name="displayname"
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
                                        name="username"
                                        placeholder="satoshi"
                                        value={setupConfig!.strikeUsername}
                                        onChange={v => setSetupConfig(c => ({ ...c, strikeUser: v.target.value }))}
                                        required
                                        autoComplete="off"
                                    />
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Destination Email</FormLabel>
                                    <Input
                                        name="email"
                                        type="email"
                                        placeholder="user@email.com"
                                        value={setupConfig!.destinationEmail}
                                        onChange={v => setSetupConfig(c => ({ ...c, destinationEmail: v.target.value }))}
                                        required
                                        autoComplete="off"
                                    />
                                </FormControl>
                                <FormControl>
                                    <FormLabel>Tip Amount to Reach(USD)</FormLabel>
                                    <InputGroup>
                                        <InputLeftAddon children={`$`} />
                                        <NumberInput width="100%" defaultValue={setupConfig!.price} isRequired onChange={(_, v) => setSetupConfig(c => ({ ...c, price: v }))} min={0.01} precision={2}>
                                            <NumberInputField placeholder="2.00" type="number" />
                                        </NumberInput>
                                    </InputGroup>
                                </FormControl>
                                {error && <Text color="red">
                                    An error occurred. Please try again or see console for details.
                                </Text>}
                                {saved && <Text color="green">
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