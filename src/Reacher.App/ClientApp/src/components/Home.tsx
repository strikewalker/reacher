import * as React from 'react';

import {
    Heading,
    Text,
    FormControl,
    FormLabel,
    Link,
    Stack,
    Box,
    Flex,
    Input,
    Image,
    Grid,
    Center,
    VStack,
    Button
} from "@chakra-ui/react";
import logo from '../images/logo_light.svg'
import bitcoin from './bitcoin.svg'

const userColor = "#fdaa26";

const Home: React.FC = () => {
    return (<>
        <Grid minH="100vh" p={3}>
            <Box maxW="m">
                <VStack spacing={4}>
                    <Center style={{ textAlign: "center" }}>
                        <VStack spacing={4}>
                            <Box>
                                <Link href="/">
                                    <Image src={logo} height="140px" display="inline" />
                                </Link>
                            </Box>
                            <Heading as="h1" size="2xl" mb={4}>
                                The Best Way to Get Reached
                            </Heading>
                            <Heading as="h2">
                                and reduce SPAM
                            </Heading>
                        </VStack>
                    </Center>
                    <Box border="2px solid white" borderRadius={20} padding={10}>
                        <VStack spacing={4} style={{ alignContent: "flex-start", width: "100%" }}>
                            <Heading as="h4">
                                How it works
                            </Heading>
                            <Text width="100%">
                                <b>Step 1.</b><br /> Sign up for a free <b>Reacher</b> email address (e.g. <i>example@reacher.me</i>) that forwards to your actual email address (e.g. <i>personal@gmail.com</i>).
                            </Text>
                            <Text width="100%">
                                <b>Step 2.</b><br /> You make your <b>Reacher</b> email address (e.g. <i>example@reacher.me</i>) public (e.g. in your Twitter profile).
                            </Text>
                            <Text width="100%">
                                <b>Step 3.</b><br /> Someone sends you an email
                            </Text>
                            <Text width="100%">
                                <b>Step 4.</b><br /> They get an email from <i>Reacher</i> requesting a <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin payment to ensure you see their email.
                            </Text>
                            <Text width="100%">
                                <b>Step 5.</b><br /> They make the payment, and their email gets sent to your actual email address (e.g. <i>personal@gmail.com</i>)
                            </Text>
                            <Text width="100%">
                                <b>Step 6.</b><br /> You respond from your email address through <i>Reacher</i>, and the recipient never sees your actual email address.
                            </Text>
                            <Link href="/setup">
                                <Button>
                                    Log In to Strike and Set Up Your Reacher Email
                                </Button>
                            </Link>
                        </VStack>
                    </Box>
                    <Text>
                        New to <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin?{" "}
                        <Link
                            href="https://strike.me/download"
                            isExternal
                            color={userColor}
                        >
                            Click here
                        </Link>{" "}
                        to download Strike and get started.
                    </Text>
                </VStack>
            </Box>
        </Grid>
    </>);
}
export default Home;