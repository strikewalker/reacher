import {
    Box, Center, Grid, Heading, Image, Link, SimpleGrid, Text, VStack, Divider
} from "@chakra-ui/react";
import * as React from 'react';
import logo from '../images/logo_light.svg';
import bitcoin from '../images/bitcoin.svg';
import login from '../images/login.svg';
import reachOut from '../images/reachOut.svg';
import tipRequest from '../images/tipRequest.svg';
import pay from '../images/pay.svg';
import delivered from '../images/delivered.svg';

import { orangeColor, ReacherFooter } from './Common';
import Button from './Button';

const toSetup =
    (<VStack>
        <Button href="/myreacher">Log In with Strike</Button>
        <Text fontSize="sm">to set up your <b>Reacher</b> email</Text>
    </VStack>);

const spacing = { base: 3, md: 5, lg: 10 };

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
                                Filter out the noise
                            </Text>
                        </VStack>
                    </Center>
                    {toSetup}
                    <Box border="2px solid white" borderRadius={20} padding={spacing} maxW="3xl">
                        <Heading as="h4" textAlign="center" mb={2}>
                            How it works
                        </Heading>
                        <Divider mb={spacing} />
                        <SimpleGrid columns={2} spacing={spacing}>
                            <Box>
                                <Heading as="h5" fontSize="2xl" mb={1}>Step 1</Heading>
                                <Text>Sign up for a free <b>Reacher</b> email address that forwards to your actual email address.</Text>
                            </Box>
                            <Image src={login} alt='Sign up for Reacher email' />
                            <Image src={reachOut} alt='Someone sends an email' />
                            <Box>
                                <Heading as="h5" fontSize="2xl" mb={1}>Step 2</Heading>
                                <Text>Someone sends an email to your <b>Reacher</b> email address.</Text>
                            </Box>
                            <Box>
                                <Heading as="h5" fontSize="2xl" mb={1}>Step 3</Heading>
                                <Text>They get an email from <i>Reacher</i> requesting a <Image src={bitcoin} height="1.3rem" display="inline" /> Bitcoin tip to ensure you see their email.</Text>
                            </Box>
                            <Image src={tipRequest} alt='They get a tip request' />
                            <Image src={pay} alt='Reacher website for payment' />
                            <Box>
                                <Heading as="h5" fontSize="2xl" mb={1}>Step 4</Heading>
                                <Text>They get directed to the Reacher website to pay the tip.</Text>
                            </Box>
                            <Box>
                                <Heading as="h5" fontSize="2xl" mb={1}>Step 5</Heading>
                                <Text mb={2}>You receive an email from <i>Reacher</i> with their message.</Text>
                                <Text>You can reply to this email to respond to them.</Text>
                            </Box>
                            <Image src={delivered} alt='Their email gets delivered' />
                        </SimpleGrid>
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
                    <ReacherFooter/>
                </VStack>
            </Box>
        </Grid>
    </>);
}
export default Home;