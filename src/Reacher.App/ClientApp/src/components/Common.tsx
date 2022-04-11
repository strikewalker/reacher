import * as React from 'react';
import { Center, Link, Text } from "@chakra-ui/react";

export const orangeColor = "#fdaa26";
export const isTest = !!["localhost", "test"].find(f => window.location.host.indexOf(f) > -1);

export const ReacherFooter = () => (
    <Center pb={6} pt={4} fontSize="sm">
        <Text>
            <Link
                href="https://strike.me/en/legal/privacy"
                isExternal
            >
                Privacy Notice
            </Link>{"  |  "}
            <Link
                href="https://strike.me/en/legal/tos"
                isExternal
            >
                Terms of Service
            </Link>{"  |  "}
            <Link
                href="mailto:support@reacher.me"
                isExternal
            >
                Support
            </Link>
        </Text>
    </Center>
);
