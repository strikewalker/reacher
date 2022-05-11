import {
    Box, Center, Grid, Heading, Image, Link, Spinner, Text, VStack, Divider, Textarea, FormLabel, FormControl, SimpleGrid, TableContainer, Table, Thead, Tr, Th, Tbody, Td, FormHelperText
} from "@chakra-ui/react";
import * as React from 'react';
import logo from '../images/logo_light.svg';
import { ReacherFooter, orangeColor } from './Common';
import { getUserModel, RecentSender, SetupConfig, updateWhitelist, UserModel } from "./Services/userRepo";
import Setup from "./Setup";
import Button from "./Button";

const MyReacher: React.FC = () => {
    const [editing, setEditing] = React.useState<boolean>(false);
    const [userModel, setUserModel] = React.useState<UserModel>();
    const [error, setError] = React.useState<boolean>(false);
    const [saved, setSaved] = React.useState(false);
    let [isLoading, setLoading] = React.useState<boolean>(false);
    const reloadAccount = async () => {
        try {
            setLoading(true);
            var userModel = await getUserModel();
            if (!userModel) {
                window.location.href = '/account/login';
                return;
            }
            const { config } = userModel!;
            setEditing(!config.reacherEmailPrefix);
            setUserModel(userModel!);
        }
        catch (e: any) {
            console.error(e);
            setError(true);
        }
        finally {
            setLoading(false);
        }
    }
    const onSaved = async (saved?: boolean) => {
        setSaved(!!saved);
        if (saved) {
            setTimeout(() => setSaved(false), 2000);
        }
        reloadAccount();
    }
    const onEdit = async () => {
        setEditing(true);
    }

    React.useEffect(() => {
        reloadAccount();
    }, []);

    if (error)
        return <Text mb={16} color="red">
            An error occurred. Please try again later or see console for details.
        </Text>;

    if (!userModel || isLoading)
        return <Center><Spinner size="xl" /></Center>;

    const { user, config, whitelist, recentSenders } = userModel;

    return (<>
        <Grid minH="100vh" p={3} pt={6}>
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
                        <VStack spacing={4}>
                            <Center><Heading as="h1" size="xl">
                                My Reacher
                            </Heading></Center>
                            <Text>
                                You are logged in as Strike user, <b>{user.name}</b> ({user.strikeUsername}). <br />
                            </Text>
                            <Text fontSize="sm">
                                Not you? <Link color={orangeColor} href="/account/logout">Click Here</Link> to log out.
                            </Text>
                            <Divider />
                            <Setup setupConfig={config} onComplete={onSaved} edit={editing} onEdit={onEdit} />
                            {saved && <Text color="green" textAlign="center">
                                Success! Profile successfully saved.
                            </Text>}
                            {config.reacherEmailPrefix && !editing && <Whitelist emails={whitelist.emailAddresses} recentSenders={recentSenders} />}
                        </VStack>
                    </Box>
                </VStack>
                <ReacherFooter />
            </Box>
        </Grid>
    </>);
}

const Whitelist: React.FC<{ emails: string, recentSenders: RecentSender[] }> = ({ emails: wl, recentSenders }) => {
    const [isSaving, setSaving] = React.useState(false);
    const [error, setError] = React.useState(false);
    const [whitelist, setWhitelist] = React.useState(wl);
    const handleSubmit = async (event: React.FormEvent<any>) => {
        event.preventDefault();
        try {
            setSaving(true);
            setError(false);
            await updateWhitelist(whitelist);
        }
        catch (e) {
            console.error(e);
            setError(true);
        }
        finally {
            setSaving(false);
        }
    }
    return (<VStack spacing={4}>
        <Divider />
        <Heading as="h2" size="md">
            Your Whitelist
        </Heading>
        <VStack spacing={4} textAlign="left">
            <Text fontSize="sm">
                Reacher will automatically forward emails sent by anyone with an email address that <b>ends with</b> the text specified.
                Add <b>one line</b> per pattern you want to match.
            </Text>
            <form onSubmit={handleSubmit} style={{ width: "100%" }}>
                <VStack width="100%" spacing={4}>
                    <FormControl>
                        <FormLabel>Whitelist</FormLabel>
                        <Textarea width="100%" noOfLines={10} placeholder="e.g. noreply@linkedin.com" value={whitelist} onChange={x => setWhitelist(x.currentTarget.value)} />
                        <FormHelperText><b>Please note</b> that emails in the last day that match a new pattern will be automatically forwarded upon save.</FormHelperText>
                    </FormControl>
                    {error && <Text color="red">
                        An error occurred. Please try again or see console for details.
                    </Text>}
                    <Button isLoading={isSaving} disabled={isSaving} type="submit" width="100%">Save Whitelist</Button>
                </VStack>
            </form>
        </VStack>
        <Divider />
        <VStack spacing={4}>
            <Heading as="h3" size="sm">
                Unforwarded Emails In The Last Day
            </Heading>
            {!!recentSenders.length ? <TableContainer>
                <Table size='sm'>
                    <Thead>
                        <Tr>
                            <Th>Name</Th>
                            <Th>Email</Th>
                            <Th isNumeric>Nr. of Emails</Th>
                        </Tr>
                    </Thead>
                    <Tbody>
                        {recentSenders.map(r => <Tr>
                            <Td>{r.fromName}</Td>
                            <Td>{r.emailAddress}</Td>
                            <Td isNumeric>{r.messageCount}</Td>
                        </Tr>)}
                    </Tbody>
                </Table>
            </TableContainer> : <Text fontSize="sm">
                You haven't received any unforwarded emails in the last day
            </Text>}
        </VStack>
        <Divider />
    </VStack>
    )
}
export default MyReacher;