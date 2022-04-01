import {
    Box, Center, Grid, Heading, Image, Link, Text, VStack
} from "@chakra-ui/react";
import * as React from 'react';
import logo from '../images/logo_light.svg';
import bitcoin from './bitcoin.svg';

import { orangeColor } from './Common';
import Button from './Button';

const toSetup =
    (<Link href="/setup" pt={2} pb={2} textDecoration="none !important">
        <Button>
            Log In with Strike to Set Up Your Reacher Email
        </Button>
    </Link>);

const Home: React.FC = () => {
    return (<>
        <Grid minH="100vh" p={3}>
            <Box>
                <VStack spacing={6}>
                    <Center style={{ textAlign: "center" }} pt={4}>
                        <VStack spacing={6}>
                            <Box pb={4}>
                                <Link href="/">
                                    <Image src={logo} height="100px" display="inline" />
                                </Link>
                            </Box>
                            <Heading as="h1" size="2xl">
                                Get Paid to Get Reached
                            </Heading>
                            <Text fontSize="1.5em">
                                Reach someone important
                            </Text>
                        </VStack>
                    </Center>
                    {toSetup}
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
                        </VStack>
                    </Box>
                    {toSetup}
                    <Text pt={6}>
                        New to <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin?{" "}
                        <Link
                            href="https://invite.strike.me/5AL8KE"
                            isExternal
                            color={orangeColor}
                        >
                            Click here
                        </Link>{" "}
                        to download Strike and get started.
                    </Text>
                    <Center pb={6}>
                        <Link mr={2}
                            href="https://strike.me/en/legal/privacy"
                            isExternal
                        >
                            Privacy Notice
                        </Link>{"  |  "}
                        <Link ml={2}
                            href="https://strike.me/en/legal/tos"
                            isExternal
                        >
                            Terms of Service
                        </Link>
                    </Center>
                </VStack>
            </Box>
        </Grid>
    </>);
}
export default Home;