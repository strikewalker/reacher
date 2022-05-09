import {
    Box, Center, Grid, Heading, Image, Link, Spinner, Text, VStack, Divider
} from "@chakra-ui/react";
import * as React from 'react';
import logo from '../images/logo_light.svg';
import { ReacherFooter, orangeColor } from './Common';
import { getUserModel, SetupConfig, UserModel } from "./Services/userRepo";
import Setup from "./Setup";

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

    const { user, config } = userModel;

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

                        <VStack
                            spacing={4}
                        >
                        <Center><Heading as="h1" size="xl">
                            My Reacher
                        </Heading></Center>
                        <Text>
                            You are logged in as Strike user, <b>{user.name}</b> ({user.strikeUsername}). <br />
                        </Text>
                        <Text fontSize="sm">
                            Not you? <Link color={orangeColor} href="/account/logout">Click Here</Link> to log out.
                        </Text>
                        <Divider/>
                        <Setup setupConfig={config} onComplete={onSaved} edit={editing} onEdit={onEdit} />
                    
                        {saved && <Text color="green" textAlign="center">
                            Success! Profile successfully saved.
                            </Text>}
                            <Divider />
                            <Whitelist />
                            <RecentSenders senders={ }
                        </VStack>
                    </Box>
                </VStack>
                <ReacherFooter />
            </Box>
        </Grid>
    </>);
}
export default MyReacher;